using System.Text.Json.Serialization;

namespace WeatherAlertsBot.OpenWeatherAPI.Models.WeatherForecast;

/// <summary>
///     Weather data for each 3 hours
/// </summary>
public sealed class WeatherForecastHoursData
{
    /// <summary>
    ///     Temperature data for weather forecast
    /// </summary>
    [JsonPropertyName("main")]
    public WeatherForecastTemperatureData WeatherForecastTemperatureData { get; set; } = null!;

    /// <summary>
    ///     Current weather for selected time
    /// </summary>
    [JsonPropertyName("weather")]
    public List<WeatherForecastCurrentWeather> WeatherForecastCurrentWeather { get; set; } = new();

    /// <summary>
    ///     Probability of precipitation for selected time
    /// </summary>
    [JsonPropertyName("pop")]
    public float ProbabilityOfPrecipitation { get; set; }

    /// <summary>
    ///     Time of selected request
    /// </summary>
    [JsonPropertyName("dt_txt")]
    public string Date { get; set; } = string.Empty;
}