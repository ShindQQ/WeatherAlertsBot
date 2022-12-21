using System.Text.Json.Serialization;

namespace WeatherAlertsBot.RussianWarship;

public sealed class RussianWarshipInfo
{
    [JsonPropertyName("date")]
    public string Date { get; set; }

    [JsonPropertyName("day")]
    public int Day { get; set; }

    [JsonPropertyName("stats")]
    public LiquidatedStats LiquidatedStats { get; set; }

    [JsonPropertyName("increase")]
    public LiquidatedStats IncreaseLiquidatedStats { get; set; }
}
