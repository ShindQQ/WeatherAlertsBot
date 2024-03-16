using System.Text.Json.Serialization;

namespace WeatherAlertsBot.OpenWeatherApi.Models.CurrentWeather;

/// <summary>
///     Class which represents current temperature from OpenWeatherAPI Current Weather
/// </summary>
public sealed class TemperatureInfo
{
    /// <summary>
    ///     Temperature of the coordinate in Kelvins
    /// </summary>
    [JsonPropertyName("temp")]
    public float Temperature { get; set; }

    /// <summary>
    ///     How does temperature feels like of the coordinate in Kelvins
    /// </summary>
    [JsonPropertyName("feels_like")]
    public float FeelsLike { get; set; }
}