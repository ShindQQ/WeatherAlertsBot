using CoreHtmlToImage;
using WeatherAlertsBot.OpenWeatherAPI.WeatherForecast;

namespace WeatherAlertsBot.RussianWarship;

public class WeatherToPrint
{
    public string IconType { get; set; }
    public string CityName { get; set; }
    public float Temperature { get; set; }
    public float FeelsLike { get; set; }
    public string? DateTime { get; set; }
}


public static class WeatherMapGenerator
{
    
    public static byte[] GenerateCurrentWeatherImage (WeatherToPrint weatherToPrint)
    {
        var weatherForecastImage = $"""
                            <div style="margin-left:5%;height:250px;width:250px;display:inline">
                            <img
                            src = "http://openweathermap.org/img/wn/{weatherToPrint.IconType}@4x.png" 
                              >
                            <h1>Weather in {weatherToPrint.CityName}</h1>
                            <h1>Temperature {weatherToPrint.Temperature:N1}°C</h1>
                            </div>
                        """;
        return new HtmlConverter().FromHtmlString(weatherForecastImage);
    } 
    
    /// <summary>
    /// need to do something with margins
    /// </summary>
    /// <param name="weatherToPrint"></param>
    /// <returns></returns>
    public static byte[] GenerateWeatherForecastImage (List<WeatherToPrint> weatherToPrint)
    {
        string result =""" <div style="width:2000px">""";
        weatherToPrint.ForEach(weather => result += $"""
                            <div style="display:inline-block;margin-right:20px;">
                            <div style="margin-left:5%;height:400px;width:250px;">
                            <img
                            src = "http://openweathermap.org/img/wn/{weather.IconType}@4x.png" 
                              >
                            <h1>Weather in {weather.CityName}</h1>
                            <h1>Temperature {weather.Temperature:N1}°C</h1>
                            </div>
                            </div>
                        """);
        return new HtmlConverter().FromHtmlString(result + "</div>");
    }
}