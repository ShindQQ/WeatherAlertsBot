using System.Text.Json.Serialization;

namespace WeatherAlertsBot.OpenWeatherAPI.Models.WeatherForecast;

/// <summary>
///     City info for weather forecast result
/// </summary>
public sealed class WeatherForecastCity
{
    /// <summary>
    ///     Name of the city
    /// </summary>
    [JsonPropertyName("name")]
    public string CityName { get; set; }
}
