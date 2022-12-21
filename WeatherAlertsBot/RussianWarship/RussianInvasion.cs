using System.Text.Json.Serialization;

namespace WeatherAlertsBot.RussianWarship;

public sealed class RussianInvasion
{
    [JsonPropertyName("data")]
    public RussianWarshipInfo RussianWarshipInfo { get; set; }
}
