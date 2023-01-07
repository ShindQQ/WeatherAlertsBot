using System.Text.Json.Serialization;

namespace WeatherAlertsBot.OpenWeatherAPI.WeatherForecast;

public class WeatherForecastHoursData
{
    [JsonPropertyName("main")]
    public WeatherForecastData WeatherForecastData { get; set; }

    [JsonPropertyName("weather")]
    public List<WeatherForecastCurrentWeather> WeatherForecastCurrentWeather { get; set; }

    [JsonPropertyName("pop")]
    public float ProbabilityOfPrecipitation { get; set; }

    [JsonPropertyName("dt_txt")]
    public string Date { get; set; }
}