using System.Text.Json.Serialization;

namespace WeatherAlertsBot.RussianWarship.AlarmsInfo;

/// <summary>
///     Class which represents alarms info
/// </summary>
public sealed class AlarmsInRegion
{
    /// <summary>
    ///     Id of the region
    /// </summary>
    [JsonPropertyName("regionId")]
    public int RegionId { get; set; }

    /// <summary>
    ///     Type of the alarm
    /// </summary>
    [JsonPropertyName("alarmType")]
    public string AlarmType { get; set; }

    /// <summary>
    ///     Initiator of the alarm
    /// </summary>
    [JsonPropertyName("alarmInitiator")]
    public string AlarmInitiator { get; set; }
}
