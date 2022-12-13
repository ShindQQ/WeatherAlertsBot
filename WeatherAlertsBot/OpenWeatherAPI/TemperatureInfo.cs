using System.Text.Json.Serialization;

namespace WeatherAlertsBot.OpenWeatherAPI;

public sealed class TemperatureInfo
{
    [JsonPropertyName("temp")]
    public float Temperature { get; set; }

    [JsonPropertyName("feels_like")]
    public float FeelsLike { get; set; }
}