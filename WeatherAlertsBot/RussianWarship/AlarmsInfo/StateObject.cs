using System.Text.Json.Serialization;

namespace WeatherAlertsBot.RussianWarship.AlarmsInfo;

/// <summary>
///     Info about state
/// </summary>
public sealed class StateObject
{
    /// <summary>
    ///     Is there alarm or not
    /// </summary>
    [JsonPropertyName("alertnow")]
    public bool Enabled { get; set; }
}