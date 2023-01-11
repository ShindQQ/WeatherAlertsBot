using System.Text.Json.Serialization;

namespace WeatherAlertsBot.OpenWeatherAPI.Models.WeatherForecast;

/// <summary>
///     Information about type of weather 
/// </summary>
public class WeatherForecastCurrentWeather
{
    /// <summary>
    ///     Type of weather e.g. (sunny, rainy)...
    /// </summary>
    [JsonPropertyName("main")]
    public string TypeOfWeather { get; set; }
}
