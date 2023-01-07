using System.Text.Json.Serialization;

namespace WeatherAlertsBot.OpenWeatherAPI.WeatherForecast;

public class WeatherForecastData
{
    [JsonPropertyName("temp")]
    public float Temperature { get; set; }

    [JsonPropertyName("feels_like")]
    public float FeelsLike { get; set; }

    [JsonPropertyName("humidity")]
    public int Humidity { get; set; }
}
