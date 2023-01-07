using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace WeatherAlertsBot.OpenWeatherAPI.WeatherForecast;

[JsonObject(MemberSerialization.OptIn)]
public sealed class WeatherForecastResult
{
    [JsonPropertyName("list")]
    public List<WeatherForecastDataForEveryThreeHours> WeatherForecastHoursData { get; set; }

    [JsonPropertyName("city")]
    public WeatherForecastCity WeatherForecastCity { get; set; }

    public string ErrorMessage { get; set; } = string.Empty;
}