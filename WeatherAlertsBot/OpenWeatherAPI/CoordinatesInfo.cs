using System.Text.Json.Serialization;

namespace WeatherAlertsBot.OpenWeatherAPI;

/// <summary>
///     Class which represents response from Geocoding API
/// </summary>
public sealed class CoordinatesInfo
{
    /// <summary>
    ///     Name of the city
    /// </summary>
    [JsonPropertyName("name")]
    public string CityName { get; set; }

    /// <summary>
    ///     Lattitude of the city
    /// </summary>
    [JsonPropertyName("lat")]
    public float Lattitude { get; set; }

    /// <summary>
    ///     Longitude of the city
    /// </summary>
    [JsonPropertyName("lon")]
    public float Longitude { get; set; }

    /// <summary>
    ///     Country in which city is located
    /// </summary>
    [JsonPropertyName("country")]
    public string Country { get; set; }

    /// <summary>
    ///     State in which city is located
    /// </summary>
    [JsonPropertyName("state")]
    public string State { get; set; }
}
