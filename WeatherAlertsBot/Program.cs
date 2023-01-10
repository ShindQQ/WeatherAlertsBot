using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using WeatherAlertsBot.BackgroundServices;
using WeatherAlertsBot.Configuration;
using WeatherAlertsBot.TelegramBotHandlers;

var botClient = new TelegramBotClient(BotConfiguration.BotAccessToken);

using CancellationTokenSource cancellationTokenSource = new();

botClient.StartReceiving(
    HandleUpdateAsync,
    HandlePollingErrorAsync,
    new(),
    cancellationTokenSource.Token
    );

 await Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
{
    services.AddHostedService<BotHostedService>();
}).StartAsync();

// Wait for eternity
await Task.Delay(-1);

cancellationTokenSource.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    UpdateHandler updateHandler = new(botClient, update, cancellationToken);

    await updateHandler.HandleMessageAsync();
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