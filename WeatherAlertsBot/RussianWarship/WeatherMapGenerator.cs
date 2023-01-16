using CoreHtmlToImage;
using WeatherAlertsBot.OpenWeatherAPI;

namespace WeatherAlertsBot.RussianWarship;

public static class WeatherImageGenerator
{
    public static byte[] GenerateCurrentWeatherImage (WeatherResponseForUser weatherToPrint)
    {
        var weatherForecastImage = $"""
                            <div style="margin-left:5%;height:250px;width:250px;display:inline">
                            <img
                            src = "http://openweathermap.org/img/wn/{weatherToPrint.IconType}@4x.png" 
                              >
                            <h1>Weather in {weatherToPrint.CityName}</h1>
                            <h1>Temperature {weatherToPrint.Temperature:N1} &degC</h1>
                            </div>
                        """;

        return new HtmlConverter().FromHtmlString(weatherForecastImage);
    } 
    
    public static byte[] GenerateWeatherForecastImage (List<WeatherResponseForUser> weatherToPrint)
    {
        string result =""" <div style="width:2000px">""";
        weatherToPrint.ForEach(weather => result += $"""
                            <div style="display:inline-block;margin-right:20px;">
                            <div style="margin-left:5%;height:400px;width:250px;">
                            <img
                            src = "http://openweathermap.org/img/wn/{weather.IconType}@4x.png" 
                              >
                            <h1>Weather in {weather.CityName}</h1>
                            <h1>Temperature {weather.Temperature:N1} &degC</h1>
                            </div>
                            </div>
                        """);

        return new HtmlConverter().FromHtmlString(result + "</div>");
    }
}