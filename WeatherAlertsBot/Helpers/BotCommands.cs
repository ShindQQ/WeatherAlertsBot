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
    ///     String for wf command
    /// </summary>
    public static readonly string WeatherForecastCommand = "/wf";

    /// <summary>
    ///     String for al_map command
    /// </summary>
    public static readonly string AlertsMapCommand = "/al_map";

    /// <summary>
    ///     String for al_lost command
    /// </summary>
    public static readonly string AlertsLostCommand = "/al_lost";

    /// <summary>
    ///     Subscription command 
    /// </summary>
    public static readonly string SubscribeCommand = "/sub";

    /// <summary>
    ///     Unsubscription command 
    /// </summary>
    public static readonly string UnsubscribeCommand = "/unsub";

    /// <summary>
    ///     Subscription al_lost command
    /// </summary>
    public static readonly string SubscribeOnAlertsLostCommand = "/sub_al_lost";

    /// <summary>
    ///     Subscription on wf command
    /// </summary>
    public static readonly string SubscribeOnWeatherForecastCommand = "/sub_wf";

    /// <summary>
    ///     Unsubscription from al_lost command
    /// </summary>
    public static readonly string UnsubscribeFromAlertsLostCommand = "/unsub_al_lost";

    /// <summary>
    ///     Unsubscription from wf command
    /// </summary>
    public static readonly string UnsubscribeFromWeatherForecastCommand = "/unsub_wf";

    /// <summary>
    ///     Receiving list of subscriptions for selected user command
    /// </summary>
    public static readonly string GetListOfSubscriptionsCommand = "/sub_list";

}
