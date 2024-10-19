using Timer = System.Timers.Timer;

namespace WeatherAlertsBot.Розширення;

public static class РозширенняТаймеру
{
    public static Task ЗапуститиІзФункцієюАсінх(this Timer таймер,
        Func<CancellationToken, Task> функція,
        CancellationToken жетонВідміни)
    {
        таймер.Elapsed += async (_, _) => await функція(жетонВідміни);
        таймер.Start();

        if (жетонВідміни.IsCancellationRequested)
        {
            таймер.Stop();
            таймер.Dispose();
        }

        return Task.CompletedTask;
    }
}