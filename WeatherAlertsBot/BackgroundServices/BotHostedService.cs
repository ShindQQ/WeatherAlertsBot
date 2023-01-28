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
    ///     Periodic timer for delay
    /// </summary>
    private readonly PeriodicTimer _periodicTimer = new(TimeSpan.FromDays(1));

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
        while (await _periodicTimer.WaitForNextTickAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
        {
            await _updateHandler.HandleSubscribersNotificationsAsync();
        }
    }
}
