using Timer = System.Timers.Timer;

namespace WeatherAlertsBot.Extensions;

public static class TimerExtension
{
    public static Task StartWithFunctionAsync(this Timer timer,
        Func<CancellationToken, Task> function,
        CancellationToken cancellationToken)
    {
        timer.Elapsed += async (_, _) => await function(cancellationToken);
        timer.Start();

        if (cancellationToken.IsCancellationRequested)
        {
            timer.Stop();
            timer.Dispose();
        }

        return Task.CompletedTask;
    }
}