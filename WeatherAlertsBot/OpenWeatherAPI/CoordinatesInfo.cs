using System.Text.Json.Serialization;

namespace WeatherAlertsBot.OpenWeatherAPI;

public class CoordinatesInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("lat")]
    public float Lattitude { get; set; }

    [JsonPropertyName("lon")]
    public float Longitude { get; set; }

    [JsonPropertyName("country")]
    public string Country { get; set; }

    [JsonPropertyName("state")]
    public string State { get; set; }
}
