using System.Text.Json.Serialization;

namespace WeatherAlertsBot.OpenWeatherApi.Models.CurrentWeather;

/// <summary>
///     Class which represents weather forecast from OpenWeatherAPI Current Weather
/// </summary>
public sealed class WeatherResult
{
    /// <summary>
    ///     List(?) of weather by coordinates
    /// </summary>
    [JsonPropertyName("weather")]
    public List<WeatherInfo> WeatherInfo { get; set; } = new();

    /// <summary>
    ///     Current temperature by coordinates
    /// </summary>
    [JsonPropertyName("main")]
    public TemperatureInfo TemperatureInfo { get; set; } = null!;

    /// <summary>
    ///     Name of the location
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
