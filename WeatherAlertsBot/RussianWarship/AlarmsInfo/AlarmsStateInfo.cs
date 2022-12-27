using System.Text.Json.Serialization;

namespace WeatherAlertsBot.RussianWarship.AlarmsInfo;

/// <summary>
///     Alarms in Ukraien data
/// </summary>
public sealed class AlarmsStateInfo
{
    /// <summary>
    ///     States of Ukraine
    /// </summary>
    [JsonPropertyName("states")]
    public Dictionary<string, StateObject> States { get; set; }
}

