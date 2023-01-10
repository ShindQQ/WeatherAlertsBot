using CoreHtmlToImage;
using WeatherAlertsBot.OpenWeatherAPI;

namespace WeatherAlertsBot.RussianWarship;

public static class WeatherMapGenerator
{
    
    public static byte[] GenerateWeatherForecastImage (WeatherResponseForUser weatherResponseForUser)
    {
        var weatherForecastImage = $"""
                            <html lang="ua">
                            <head>
                            <meta charset="utf-8">
                             </head>
                            <div style="margin-left:5%";>
                            <img
                            src = "http://openweathermap.org/img/wn/{weatherResponseForUser.IconType}@4x.png" 
                               >
                            <h1>Weather in {weatherResponseForUser.CityName}</h1>
                            <h1>Temperature {weatherResponseForUser.Temperature:N1}°C</h1>
                            </div>
                        </html>
                        """;
        return new HtmlConverter().FromHtmlString(weatherForecastImage);
    }
    
}