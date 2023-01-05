using Microsoft.Extensions.Hosting;
using System.Threading;
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
            await _botClient.SendTextMessageAsync(405504138, "/alerts_map");
            await Task.Delay(2000);
        }
    }
}
