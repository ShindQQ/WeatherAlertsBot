using CoreHtmlToImage;
using WeatherAlertsBot.Helpers;
using WeatherAlertsBot.OpenWeatherAPI.Models.WeatherForecast;

namespace WeatherAlertsBot.OpenWeatherAPI;

/// <summary>
///     Static class for generating images for weather forecasts
/// </summary>
public static class WeatherImageGenerator
{
    /// <summary>
    ///     Generating image for current weather
    /// </summary>
    /// <param name="currentWeather">Weather which will be shown</param>
    /// <returns>Byte array of the image</returns>
    public static byte[] GenerateCurrentWeatherImage(WeatherResponseForUser currentWeather)
    {
        var weatherForecastImage = $"""
        <div style="height:220px;
        width:500px;
        background-image:-webkit-linear-gradient(67deg, #151125 12%, #39278a 88%);
        text-align:center;
        position:absolute;top:0px;left:0px">
            <img
            src = "{APIsLinks.OpenWeatherApiIcons}{currentWeather.IconType}@2x.png" 
              >
            <h1 style="font-size:14px;color:white">Weather in {currentWeather.CityName}</h1>
            <h1 style="font-size:14px;color:white">Temperature {currentWeather.Temperature:N2} &degC</h1>
            <h1 style="font-size:14px;color:white">Feels like {currentWeather.FeelsLike:N2} &degC</h1>
        </div>
        """;

        return new HtmlConverter().FromHtmlString(weatherForecastImage, width: 500);
    }

    /// <summary>
    ///     Generating image for weather forecast
    /// </summary>
    /// <param name="weatherForecast">Weather forecast data</param>
    /// <returns>Byte array of the image</returns>
    public static byte[] GenerateWeatherForecastImage(WeatherForecastResult weatherForecast)
    {
        string result = """
            <div style="width:1300px;
                background-image:-webkit-linear-gradient(67deg, #151125 12%, #39278a 88%);
                text-align:center;
                position: absolute; top: 0px; left: 0px">
            """;

        weatherForecast.WeatherForecastHoursData.ForEach(weatherData => result +=
        $"""
            <div style="display:inline-block;">
                <div style="height:430px;width:320px;">
                    <img
                    src =  "{APIsLinks.OpenWeatherApiIcons}{weatherData.WeatherForecastCurrentWeather.First().IconType}@4x.png" 
                      >
                    <h1 style="font-size:22px;color:white">Weather in {weatherForecast.WeatherForecastCity.CityName}</h1>
                    <h1 style="font-size:22px;color:white">On {weatherData.Date[..^3]}</h1>
                    <h1 style="font-size:22px;color:white">Temperature {weatherData.WeatherForecastTemperatureData.Temperature:N2} &degC</h1>
                    <h1 style="font-size:22px;color:white">Feels like {weatherData.WeatherForecastTemperatureData.FeelsLike:N2} &degC</h1>
                    <h1 style="font-size:22px;color:white">Humidity {weatherData.WeatherForecastTemperatureData.Humidity}%</h1>
                </div>
            </div>
        """);

        return new HtmlConverter().FromHtmlString(result + "</div>", width: 1300);
    }
}