using System.Text.Json.Serialization;

namespace WeatherAlertsBot.OpenWeatherAPI;

public sealed class WeatherInfo
{
    [JsonPropertyName("main")]
    public string TypeOfWeather { get; set; }
}
