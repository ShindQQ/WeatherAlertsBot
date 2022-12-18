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
    ///     Error message for user if there were some troubles for his request
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;
}
