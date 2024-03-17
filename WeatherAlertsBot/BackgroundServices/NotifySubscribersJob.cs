using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using WeatherAlertsBot.Configuration;
using WeatherAlertsBot.Extensions;
using WeatherAlertsBot.Helpers;
using WeatherAlertsBot.TelegramBotHandlers;
using Timer = System.Timers.Timer;

namespace WeatherAlertsBot.BackgroundServices;

/// <summary>
///     Background job for sending notifications to subscribed users
/// </summary>
public sealed class NotifySubscribersJob : BackgroundService
{
    private static bool _hasBeenExecuted;
    private static Timer _timer = null!;
    private readonly BackgroundJobOptions _options;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    ///     Constructor for di
    /// </summary>
    /// <param name="serviceProvider">Service provider</param>
    /// <param name="options">Job options</param>
    public NotifySubscribersJob(
        IServiceProvider serviceProvider,
        IOptions<BackgroundJobOptions> options)
    {
        _serviceProvider = serviceProvider;
        _options = options.Value;
        _timer = new Timer(_options.TimerIntervalInMilliseconds);
    }

    /// <summary>
    ///     Execution by timer
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        return _timer.StartWithFunctionAsync(HandleSendingAsync, cancellationToken);
    }

    /// <summary>
    ///     Handling of the sending job
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    private async Task HandleSendingAsync(CancellationToken cancellationToken)
    {
        var ukraineCurrentTime = TimeHelper.GetUkraineCurrentTime();

        if (ukraineCurrentTime.Hour != _options.ExecuteAt)
        {
            _hasBeenExecuted = false;
            return;
        }

        if (ukraineCurrentTime.Hour == _options.ExecuteAt && _hasBeenExecuted)
            return;

        await Execute();

        GC.Collect();
    }

    /// <summary>
    ///     Execution of background subscriber job
    /// </summary>
    /// <returns>Task</returns>
    private async Task Execute()
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var updateHandler = scope.ServiceProvider.GetService<IUpdateHandler>();

        await updateHandler!.HandleSubscribersNotificationsAsync();

        _hasBeenExecuted = true;
    }
}