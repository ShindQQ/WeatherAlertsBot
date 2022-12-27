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
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    /// <summary>
    ///     Type of state
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

    /// <summary>
    ///     When was alarm enabled
    /// </summary>
    [JsonPropertyName("enabled_at")]
    public string EnabledAt { get; set; }

    /// <summary>
    ///     When was alarm disabled
    /// </summary>
    [JsonPropertyName("disabled_at")]
    public string DisabledAt { get; set; }
}