namespace WeatherAlertsBot.Configuration;

public sealed record ОпціїФоновоїЗадачи
{
    public const string НазваКонфіга = "BackgroundJob";

    public int CheckTimerInMinutes { get; init; }

    public int TimerIntervalInMilliseconds => CheckTimerInMinutes * 60 * 1000;

    public int ExecuteAt { get; init; }
}