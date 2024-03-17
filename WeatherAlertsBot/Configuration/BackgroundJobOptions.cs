namespace WeatherAlertsBot.Configuration;

public sealed record BackgroundJobOptions
{
    public const string ConfigName = "BackgroundJob";

    public int CheckTimerInMinutes { get; init; }

    public int TimerIntervalInMilliseconds => CheckTimerInMinutes * 60 * 1000;

    public int ExecuteAt { get; init; }
}