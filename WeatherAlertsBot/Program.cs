using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using WeatherAlertsBot.OpenWeatherAPI;
using WeatherAlertsBot.TelegramBotHandlers;

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

    UpdateHandler updateHandler = new(botClient, update, cancellationToken);

    await updateHandler.HandleMessageTextAsync();
    await updateHandler.HandleMessageLocationAsync();
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