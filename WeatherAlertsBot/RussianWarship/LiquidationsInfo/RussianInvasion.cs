using System.Text.Json.Serialization;

namespace WeatherAlertsBot.RussianWarship.LiquidationsInfo;

/// <summary>
///     Class for working with RussianWarshipAPI
/// </summary>
public sealed class RussianInvasion
{
    /// <summary>
    ///     Data about enemies looses
    /// </summary>
    [JsonPropertyName("data")]
    public RussianWarshipInfo RussianWarshipInfo { get; set; } = null!;
}