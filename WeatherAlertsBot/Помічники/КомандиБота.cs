namespace WeatherAlertsBot.Помічники;

/// <summary>
///     Static class with list of available commands
/// </summary>
public static class КомандиБота
{
    /// <summary>
    ///     String for start command
    /// </summary>
    public const string КомандаСтарт = "/start";

    /// <summary>
    ///     String for help command
    /// </summary>
    public const string КомандаДопомога = "/help";

    /// <summary>
    ///     String for weather command
    /// </summary>
    public const string КомандаПогода = "/weather";

    /// <summary>
    ///     String for wf command
    /// </summary>
    public const string КомандаПрогнозПогоди = "/wf";

    /// <summary>
    ///     String for al_map command
    /// </summary>
    public const string КомандаКартаТривог = "/al_map";

    /// <summary>
    ///     String for al_map_stick command
    /// </summary>
    public const string КомандаКартаТривогСтікер = "/al_map_stick";

    /// <summary>
    ///     String for al_lost command
    /// </summary>
    public const string КомандаПовідомленняВтрат = "/al_lost";

    /// <summary>
    ///     Subscription command
    /// </summary>
    public const string КомандаПідписатися = "/sub";

    /// <summary>
    ///     UnSubscription command
    /// </summary>
    public const string КомандаВідписатися = "/unsub";

    /// <summary>
    ///     Subscription al_lost command
    /// </summary>
    public const string КомандаПідписатисяНаПовідомленняВтрат = "/sub_al_lost";

    /// <summary>
    ///     Subscription on wf command
    /// </summary>
    public const string КомандаПідписатисяНаПрогнозПогоди = "/sub_wf";

    /// <summary>
    ///     UnSubscription from al_lost command
    /// </summary>
    public const string КомандаВідписатисяВідПовідомленьВтрат = "/unsub_al_lost";

    /// <summary>
    ///     UnSubscription from wf command
    /// </summary>
    public const string КомандаВідписатисяВідПрогнозаПогоди = "/unsub_wf";

    /// <summary>
    ///     Receiving list of subscriptions for selected user command
    /// </summary>
    public const string КомандаВзятиСписокПідпісок = "/sub_list";

    /// <summary>
    ///     Creating reminder command
    /// </summary>
    public const string Нагадування = "/remind";
}