using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using WeatherAlertsBot.Configuration;
using WeatherAlertsBot.Розширення;
using WeatherAlertsBot.Помічники;
using WeatherAlertsBot.ОбробникиТелеграмБота;
using Timer = System.Timers.Timer;

namespace WeatherAlertsBot.ФоновіСервіси;

/// <summary>
///     Background job for sending notifications to subscribed users
/// </summary>
public sealed class ЗадачаПовідомленняПідписників : BackgroundService
{
    private static bool _булаВиконана;
    private static Timer _таймер = null!;
    private readonly ОпціїФоновоїЗадачи _опції;
    private readonly IServiceProvider _постачальникСервісів;

    /// <summary>
    ///     Constructor for di
    /// </summary>
    /// <param name="постачальникСервісів">Service provider</param>
    /// <param name="options">Job options</param>
    public ЗадачаПовідомленняПідписників(
        IServiceProvider постачальникСервісів,
        IOptions<ОпціїФоновоїЗадачи> options)
    {
        _постачальникСервісів = постачальникСервісів;
        _опції = options.Value;
        _таймер = new Timer(_опції.TimerIntervalInMilliseconds);
    }

    /// <summary>
    ///     Execution by timer
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        return _таймер.ЗапуститиІзФункцієюАсінх(HandleSendingAsync, cancellationToken);
    }

    /// <summary>
    ///     Handling of the sending job
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    private async Task HandleSendingAsync(CancellationToken cancellationToken)
    {
        var поточнийУкраїнськийЧас = ПомічникЧасу.ВзятиПоточнийУкраїнськийЧас();

        if (поточнийУкраїнськийЧас.Hour != _опції.ExecuteAt)
        {
            _булаВиконана = false;
            return;
        }

        if (поточнийУкраїнськийЧас.Hour == _опції.ExecuteAt && _булаВиконана)
            return;

        await Виконати();

        GC.Collect();
    }

    /// <summary>
    ///     Execution of background subscriber job
    /// </summary>
    /// <returns>Task</returns>
    private async Task Виконати()
    {
        await using var зона = _постачальникСервісів.CreateAsyncScope();
        var обробникОновлень = зона.ServiceProvider.GetService<ІОбробникОновлення>();

        await обробникОновлень!.ОбробитиПовідомленняПідпісниківАсінх();

        _булаВиконана = true;
    }
}