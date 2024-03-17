namespace WeatherAlertsBot.Helpers;

/// <summary>
///     Static class with list of available commands
/// </summary>
public static class BotCommands
{
    /// <summary>
    ///     String for start command
    /// </summary>
    public const string StartCommand = "/start";

    /// <summary>
    ///     String for help command
    /// </summary>
    public const string HelpCommand = "/help";

    /// <summary>
    ///     String for weather command
    /// </summary>
    public const string WeatherCommand = "/weather";

    /// <summary>
    ///     String for wf command
    /// </summary>
    public const string WeatherForecastCommand = "/wf";

    /// <summary>
    ///     String for al_map command
    /// </summary>
    public const string AlertsMapCommand = "/al_map";

    /// <summary>
    ///     String for al_map_stick command
    /// </summary>
    public const string AlertsMapStickerCommand = "/al_map_stick";

    /// <summary>
    ///     String for al_lost command
    /// </summary>
    public const string AlertsLostCommand = "/al_lost";

    /// <summary>
    ///     Subscription command
    /// </summary>
    public const string SubscribeCommand = "/sub";

    /// <summary>
    ///     UnSubscription command
    /// </summary>
    public const string UnsubscribeCommand = "/unsub";

    /// <summary>
    ///     Subscription al_lost command
    /// </summary>
    public const string SubscribeOnAlertsLostCommand = "/sub_al_lost";

    /// <summary>
    ///     Subscription on wf command
    /// </summary>
    public const string SubscribeOnWeatherForecastCommand = "/sub_wf";

    /// <summary>
    ///     UnSubscription from al_lost command
    /// </summary>
    public const string UnsubscribeFromAlertsLostCommand = "/unsub_al_lost";

    /// <summary>
    ///     UnSubscription from wf command
    /// </summary>
    public const string UnsubscribeFromWeatherForecastCommand = "/unsub_wf";

    /// <summary>
    ///     Receiving list of subscriptions for selected user command
    /// </summary>
    public const string GetListOfSubscriptionsCommand = "/sub_list";

    /// <summary>
    ///     Creating reminder command
    /// </summary>
    public const string Reminder = "/remind";
}