using Microsoft.Extensions.Hosting;
using WeatherAlertsBot.TelegramBotHandlers;

namespace WeatherAlertsBot.BackgroundServices;

/// <summary>
///     Backgorund service for sending notifications to subscribed users
/// </summary>
public sealed class BotHostedService : BackgroundService
{
    /// <summary>
    ///     Handler for sending messages to users
    /// </summary>
    private readonly UpdateHandler _updateHandler;

    /// <summary>
    ///     Constructor for di
    /// </summary>
    /// <param name="updateHandler">Handler of messages</param>
    public BotHostedService(UpdateHandler updateHandler)
    {
        _updateHandler = updateHandler;
    }

    /// <summary>
    ///     Execution of background subscriber service
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            //await _updateHandler.HandleSubscribersNotificationsAsync();

            await Task.Delay(86400000, cancellationToken);
        }
    }
}
