using System.Text.Json.Serialization;

namespace WeatherAlertsBot.РосійськийКорабль.AlarmsInfo;

/// <summary>
///     Alarms in Ukraine data
/// </summary>
public sealed class ІнформаціяТривогОбластей
{
    /// <summary>
    ///     States of Ukraine
    /// </summary>
    [JsonPropertyName("states")]
    public Dictionary<string, ОбєктОбласті> Області { get; set; } = null!;
}