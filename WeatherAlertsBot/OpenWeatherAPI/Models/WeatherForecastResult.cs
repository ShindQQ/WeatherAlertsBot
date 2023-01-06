using System.Text.Json.Serialization;

namespace WeatherAlertsBot.OpenWeatherAPI.Models;

/// <summary>
///     Class which represents weather forecast from OpenWeatherAPI Current Weather
/// </summary>
public sealed class WeatherForecastResult
{
    /// <summary>
    ///     List(?) of weather by coordinates
    /// </summary>
    [JsonPropertyName("weather")]
    public List<WeatherInfo> WeatherInfo { get; set; }

    /// <summary>
    ///     Current temperature by coordinates
    /// </summary>
    [JsonPropertyName("main")]
    public TemperatureInfo TemperatureInfo { get; set; }

    /// <summary>
    ///     Name of the location
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }
}
