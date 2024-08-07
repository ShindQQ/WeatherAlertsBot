﻿using System.Globalization;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using WeatherAlertsBot.DAL.Entities;
using WeatherAlertsBot.Helpers;
using WeatherAlertsBot.OpenWeatherApi;
using WeatherAlertsBot.RequestHandlers;
using WeatherAlertsBot.RussianWarship;
using WeatherAlertsBot.RussianWarship.LiquidationsInfo;
using WeatherAlertsBot.UserServices;

namespace WeatherAlertsBot.TelegramBotHandlers;

/// <summary>
///     Telegram Bot Update Handler
/// </summary>
public sealed class UpdateHandler : IUpdateHandler
{
    private static readonly char[] Separators = [' '];

    /// <summary>
    ///     A client interface to use Telegram Bot API
    /// </summary>
    private readonly ITelegramBotClient _botClient;

    /// <summary>
    ///     Cancellation Token
    /// </summary>
    private readonly CancellationTokenSource _cancellationTokenSource;

    /// <summary>
    ///     Subscriber service to work with db
    /// </summary>
    private readonly ISubscriberRepository _subscriberRepository;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="telegramBotClient">A client interface to use Telegram Bot API</param>
    /// <param name="subscriberRepository">Service for work with db context</param>
    /// <param name="cancellationTokenSource">Cancellation Token Source</param>
    public UpdateHandler(
        ITelegramBotClient telegramBotClient,
        ISubscriberRepository subscriberRepository,
        CancellationTokenSource cancellationTokenSource)
    {
        _botClient = telegramBotClient;
        _subscriberRepository = subscriberRepository;
        _cancellationTokenSource = cancellationTokenSource;
    }

    /// <summary>
    ///     Handling user message
    /// </summary>
    /// <param name="update">Variable for handling user chat info and messages</param>
    /// <returns>Task</returns>
    public async Task HandleMessageAsync(Update update)
    {
        var userMessage = update.Message;

        if (userMessage is null)
            return;

        var userMessageText = userMessage.Text;
        var chatId = userMessage.Chat.Id;

        if (userMessageText is not null)
        {
            var handledText = userMessageText
                .Replace("*", @"\*")
                .Replace(",", @"\,")
                .Replace(",", @"\,")
                .Replace("~", @"\~")
                .Replace("`", @"\`")
                .Replace(">", @"\>")
                .Replace("<", @"\<")
                .Replace("#", @"\#")
                .Replace("+", @"\+")
                .Replace("=", @"\=")
                .Replace("|", @"\|")
                .Replace("{", @"\}")
                .Replace("{", @"\{")
                .Replace("!", @"\!")
                .Replace(".", @"\.")
                .Replace("-", @"\-");
            await HandleCommandMessage(chatId, handledText);
        }

        
        var userMessageLocation = userMessage.Location;

        if (userMessageLocation is not null)
            await HandleLocationMessageAsync(chatId, userMessageLocation);

        if (userMessageText is null && userMessageLocation is null && userMessage.Type == MessageType.Text)
            await HandleErrorMessage(chatId);
    }

    /// <summary>
    ///     Handling sending notifications to users
    /// </summary>
    /// <returns>Task</returns>
    public async Task HandleSubscribersNotificationsAsync()
    {
        var subscribers = await _subscriberRepository.GetSubscribersAsync();

        foreach (var subscriber in subscribers)
        foreach (var subscriberCommand in subscriber.Commands)
            await HandleCommandMessage(subscriber.ChatId, subscriberCommand.CommandName);
    }

    /// <summary>
    ///     Handling user`s commands
    /// </summary>
    /// <param name="chatId">User chat id</param>
    /// <param name="userMessage">Command sent by user</param>
    /// <returns>Command for user`s request</returns>
    private Task HandleCommandMessage(long chatId, string userMessage)
    {
        return userMessage switch
        {
            BotCommands.StartCommand => HandleErrorMessage(chatId),
            BotCommands.HelpCommand => HandleErrorMessage(chatId),
            _ when userMessage.StartsWith(BotCommands.WeatherForecastCommand) => HandleWeatherForecastMessageAsync(
                chatId, userMessage),
            _ when userMessage.StartsWith(BotCommands.WeatherCommand) => HandleWeatherMessageAsync(chatId, userMessage),
            _ when userMessage.StartsWith(BotCommands.AlertsMapStickerCommand) => HandleAlertsStickerInfo(chatId),
            _ when userMessage.StartsWith(BotCommands.AlertsMapCommand) => HandleAlertsPhotoInfo(chatId),
            _ when userMessage.StartsWith(BotCommands.AlertsLostCommand) => HandleRussianInvasionInfo(chatId),
            _ when userMessage.StartsWith(BotCommands.GetListOfSubscriptionsCommand) =>
                HandleSubscriptionListInfoAsync(chatId),
            _ when userMessage.StartsWith(BotCommands.SubscribeCommand) => HandleSubscribeMessageAsync(chatId,
                userMessage),
            _ when userMessage.StartsWith(BotCommands.UnsubscribeCommand) => HandleUnSubscribeMessageAsync(chatId,
                userMessage),
            _ when userMessage.StartsWith(BotCommands.Reminder) => HandleReminderMessageSetupAsync(chatId,
                userMessage),
            _ => Task.CompletedTask
        };
    }

    /// <summary>
    ///     Handling subscription string results
    /// </summary>
    /// <param name="userMessage">Message sent by user</param>
    /// <returns>Command for editing db</returns>
    private static string HandleSubscriptionMessage(string userMessage)
    {
        return userMessage.Trim().Split(' ', 2) switch
        {
            _ when userMessage.Equals(BotCommands.SubscribeOnAlertsLostCommand) ||
                   userMessage.Equals(BotCommands.UnsubscribeFromAlertsLostCommand) => BotCommands.AlertsLostCommand,
            [{ } userCommand, { } userCityName] when userCommand.StartsWith(BotCommands
                                                         .SubscribeOnWeatherForecastCommand) ||
                                                     userCommand.StartsWith(BotCommands
                                                         .UnsubscribeFromWeatherForecastCommand)
                => BotCommands.WeatherForecastCommand + " " + userCityName,
            _ => string.Empty
        };
    }

    /// <summary>
    ///     Handling receiving subscriber commands list
    /// </summary>
    /// <param name="chatId">User chat id</param>
    /// <returns>List of user`s commands</returns>
    private async Task HandleSubscriptionListInfoAsync(long chatId)
    {
        var subscriber = await _subscriberRepository.FindSubscriberAsync(chatId);
        var message = "You are not subscribed to any services yet!";

        if (subscriber is not null)
            message = "Your subscription list:\n" +
                      string.Join("\n", subscriber.Commands.Select(command => $"{command.CommandName}"));

        await HandleTextMessageAsync(chatId, $"`{message}`");
    }

    /// <summary>
    ///     Handling user`s commands
    /// </summary>
    /// <param name="chatId">User chat id</param>
    /// <param name="userMessage">Command sent by user</param>
    /// <returns>Message with subscription status</returns>
    public async Task HandleSubscribeMessageAsync(long chatId, string userMessage)
    {
        var command = HandleSubscriptionMessage(userMessage);

        if (string.IsNullOrEmpty(command))
        {
            await HandleErrorMessage(chatId);
            return;
        }

        await HandleTextMessageAsync(chatId,
            $"`{GenerateMessageForSubscriptionResult(
                await _subscriberRepository.AddSubscriberAsync(new Subscriber { ChatId = chatId }, command))}`");
    }

    /// <summary>
    ///     Handling user`s commands
    /// </summary>
    /// <param name="chatId">User chat id</param>
    /// <param name="userMessage">Command sent by user</param>
    /// <returns>Message with subscription status</returns>
    public async Task HandleUnSubscribeMessageAsync(long chatId, string userMessage)
    {
        var command = HandleSubscriptionMessage(userMessage);

        if (string.IsNullOrEmpty(command))
        {
            await HandleErrorMessage(chatId);
            return;
        }

        await HandleTextMessageAsync(chatId,
            $"`{GenerateMessageForSubscriptionResult(
                await _subscriberRepository.RemoveCommandFromSubscriberAsync(chatId, command))}`");
    }

    /// <summary>
    ///     Generating message response for user on result of affected rows in table
    /// </summary>
    /// <param name="affectedRows">Amount of affected rows</param>
    /// <returns>String with message for user</returns>
    public static string GenerateMessageForSubscriptionResult(int affectedRows)
    {
        return affectedRows == 0 ? "Operation unsuccessful!" : "Operation successful!";
    }

    /// <summary>
    ///     Handling /weather [city_name] message
    /// </summary>
    /// <param name="chatId">User chat id</param>
    /// <param name="userMessageText">Message sent by user</param>
    /// <returns>Sent weather to the user</returns>
    private async Task HandleWeatherMessageAsync(long chatId, string userMessageText)
    {
        var weatherResponseForUser = await WeatherHandler.SendWeatherByUserMessageAsync(userMessageText);
        var errorMessage = weatherResponseForUser.ErrorMessage;

        if (!string.IsNullOrEmpty(errorMessage))
        {
            await HandleTextMessageAsync(chatId, $"`{errorMessage}`");
            return;
        }

        await HandlePhotoMessageAsync(chatId, WeatherImageGenerator.GenerateCurrentWeatherImage(weatherResponseForUser),
            $"""
             `Current weather in {weatherResponseForUser.CityName} is {weatherResponseForUser.Temperature} °C.
             Feels like {weatherResponseForUser.FeelsLike} °C. {weatherResponseForUser.TypeOfWeather}.`
             """);
    }


    /// <summary>
    ///     Handling /remind [time as hours:minutes] [text] message setup
    /// </summary>
    /// <param name="chatId">User chat id</param>
    /// <param name="userMessageText">Message sent by user</param>
    /// <returns>Sent weather to the user</returns>
    private async Task HandleReminderMessageSetupAsync(long chatId, string userMessageText)
    {
        try
        {
            var parts = userMessageText.Split(Separators, StringSplitOptions.RemoveEmptyEntries);

            var time = DateTime.ParseExact(parts[1] + ' ' + parts[2], "d/M H:mm", CultureInfo.InvariantCulture);
            
            var offset = TimeHelper.CalculateOffset(time);
            
            if (offset.TotalSeconds < 0)
            {
                await HandleTextMessageAsync(chatId, "Time cannot be in the past");
                return;
            }
            
            var text = string.Join(" ", parts.Skip(3));
            text = WebUtility.UrlEncode(text);
            
            Task.Run(() => HandleReminderMessageAsync(chatId, text, offset));
        }
        catch
        {
            await HandleErrorMessage(chatId);
        }
    }

    /// <summary>
    ///     Handling /remind [time as hours:minutes] [text] message
    /// </summary>
    /// <param name="chatId">User chat id</param>
    /// <param name="userMessageText">Message sent by user</param>
    /// <param name="offset">Offset</param>
    /// <returns>Sent weather to the user</returns>
    public async Task HandleReminderMessageAsync(long chatId, string userMessageText,
        TimeSpan offset)
    {
        try
        {
            userMessageText = WebUtility.UrlDecode(userMessageText);
            var doesNeedDots = userMessageText.Length > 30;
            var shortenMessage = userMessageText[..Math.Min(userMessageText.Length, 30)];

            if(doesNeedDots)
                shortenMessage += @"\.\.\.";
            
            var notificationText = $"""
                                    {shortenMessage}
                                    {offset.Hours:00}:{offset.Minutes:00}
                                    """;

            var messageId = await HandleTextMessageAsync(chatId, notificationText, ParseMode.Html);

            shortenMessage = shortenMessage.Replace(@"\.\.\.", "...");
            
            await _botClient.PinChatMessageAsync(chatId, messageId);

            while (offset.Hours > 0 || offset.Minutes > 0)
            {
                notificationText = $"""
                                    {shortenMessage}
                                    {offset.Hours:00}:{offset.Minutes:00}
                                    """;

                try
                {
                    await _botClient.EditMessageTextAsync(chatId, messageId, notificationText);
                }
                catch
                {
                    // ignored
                }

                await Task.Delay(60000);
                offset += TimeSpan.FromMinutes(-1);
            }

            await _botClient.DeleteMessageAsync(chatId, messageId);

            await HandleTextMessageAsync(chatId, userMessageText);
        }
        catch
        {
            // ignored
        }
    }

    /// <summary>
    ///     Handling /weather_forecast [city_name] message
    /// </summary>
    /// <param name="chatId">User chat id</param>
    /// <param name="userMessageText">Message sent by user</param>
    /// <returns>Sent weather forecast to the user</returns>
    private async Task HandleWeatherForecastMessageAsync(long chatId, string userMessageText)
    {
        var weatherForecastResult = await WeatherHandler.SendWeatherForecastByUserMessageAsync(userMessageText);
        var errorMessage = weatherForecastResult.ErrorMessage;

        if (!string.IsNullOrEmpty(errorMessage))
        {
            await HandleTextMessageAsync(chatId, $"`{errorMessage}`");

            return;
        }

        await HandlePhotoMessageAsync(chatId, WeatherImageGenerator.GenerateWeatherForecastImage(
                weatherForecastResult),
            $"`Current weather in {weatherForecastResult.WeatherForecastCity.CityName} for next 24 hours:\n\n"
            + string.Join("\n\n", weatherForecastResult.WeatherForecastHoursData.Select(weatherData =>
                $"""
                 Time: {weatherData.Date[..^3]}:
                 Temperature: {weatherData.WeatherForecastTemperatureData.Temperature:N2} °C.
                 Feels like {weatherData.WeatherForecastTemperatureData.FeelsLike:N2} °C.
                 Humidity {weatherData.WeatherForecastTemperatureData.Humidity}%.
                 {weatherData.WeatherForecastCurrentWeather.First().TypeOfWeather}.
                 """)) + "`");
    }

    /// <summary>
    ///     Handling user location message
    /// </summary>
    /// <param name="chatId">>User chat id</param>
    /// <param name="userLocation">Location sent by user</param>
    /// <returns>Task with weather location</returns>
    private async Task HandleLocationMessageAsync(long chatId, Location userLocation)
    {
        var weatherResponseForUser = await WeatherHandler.SendWeatherByUserLocationAsync(userLocation);

        await HandleTextMessageAsync(chatId,
            $"""
             `Current weather in {weatherResponseForUser.CityName} is {weatherResponseForUser.Temperature:N2} °C.
             Feels like {weatherResponseForUser.FeelsLike:N2} °C. {weatherResponseForUser.TypeOfWeather}.`
             """);
    }

    /// <summary>
    ///     Handling error message type
    /// </summary>
    /// <param name="chatId">User chat id</param>
    /// <returns>Task with error message</returns>
    private Task HandleErrorMessage(long chatId)
    {
        return HandleTextMessageAsync(chatId,
            $"""
             Hello\!
             To receive weather by city name send me: `{BotCommands.WeatherCommand}` \[city\_name\]\!
             To receive weather forecast by city name send me: `{BotCommands.WeatherForecastCommand}` \[city\_name\]\!
             Or just send me your location for receiving current weather\!
             For map of alerts use `{BotCommands.AlertsMapCommand}`\!
             To see russian losses use `{BotCommands.AlertsLostCommand}`\!
             For subscribing on `{BotCommands.AlertsLostCommand}` command `{BotCommands.SubscribeOnAlertsLostCommand}`\!
             For unsubscribing from `{BotCommands.AlertsLostCommand}` command `{BotCommands.UnsubscribeFromAlertsLostCommand}`\!
             For subscribing on `{BotCommands.WeatherForecastCommand}` command `{BotCommands.SubscribeOnWeatherForecastCommand}` \[city\_name\]\!
             For unsubscribing from `{BotCommands.WeatherForecastCommand}` command `{BotCommands.UnsubscribeFromWeatherForecastCommand}` \[city\_name\]\!
             For receiving list of all your subscriptions send me `{BotCommands.GetListOfSubscriptionsCommand}`\!
             Setting up reminder: `{BotCommands.Reminder}` \[time as day:\month hours\:minutes\] \[text\]
             My GitHub: `https://github.com/ShindQQ/WeatherAlertsBot`
             """);
    }

    /// <summary>
    ///     Receiving info about liquidations in russian invasion
    /// </summary>
    /// <param name="chatId">User chat id</param>
    private async Task HandleRussianInvasionInfo(long chatId)
    {
        await HandleTextMessageAsync(chatId,
            (await ApisRequestsHandler.GetResponseFromApiAsync<RussianInvasion>(ApIsLinks.RussianWarshipUrl))
            !.RussianWarshipInfo.ToString());
    }

    /// <summary>
    ///     Receiving info about alerts in Ukraine regions
    /// </summary>
    /// <param name="chatId">User chat id</param>
    /// <returns>Map with alerts and list of regions to the user</returns>
    private async Task HandleAlertsPhotoInfo(long chatId)
    {
        var regions = await ApisRequestsHandler.GetResponseForAlertsCachedAsync();

        await HandlePhotoMessageAsync(chatId, AlarmsMapGenerator.DrawAlertsMap(regions, false),
            "`Current alerts in Ukraine:\n" + string.Join("\n",
                regions.Where(region => region.Value.Enabled)
                    .Select(region => $"🚨 {region.Key.Trim('\'')};")) + "`");
    }

    /// <summary>
    ///     Receiving info about alerts in Ukraine regions with sticker
    /// </summary>
    /// <param name="chatId">User chat id</param>
    /// <returns>Map with alerts and list of regions to the user</returns>
    private async Task HandleAlertsStickerInfo(long chatId)
    {
        var regions = await ApisRequestsHandler.GetResponseForAlertsCachedAsync();

        await HandleStickerAsync(chatId, AlarmsMapGenerator.DrawAlertsMap(regions));
    }

    /// <summary>
    ///     Handling sending photo message with caption
    /// </summary>
    /// <param name="chatId">Id og the chat to send to</param>
    /// <param name="bytes">Bytes array (photo)</param>
    /// <param name="messageForUser">Message which will be sent or not(caption)</param>
    /// <returns>Sent photo message with caption or not</returns>
    private async Task HandlePhotoMessageAsync(long chatId, byte[] bytes, string? messageForUser)
    {
        try
        {
            await _botClient.SendPhotoAsync(chatId, InputFile.FromStream(new MemoryStream(bytes)),
                caption: messageForUser,
                parseMode: ParseMode.MarkdownV2,
                cancellationToken: _cancellationTokenSource.Token);
        }
        catch (ApiRequestException)
        {
        }
    }

    /// <summary>
    ///     Handling sending text messages because of Telegram.Bot.Exceptions
    /// </summary>
    /// <param name="chatId">Id of the chat to send to</param>
    /// <param name="messageForUser">Message which will be sent</param>
    /// <param name="parseMode">Parse mode</param>
    /// <returns>Sent text message</returns>
    public async Task<int> HandleTextMessageAsync(long chatId, string messageForUser,
        ParseMode parseMode = ParseMode.MarkdownV2)
    {
        try
        {
            var message = await _botClient.SendTextMessageAsync(chatId, messageForUser,
                parseMode: parseMode,
                cancellationToken: _cancellationTokenSource.Token);

            return message.MessageId;
        }
        catch (ApiRequestException ex)
        {
            return -1;
        }
    }

    /// <summary>
    ///     Handling sending sticker messages because of Telegram.Bot.Exceptions
    /// </summary>
    /// <param name="chatId">Id of the chat to send to</param>
    /// <param name="bytes">Bytes array (photo)</param>
    /// <returns>Sent text message</returns>
    private async Task HandleStickerAsync(long chatId, byte[] bytes)
    {
        try
        {
            await _botClient.SendStickerAsync(chatId, InputFile.FromStream(new MemoryStream(bytes)),
                cancellationToken: _cancellationTokenSource.Token);
        }
        catch (ApiRequestException)
        {
        }
    }
}