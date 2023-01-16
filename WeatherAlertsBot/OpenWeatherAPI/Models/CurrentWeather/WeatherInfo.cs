using System.Text.Json.Serialization;

namespace WeatherAlertsBot.OpenWeatherAPI.Models.CurrentWeather;

/// <summary>
///     Class which represents current weather from OpenWeatherAPI Current Weather
/// </summary>
public sealed class WeatherInfo
{
    /// <summary>
    ///     Type of weather by coordinates
    /// </summary>
    [JsonPropertyName("main")]
    public string TypeOfWeather { get; set; }

    /// <summary>
    ///     Type of received icon for weather
    /// </summary>
    [JsonPropertyName("icon")]
    public string IconType { get; set; }
}
