using System.Data;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using WeatherAlertsBot.Commands;
using WeatherAlertsBot.DAL.Entities;
using WeatherAlertsBot.Helpers;
using WeatherAlertsBot.OpenWeatherAPI;
using WeatherAlertsBot.Requesthandlers;
using WeatherAlertsBot.RussianWarship;
using WeatherAlertsBot.UserServices;

namespace WeatherAlertsBot.TelegramBotHandlers;

/// <summary>
///     Telegram Bot Update Handler
/// </summary>
public sealed class UpdateHandler
{
    /// <summary>
    ///     A client interface to use Telegram Bot API
    /// </summary>
    private readonly ITelegramBotClient _botClient;

    /// <summary>
    ///     Incoming update from user
    /// </summary>
    private readonly Update _update;

    /// <summary>
    ///     Cancellation Token
    /// </summary>
    private readonly CancellationToken _cancellationToken;

    /// <summary>
    ///     Subscriber survice to work with db
    /// </summary>
    private readonly SubscriberService _subscriberService = new();

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="telegramBotClient">A client interface to use Telegram Bot API</param>
    /// <param name="update">Incoming update from user</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    public UpdateHandler(ITelegramBotClient telegramBotClient, Update update, CancellationToken cancellationToken)
    {
        _botClient = telegramBotClient;
        _update = update;
        _cancellationToken = cancellationToken;
    }

    /// <summary>
    ///     Constructor without update
    /// </summary>
    /// <param name="telegramBotClient">A client interface to use Telegram Bot API</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    public UpdateHandler(ITelegramBotClient telegramBotClient, CancellationToken cancellationToken)
    {
        _botClient = telegramBotClient;
        _update = new();
        _cancellationToken = cancellationToken;
    }

    /// <summary>
    ///     Handling user message
    /// </summary>
    /// <returns>Task</returns>
    public async Task HandleMessageAsync()
    {
        if (_update.Message != null)
        {
            var userMessage = _update.Message;
            var userMessageText = userMessage.Text;

            if (userMessageText != null)
            {
                await HandleCommandMessage(_update.Message!.Chat.Id, userMessageText);
            }

            var userMessageLocation = userMessage.Location;

            if (userMessageLocation != null)
            {
                await HandleLocationMessageAsync(userMessageLocation);
            }

            if (userMessageText == null && userMessageLocation == null)
            {
                await HandleErrorMessage(_update.Message!.Chat.Id);
            }
        }
    }

    /// <summary>
    ///     Handling sending notifications to users
    /// </summary>
    /// <returns>Task</returns>
    public async Task HandleSubscribersNotificationsAsync()
    {
        var subscribers = await _subscriberService.GetSubscribersAsync();

        subscribers.ForEach(subscriber => subscriber.Commands
            .ForEach(async command => await HandleCommandMessage(subscriber.ChatId, command.CommandName)));
    }

    /// <summary>
    ///     Handling user`s commands
    /// </summary>
    /// <param name="chatId">User chat id</param>
    /// <param name="userMessage">Command sent by user</param>
    /// <returns>Command for user`s request</returns>
    public Task HandleCommandMessage(long chatId, string userMessage)
    {
        Task command = userMessage switch
        {
            BotCommands.StartCommand => HandleStartMessage(chatId),
            _ when userMessage.StartsWith(BotCommands.WeatherForecastCommand) => HandleWeatherForecastMessageAsync(chatId, userMessage),
            _ when userMessage.StartsWith(BotCommands.WeatherCommand) => HandleWeatherMessageAsync(chatId, userMessage),
            _ when userMessage.StartsWith(BotCommands.AlertsMapCommand) => HandleAlertsInfo(chatId),
            _ when userMessage.StartsWith(BotCommands.AlertsLostCommand) => HandleRussianInvasionInfo(chatId),
            _ when userMessage.StartsWith(BotCommands.GetListOfSubscriptionsCommand) => HandleSubscriptionListInfoAsync(chatId),
            _ when userMessage.StartsWith(BotCommands.SubscribeCommand) => HandleSubscribeMessageAsync(chatId, userMessage),
            _ when userMessage.StartsWith(BotCommands.UnsubscribeCommand) => HandleUnSubscribeMessageAsync(chatId, userMessage),
            _ => HandleErrorMessage(chatId)
        };

        return command;
    }

    /// <summary>
    ///     Handling subscription string results
    /// </summary>
    /// <param name="userMessage">Message sent by user</param>
    /// <returns>Command for editing db</returns>
    private static string HandleSubscriptionMessage(string userMessage)
    {
        var splittedMessage = userMessage.Trim().Split(' ', 2);

        return splittedMessage switch
        {
            _ when userMessage.Equals(BotCommands.SubscribeOnAlertsLostCommand) ||
                userMessage.Equals(BotCommands.UnsubscribeFromAlertsLostCommand) => BotCommands.AlertsLostCommand,
            [string userCommand, string userCityName] when userCommand.StartsWith(BotCommands.SubscribeOnWeatherForecastCommand) ||
                userCommand.StartsWith(BotCommands.UnsubscribeFromWeatherForecastCommand)
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
        var subscriber = await _subscriberService.FindSubscriberAsync(chatId);
        var message = "You aren`t subscribed to any services yet!";

        if (subscriber != null)
        {
            message = $"Your subscription list:\n" +
                string.Join("\n", subscriber.Commands.Select(command => $"{command.CommandName}"));
        }

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
            await _subscriberService.AddSubscriberAsync(new Subscriber { ChatId = chatId }, command))}`");
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
            await _subscriberService.RemoveCommandFromSubscriberAsync(chatId, command))}`");
    }

    /// <summary>
    ///     Generating message response for user on result of affected rows in table
    /// </summary>
    /// <param name="affectedRows">Ammount of affected rows</param>
    /// <returns>String with message for user</returns>
    public string GenerateMessageForSubscriptionResult(int affectedRows)
        => affectedRows == 0 ? "Operation unsuccessful!" : "Operation successful!";

    /// <summary>
    ///     Handling /start message
    /// </summary>
    /// <param name="chatId">User chat id</param>
    /// <returns>Returns user start message</returns>
    private Task HandleStartMessage(long chatId)
    {
        return HandleErrorMessage(chatId);
    }

    /// <summary>
    ///     Handling /weather [city_name] message
    /// </summary>
    /// <param name="chatId">User chat id</param>
    /// <param name="userMessageText">Message sent by user</param>
    /// <returns>True if user`s message starts with /weather and there were no troubles with request, false if there was troubleshooting</returns>
    private async Task HandleWeatherMessageAsync(long chatId, string userMessageText)
    {
        var weatherResponseForUser = await WeatherHandler.SendWeatherByUserMessageAsync(userMessageText);
        var errorMessage = weatherResponseForUser.ErrorMessage;

        if (!string.IsNullOrEmpty(errorMessage))
        {
            await HandleTextMessageAsync(chatId, $"`{errorMessage}`");

            return;
        }

        await HandleTextMessageAsync(chatId, $"""
                `Current weather in {weatherResponseForUser.CityName} is {weatherResponseForUser.Temperature:N2} °C.
                Feels like {weatherResponseForUser.FeelsLike:N2} °C. Type of weather: {weatherResponseForUser.TypeOfWeather}.`
                """);
    }

    /// <summary>
    ///     Handling /weather_forecast [city_name] message
    /// </summary>
    /// <param name="chatId">User chat id</param>
    /// <param name="userMessageText">Message sent by user</param>
    /// <returns>True if user`s message starts with /weather_forecast and there were no troubles with request, false if there was troubleshooting</returns>
    private async Task HandleWeatherForecastMessageAsync(long chatId, string userMessageText)
    {
        var weatherForecastResult = await WeatherHandler.SendWeatherForecastByUserMessageAsync(userMessageText);
        var errorMessage = weatherForecastResult.ErrorMessage;

        if (!string.IsNullOrEmpty(errorMessage))
        {
            await HandleTextMessageAsync(chatId, $"`{errorMessage}`");

            return;
        }

        await HandleTextMessageAsync(chatId,
            $"`Current weather in {weatherForecastResult.WeatherForecastCity.CityName} for next 24 hours:\n\n"
            + string.Join("\n\n", weatherForecastResult.WeatherForecastHoursData.Select(weatherData =>
            $"""
            Time: {weatherData.Date[^8..]}: 
            Temperature: {weatherData.WeatherForecastTemperatureData.Temperature:N2} °C.
            Feels like {weatherData.WeatherForecastTemperatureData.FeelsLike:N2} °C.
            Humidity {weatherData.WeatherForecastTemperatureData.Humidity}. 
            Type of weather: {weatherData.WeatherForecastCurrentWeather.First().TypeOfWeather}.
            """)) + "`");
    }

    /// <summary>
    ///     Handling user location message
    /// </summary>
    /// <returns>Task with weather location</returns>
    private async Task HandleLocationMessageAsync(Location userLocation)
    {
        var weatherResponseForUser = await WeatherHandler.SendWeatherByUserLocationAsync(userLocation);

        await HandleTextMessageAsync(_update.Message!.Chat.Id,
           $"""
           `Current weather in {weatherResponseForUser.CityName} is {weatherResponseForUser.Temperature:N2} °C.
           Feels like {weatherResponseForUser.FeelsLike:N2} °C. Type of weather: {weatherResponseForUser.TypeOfWeather}.`
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
            `Hello!
            To receive weather by city name send me the message in format: {BotCommands.WeatherCommand} [city_name]!
            To receive weather forecast by city name send me the message in format: {BotCommands.WeatherForecastCommand} [city_name]!
            Or just send me your location for receiving current weather!
            For map of alerts use {BotCommands.AlertsLostCommand}!
            To see russian losses use {BotCommands.AlertsMapCommand}!
            For subscribing on alerts_lost command {BotCommands.SubscribeOnAlertsLostCommand}!
            For unsubscribing from alerts_lost command {BotCommands.UnsubscribeFromAlertsLostCommand}!
            For subscribing on weather_forecast command {BotCommands.SubscribeOnWeatherForecastCommand} [city_name]!
            For unsubscribing from weather_forecast command {BotCommands.UnsubscribeFromWeatherForecastCommand} [city_name]!
            For receiving list of all your subscriptions send me {BotCommands.GetListOfSubscriptionsCommand}!`
            """);
    }

    /// <summary>
    ///     Receiving info about liquidations in russian invasion
    /// </summary>
    /// <param name="chatId">User chat id</param>
    /// no troubles with request, false if there was troubleshooting</returns>
    private async Task HandleRussianInvasionInfo(long chatId)
    {
        var russianInvasion = (await APIsRequestsHandler.GetResponseFromAPIAsync<RussianInvasion>(APIsLinks.RussianWarshipUrl))!.RussianWarshipInfo;

        await HandleTextMessageAsync(chatId,
            russianInvasion.ToString());
    }

    /// <summary>
    ///     Receiving info about alerts in Ukraine regions
    /// </summary>
    /// <param name="chatId">User chat id</param>
    /// <returns>True if user`s message equals with /alerts_map and there were 
    /// no troubles with request, false if there was troubleshooting</returns>
    private async Task HandleAlertsInfo(long chatId)
    {
        string messageForUser = $"`Information about current alerts in Ukraine:\n";

        var regions = await APIsRequestsHandler.GetResponseForAlertsCachedAsync();

        var bytes = AlarmsMapGenerator.DrawAlertsMap(regions);

        try
        {
            await _botClient.SendPhotoAsync(chatId, new InputOnlineFile(new MemoryStream(bytes)),
                messageForUser + string.Join("\n",
                regions.Where(region => region.Value.Enabled)
                .Select(region => $"🚨 {region.Key.Trim('\'')}; Enabled at: " +
                $"{DateTime.Parse(region.Value.EnabledAt):MM/dd/yyyy HH:mm}")) + "`",
                ParseMode.MarkdownV2, cancellationToken: _cancellationToken);
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
    /// <returns>Sent message</returns>
    private async Task HandleTextMessageAsync(long chatId, string messageForUser)
    {
        try
        {
            await _botClient.SendTextMessageAsync(chatId,
                messageForUser,
                ParseMode.MarkdownV2, cancellationToken: _cancellationToken);
        }
        catch (ApiRequestException)
        {
        }
    }
}
