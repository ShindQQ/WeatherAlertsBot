using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using WeatherAlertsBot.Configuration;

namespace WeatherAlertsBot.BackgroundServices;

/// <summary>
///     Backgorund service for sending notifications to subscribed users
/// </summary>
public sealed class BotHostedService : BackgroundService
{
    /// <summary>
    ///     Telegram bot client
    /// </summary>
    private ITelegramBotClient _botClient = new TelegramBotClient(BotConfiguration.BotAccessToken);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(2000);
        }
    }
}
