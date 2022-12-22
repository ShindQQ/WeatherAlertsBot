using System.Text.Json.Serialization;

namespace WeatherAlertsBot.RussianWarship.AlarmsInfo;

/// <summary>
///     Alarm region data
/// </summary>
public sealed class Region
{
    /// <summary>
    ///     Name of the region
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    ///     Regions with alarm
    /// </summary>
    [JsonPropertyName("alarmsInRegion")]
    public List<AlarmsInRegion> AlarmsInRegion { get; set; }

    /// <summary>
    ///     Is it region or city
    /// </summary>
    [JsonPropertyName("state")]
    public bool State { get; set; }
}
