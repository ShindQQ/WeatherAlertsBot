namespace WeatherAlertsBot.OpenWeatherAPI;

/// <summary>
///     Class which represents response for user searching for weather
/// </summary>
public sealed class WeatherResponseForUser
{
    /// <summary>
    ///     Name of the city in which user was searching for weather
    /// </summary>
    public string CityName { get; set; } = string.Empty;

    /// <summary>
    ///     Lattitude of selected city/ given location
    /// </summary>
    public float Lattitude { get; set; }

    /// <summary>
    ///     Longitude of selected city/ given location
    /// </summary>
    public float Longitude { get; set; }

    /// <summary>
    ///     Result of temperature by given city name/location
    /// </summary>
    public float Temperature { get; set; }

    /// <summary>
    ///     Result of how temperature feels like by given city name/location
    /// </summary>
    public float FeelsLike { get; set; }

    /// <summary>
    ///     Which weather it`s now (snowing, sunny, etc.)
    /// </summary>
    public string TypeOfWeather { get; set; } = string.Empty;

    /// <summary>
    ///     Error message for user if there were some troubles for his request
    /// </summary>

    /// <summary>
    /// Type of icon that will need to be downloaded from OpenWeatherAPI
    /// </summary>
    public string IconType { get; set; }

    public string ErrorMessage { get; set; } = string.Empty;
}
