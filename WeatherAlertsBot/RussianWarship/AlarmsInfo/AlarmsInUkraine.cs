using System.Text.Json.Serialization;

namespace WeatherAlertsBot.RussianWarship.AlarmsInfo;

/// <summary>
///     Information about alarms in Ukratne
/// </summary>
public sealed class AlarmsInUkraine
{
    /// <summary>
    ///     Regions with alarm
    /// </summary>
    [JsonPropertyName("regions")]
    public List<Region> Regions { get; set; }

}
