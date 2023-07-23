using System.Text.Json.Serialization;

namespace WeatherAlertsBot.OpenWeatherAPI.Models.WeatherForecast;

/// <summary>
///     Information about type of weather 
/// </summary>
public sealed class WeatherForecastCurrentWeather
{
    /// <summary>
    ///     Type of weather e.g. (sunny, rainy)...
    /// </summary>
    [JsonPropertyName("main")]
    public string TypeOfWeather { get; set; } = string.Empty;

    /// <summary>
    ///     Type of received icon for weather
    /// </summary>
    [JsonPropertyName("icon")]
    public string IconType { get; set; } = string.Empty;
}
