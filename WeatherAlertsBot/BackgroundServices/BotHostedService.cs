using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using WeatherAlertsBot.Configuration;

namespace WeatherAlertsBot.BackgroundServices;

public sealed class BotHostedService : BackgroundService
{
    private ITelegramBotClient _botClient = new TelegramBotClient(BotConfiguration.BotAccessToken);
    private readonly CancellationTokenSource _stoppingCts =
                                                   new CancellationTokenSource();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(2000);
        }
    }
}
