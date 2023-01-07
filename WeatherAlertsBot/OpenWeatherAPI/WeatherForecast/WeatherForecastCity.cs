using System.Text.Json.Serialization;

namespace WeatherAlertsBot.OpenWeatherAPI.WeatherForecast;

public class WeatherForecastCity
{
    [JsonPropertyName("name")]
    public string CityName { get; set; }
}
