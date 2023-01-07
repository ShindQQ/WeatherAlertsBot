using System.Text.Json.Serialization;

namespace WeatherAlertsBot.OpenWeatherAPI.WeatherForecast;

public class WeatherForecastCurrentWeather
{
    [JsonPropertyName("main")]
    public string TypeOfWeather { get; set; }
}
