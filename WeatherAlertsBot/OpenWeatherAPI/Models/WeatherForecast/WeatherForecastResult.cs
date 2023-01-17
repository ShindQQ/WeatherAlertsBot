using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace WeatherAlertsBot.OpenWeatherAPI.Models.WeatherForecast;

/// <summary>
///     Class to represent weather forecast 
/// </summary>
[JsonObject(MemberSerialization.OptIn)]
public sealed class WeatherForecastResult
{
    /// <summary>
    ///     Data for selected city by hours
    /// </summary>
    [JsonPropertyName("list")]
    public List<WeatherForecastHoursData> WeatherForecastHoursData { get; set; }

    /// <summary>
    ///     Name of the specified city
    /// </summary>
    [JsonPropertyName("city")]
    public WeatherForecastCity WeatherForecastCity { get; set; }

    /// <summary>
    ///     Error message for user if something went wrong for specified request
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;
}