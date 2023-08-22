using Quartz;
using WeatherAlertsBot.TelegramBotHandlers;

namespace WeatherAlertsBot.BackgroundServices;

/// <summary>
///     Backgorund job for sending notifications to subscribed users
/// </summary>
public sealed class NotifySubscribersJob : IJob
{
    /// <summary>
    ///     Handler for sending messages to users
    /// </summary>
    private readonly IUpdateHandler _updateHandler;

    /// <summary>
    ///     Constructor for di
    /// </summary>
    /// <param name="updateHandler">Handler of messages</param>
    public NotifySubscribersJob(IUpdateHandler updateHandler)
    {
        _updateHandler = updateHandler;
    }

    /// <summary>
    ///     Execution of background subscriber job
    /// </summary>
    /// <returns></returns>
    public async Task Execute(IJobExecutionContext context)
    {
        await _updateHandler.HandleSubscribersNotificationsAsync();
    }
}
