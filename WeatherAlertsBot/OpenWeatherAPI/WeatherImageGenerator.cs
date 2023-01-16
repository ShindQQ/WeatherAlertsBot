using CoreHtmlToImage;

namespace WeatherAlertsBot.OpenWeatherAPI;

/// <summary>
///     Static class for generating images for weather forecasts
/// </summary>
public static class WeatherImageGenerator
{
    /// <summary>
    ///     Generating image for current weather
    /// </summary>
    /// <param name="weather">Weather which will be shown</param>
    /// <returns>Byte array of the image</returns>
    public static byte[] GenerateCurrentWeatherImage(WeatherResponseForUser weather)
    {
        var weatherForecastImage = $"""
        <div style="height:220px;
        width:500px;
        background-image:-webkit-linear-gradient(67deg, #151125 12%, #39278a 88%);
        text-align:center;
        position:absolute;top:0px;left:0px">
            <div>
                <img
                src = "http://openweathermap.org/img/wn/{weather.IconType}@2x.png" 
                  >
                <h1 style="font-size:12px;color:white">Weather in {weather.CityName}</h1>
                <h1 style="font-size:12px;color:white">Temperature {weather.Temperature:N2} &degC</h1>
                <h1 style="font-size:12px;color:white">Feels like {weather.FeelsLike:N2} &degC</h1>
            </div>
        </div>
        """;

        return new HtmlConverter().FromHtmlString(weatherForecastImage, width: 500);
    }

    public static byte[] GenerateWeatherForecastImage(List<WeatherResponseForUser> weatherToPrint)
    {
        string result = """ <div style="width:2000px">""";
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