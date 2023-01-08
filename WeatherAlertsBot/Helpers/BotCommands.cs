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
    public static readonly string WeatherCommand = "/weather";

    /// <summary>
    ///     String for weather_forecast command
    /// </summary>
    public static readonly string WeatherForecastCommand = "/weather_forecast";

    /// <summary>
    ///     String for alerts_map command
    /// </summary>
    public static readonly string AlertsMapCommand = "/alerts_map";

    /// <summary>
    ///     String for alerts_lost command
    /// </summary>
    public static readonly string AlertsLostCommand = "/alerts_lost";

    /// <summary>
    ///     Subscription command 
    /// </summary>
    public static readonly string SubscribeCommand = "/subscribe_on";

    /// <summary>
    ///     Unsubscription command 
    /// </summary>
    public static readonly string UnsubscribeCommand = "/unsubscribe_from";

    /// <summary>
    ///     Subscription on alerts_lost command
    /// </summary>
    public static readonly string SubscribeOnAlertsLostCommand = "/subscribe_on_alerts_lost";

    /// <summary>
    ///     Subscription on weather_forecast command
    /// </summary>
    public static readonly string SubscribeOnWeatherForecastCommand = "/subscribe_on_weather_forecast";

    /// <summary>
    ///     Unsubscription from alerts_lost command
    /// </summary>
    public static readonly string UnsubscribeFromAlertsLostCommand = "/unsubscribe_from_alerts_lost";

    /// <summary>
    ///     Unsubscription from weather_forecast command
    /// </summary>
    public static readonly string UnsubscribeFromWeatherForecastCommand = "/unsubscribe_from_weather_forecast";

    /// <summary>
    ///     Receiving list of subscriptions for selected user command
    /// </summary>
    public static readonly string GetListOfSubscriptionsCommand = "/get_list_of_subscriptions";

}
