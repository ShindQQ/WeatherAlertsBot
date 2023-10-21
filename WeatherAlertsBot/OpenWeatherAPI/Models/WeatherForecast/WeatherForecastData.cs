using System.Text.Json.Serialization;

namespace WeatherAlertsBot.OpenWeatherApi.Models.WeatherForecast;

/// <summary>
///     Data about temperature for weather forecast
/// </summary>
public sealed class WeatherForecastTemperatureData
{
    /// <summary>
    ///     Current weather for selected time
    /// </summary>
    [JsonPropertyName("temp")]
    public float Temperature { get; set; }

    /// <summary>
    ///     How weather feels for selected time
    /// </summary>
    [JsonPropertyName("feels_like")]
    public float FeelsLike { get; set; }

    /// <summary>
    ///     Humidity rate for selected time
    /// </summary>
    [JsonPropertyName("humidity")]
    public int Humidity { get; set; }
}
