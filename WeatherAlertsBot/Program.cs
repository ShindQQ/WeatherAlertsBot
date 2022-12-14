using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using WeatherAlertsBot.OpenWeatherAPI;

var configuration = new ConfigurationBuilder()
                 .AddJsonFile($"appsettings.json", true, true).Build();

var botClient = new TelegramBotClient(configuration["BotAccessToken"]);

using CancellationTokenSource cansellationTokenSource = new();

ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>()
};

botClient.StartReceiving(
    HandleUpdateAsync,
    HandlePollingErrorAsync,
    receiverOptions,
    cansellationTokenSource.Token
    );

Console.ReadLine();

cansellationTokenSource.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    var weatherHandler = new WeatherHandler();
    if (update.Message != null)
    {
        var userMessageText = update.Message.Text;
        if (userMessageText.ToLower().Contains("/start"))
        {
            await botClient.SendTextMessageAsync(update.Message.Chat.Id,
                @"`Hello!\nTo receive weather by city name send me the message in format: weather [city_name]`",
                ParseMode.MarkdownV2, cancellationToken: cancellationToken);
        }
        else if (userMessageText.ToLower().Contains("weather"))
        {
            var weatherResponseForUser = await weatherHandler.CreateResponseForUser(userMessageText);
            var errorMessage = weatherResponseForUser.ErrorMessage;

            if (string.IsNullOrEmpty(errorMessage))
            {
                var location = await botClient.SendLocationAsync(update.Message.Chat.Id,
                   weatherResponseForUser.Lattitude, weatherResponseForUser.Longitude,
                   cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(location.Chat.Id,
                   @$"`Current weather in {weatherResponseForUser.CityName} is {weatherResponseForUser.Temperature} °C, 
               feels like {weatherResponseForUser.FeelsLike} °C.`",
                   ParseMode.MarkdownV2, cancellationToken: cancellationToken, replyToMessageId: location.MessageId);
                return;
            }

            await botClient.SendTextMessageAsync(update.Message.Chat.Id,
                   errorMessage, ParseMode.MarkdownV2, cancellationToken: cancellationToken);
        };
    }
}

async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    await Task.CompletedTask;
}