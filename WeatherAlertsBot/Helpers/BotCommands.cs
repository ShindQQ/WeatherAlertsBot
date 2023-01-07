namespace WeatherAlertsBot.Commands;

public static class BotCommands
{
    /// <summary>
    ///     String for start command
    /// </summary>
    public const string StartCommand = "/start";

    /// <summary>
    ///     String for weather command
    /// </summary>
    public const string WeatherCommand = "/weather";

    /// <summary>
    ///     String for weather_forecast command
    /// </summary>
    public const string WeatherForecastCommand = "/weather_forecast";

    /// <summary>
    ///     String for alerts_map command
    /// </summary>
    public const string AlertsMapCommand = "/alerts_map";

    /// <summary>
    ///     String for alerts_lost command
    /// </summary>
    public const string AlertsLostCommand = "/alerts_lost";

    /// <summary>
    ///     Subscription on alerts_lost command
    /// </summary>
    public const string SubscribeOnAlertsCommand = "/subscribe_on_alerts_lost";

    /// <summary>
    ///     Subscription on weather_forecast command
    /// </summary>
    public const string SubscribeOnWeatherForecastCommand = "/subscribe_on_weather_forecast";

}
