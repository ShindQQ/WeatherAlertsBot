namespace WeatherAlertsBot.Помічники;

/// <summary>
///     Static class for links on different API`s which will be used further
/// </summary>
public static class ПосиланняАПІ
{
    /// <summary>
    ///     Url for receiving list of alarms in Ukraine
    /// </summary>
    public const string AlarmsInUkraineInfoUrl = "https://ubilling.net.ua/aerialalerts/?map";

    /// <summary>
    ///     Url for receiving list of enemy looses
    /// </summary>
    public const string RussianWarshipUrl = "https://russianwarship.rip/api/v1/statistics/latest";

    /// <summary>
    ///     Open Weather API url
    /// </summary>
    public const string OpenWeatherApiUrl = "https://api.openweathermap.org";

    /// <summary>
    ///     Open weather API icons
    /// </summary>
    public const string OpenWeatherApiIcons = "http://openweathermap.org/img/wn/";

    /// <summary>
    ///     Open Weather API`s Current weather url
    /// </summary>
    public const string CurrentWeatherUrl = "/data/2.5/weather";

    /// <summary>
    ///     Open Weather API`s Weather forecast url
    /// </summary>
    public const string WeatherForecastUrl = "/data/2.5/forecast";

    /// <summary>
    ///     Open Weather API`s Geo API url
    /// </summary>
    public const string GeoAPIUrl = "/geo/1.0/direct";
}