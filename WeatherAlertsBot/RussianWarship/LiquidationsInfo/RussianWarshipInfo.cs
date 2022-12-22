using System.Text.Json.Serialization;

namespace WeatherAlertsBot.RussianWarship;

/// <summary>
///     Info about looses
/// </summary>
public sealed class RussianWarshipInfo
{
    /// <summary>
    ///     Date of update
    /// </summary>
    [JsonPropertyName("date")]
    public string Date { get; set; }

    /// <summary>
    ///     Day of the war
    /// </summary>
    [JsonPropertyName("day")]
    public int Day { get; set; }

    /// <summary>
    ///     Enemies looses info
    /// </summary>
    [JsonPropertyName("stats")]
    public LiquidatedStats LiquidatedStats { get; set; }

    /// <summary>
    ///     Enemies looses change 
    /// </summary>
    [JsonPropertyName("increase")]
    public LiquidatedStats IncreaseLiquidatedStats { get; set; }
}
