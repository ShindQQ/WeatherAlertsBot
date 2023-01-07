using System.Text.Json.Serialization;

namespace WeatherAlertsBot.OpenWeatherAPI.WeatherForecast;

/// <summary>
///     Weather data for each 3 hours
/// </summary>
public class WeatherForecastDataForEveryThreeHours
{
    /// <summary>
    ///     Temperature data for weather forecast
    /// </summary>
    [JsonPropertyName("main")]
    public WeatherForecastTemperatureData WeatherForecastTemperatureData { get; set; }

    /// <summary>
    ///     Current weather for selected time
    /// </summary>
    [JsonPropertyName("weather")]
    public List<WeatherForecastCurrentWeather> WeatherForecastCurrentWeather { get; set; }

    /// <summary>
    ///     Probability of precipitation for selected time
    /// </summary>
    [JsonPropertyName("pop")]
    public float ProbabilityOfPrecipitation { get; set; }

    /// <summary>
    ///     Time of selected request
    /// </summary>
    [JsonPropertyName("dt_txt")]
    public string Date { get; set; }
}