using System.Text.Json.Serialization;

namespace WeatherAlertsBot.OpenWeatherAPI;

public sealed class WeatherForecastResult
{
    [JsonPropertyName("weather")]
    public List<WeatherInfo> WeatherInfo { get; set; }

    [JsonPropertyName("main")]
    public TemperatureInfo TemperatureInfo { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}
