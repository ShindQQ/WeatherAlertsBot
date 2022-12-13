using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
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
    var userMessageText = update.Message.Text;

    string message = "Something went wrong";

    if (userMessageText.ToLower().Contains("weather")) message = "I`ll send you the weather!";
    else if (userMessageText.ToLower().Contains("something")) message = "something";


    WeatherHandler weatherHandler = new();
    var result = await weatherHandler.GetCurrentWeatherByCoordinatesAsync(44.34, 10.99);
    //var result = await weatherHandler.GetLattitudeAndLongitudeByCityNameAsync("London");

    await botClient.SendTextMessageAsync(update.Message.Chat.Id, result.Name, cancellationToken: cancellationToken);
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