using System.Text.Json.Serialization;

namespace WeatherAlertsBot.РосійськийКорабль.AlarmsInfo;

/// <summary>
///     Info about state
/// </summary>
public sealed class ОбєктОбласті
{
    /// <summary>
    ///     Is there alarm or not
    /// </summary>
    [JsonPropertyName("alertnow")]
    public bool Включена { get; set; }
}