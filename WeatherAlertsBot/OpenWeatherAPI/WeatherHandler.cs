using Microsoft.Extensions.Configuration;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using WeatherAlertsBot.Requesthandlers;

namespace WeatherAlertsBot.OpenWeatherAPI;

/// <summary>
///     Class which represents work with OpenWeatherAPI
/// </summary>
public sealed class WeatherHandler
{
    /// <summary>
    ///     Our Api Key
    /// </summary>
    private string OpenWeatherApiKey { get; init; }

    /// <summary>
    ///     Class of logic for calling APIs
    /// </summary>
    private readonly APIsRequestsHandler APIsRequestsHandler = new();

    /// <summary>
    ///     Constructor for api key inicializing 
    /// </summary>
    public WeatherHandler()
    {
        var configuration = new ConfigurationBuilder()
                 .AddJsonFile($"appsettings.json", true, true).Build();

        OpenWeatherApiKey = configuration["OpenWeatherApiKey"];
    }

    /// <summary>
    ///     Receiving current weather info by lattitude and longitude
    /// </summary>
    /// <param name="lattitude">Location lattitude</param>
    /// <param name="longitude">Location longitude</param>
    /// <returns>WeatherForecastResult</returns>
    public async ValueTask<WeatherForecastResult?> GetCurrentWeatherByCoordinatesAsync(float lattitude, float longitude)
    {
        string url = $"https://api.openweathermap.org/data/2.5/weather?lat={lattitude}&lon={longitude}&appid={OpenWeatherApiKey}";

        return await APIsRequestsHandler.GetResponseFromAPI<WeatherForecastResult>(url);
    }

    /// <summary>
    ///     Receiving coordinates of city
    /// </summary>
    /// <param name="cityName">Name of the city</param>
    /// <returns>CoordinatesInfo</returns>
    public async ValueTask<IEnumerable<CoordinatesInfo>?> GetLattitudeAndLongitudeByCityNameAsync(string cityName)
    {
        string url = $"http://api.openweathermap.org/geo/1.0/direct?q={cityName}&appid={OpenWeatherApiKey}";

        return await APIsRequestsHandler.GetResponseFromAPI<IEnumerable<CoordinatesInfo>>(url);
    }

    /// <summary>
    ///     Generating data for response for the user
    /// </summary>
    /// <param name="userMessage">Message sent by the user</param>
    /// <returns>WeatherResponseForUser</returns>
    public async ValueTask<WeatherResponseForUser> SendWeatherByUserMessageAsync(string userMessage)
    {
        if (userMessage.Equals("/weather"))
        {
            return new WeatherResponseForUser { ErrorMessage = @"`Format of the input was wrong\!`" };
        }

        var splittedUserMessage = userMessage.Trim().Split(' ', 2);

        if (!splittedUserMessage[0].ToLower().StartsWith("/weather"))
        {
            return new WeatherResponseForUser { ErrorMessage = @"`Format of the input was wrong\!`" };
        }

        var coordinatesInfo = await GetLattitudeAndLongitudeByCityNameAsync(splittedUserMessage[1]);

        if (coordinatesInfo == null || !coordinatesInfo.Any())
        {
            return new WeatherResponseForUser { ErrorMessage = @"`No data was found for your request\!`" };
        }

        var coordinatesInfoFirst = coordinatesInfo.First();

        var temperatureInfo = await GetCurrentWeatherByCoordinatesAsync(coordinatesInfoFirst.Lattitude, coordinatesInfoFirst.Longitude);

        if (temperatureInfo == null)
        {
            return new WeatherResponseForUser { ErrorMessage = @"`No data was found for your request\!`" };
        }

        return new WeatherResponseForUser
        {
            CityName = coordinatesInfoFirst.CityName,
            Temperature = temperatureInfo.TemperatureInfo.Temperature - 273.15f,
            FeelsLike = temperatureInfo.TemperatureInfo.FeelsLike - 273.15f,
            Longitude = coordinatesInfoFirst.Longitude,
            Lattitude = coordinatesInfoFirst.Lattitude,
            WeatherInfo = temperatureInfo.WeatherInfo.First().TypeOfWeather
        };
    }

    /// <summary>
    ///     Receiving weather by user location
    /// </summary>
    /// <param name="userLocation">Location sent by user</param>
    /// <returns>WeatherResponseForUser</returns>
    public async ValueTask<WeatherResponseForUser> SendWeatherByUserLocationAsync(Location userLocation)
    {
        var temperatureInfo = await GetCurrentWeatherByCoordinatesAsync((float)userLocation.Latitude, (float)userLocation.Longitude); ;

        if (temperatureInfo == null)
        {
            return new WeatherResponseForUser { ErrorMessage = @"`No data was found for your request\!`" };
        }

        return new WeatherResponseForUser
        {
            CityName = temperatureInfo.Name,
            Temperature = temperatureInfo.TemperatureInfo.Temperature - 273.15f,
            FeelsLike = temperatureInfo.TemperatureInfo.FeelsLike - 273.15f,
            WeatherInfo = temperatureInfo.WeatherInfo.First().TypeOfWeather
        };
    }
}
