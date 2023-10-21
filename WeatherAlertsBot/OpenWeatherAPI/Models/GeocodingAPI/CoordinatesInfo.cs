using System.Text.Json.Serialization;

namespace WeatherAlertsBot.OpenWeatherApi.Models.GeoCodingAPI;

/// <summary>
///     Class which represents response from GeoCoding API
/// </summary>
public sealed class CoordinatesInfo
{
    /// <summary>
    ///     Name of the city
    /// </summary>
    [JsonPropertyName("name")]
    public string CityName { get; set; } = string.Empty;

    /// <summary>
    ///     Latitude of the city
    /// </summary>
    [JsonPropertyName("lat")]
    public float Latitude { get; set; }

    /// <summary>
    ///     Longitude of the city
    /// </summary>
    [JsonPropertyName("lon")]
    public float Longitude { get; set; }

    /// <summary>
    ///     Country in which city is located
    /// </summary>
    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;

    /// <summary>
    ///     State in which city is located
    /// </summary>
    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;
}
