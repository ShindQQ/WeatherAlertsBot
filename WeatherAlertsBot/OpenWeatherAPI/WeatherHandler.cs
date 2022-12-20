using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using Telegram.Bot.Types;

namespace WeatherAlertsBot.OpenWeatherAPI;

/// <summary>
///     Class which represents work with OpenWeatherAPI
/// </summary>
public sealed class WeatherHandler
{
    /// <summary>
    ///     HttpClient 
    /// </summary>
    private static readonly HttpClient HttpClient = new();

    /// <summary>
    ///     Our Api Key
    /// </summary>
    private string OpenWeatherApiKey { get; init; }

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
    public async ValueTask<WeatherForecastResult> GetCurrentWeatherByCoordinatesAsync(float lattitude, float longitude)
    {
        string url = @$"https://api.openweathermap.org/data/2.5/weather?lat={lattitude}&lon={longitude}&appid={OpenWeatherApiKey}";

        return await GetResponseFromOpenWeatherAPI<WeatherForecastResult>(url);
    }

    /// <summary>
    ///     Receiving coordinates of city
    /// </summary>
    /// <param name="cityName">Name of the city</param>
    /// <returns>CoordinatesInfo</returns>
    public async ValueTask<IEnumerable<CoordinatesInfo>> GetLattitudeAndLongitudeByCityNameAsync(string cityName)
    {
        string url = @$"http://api.openweathermap.org/geo/1.0/direct?q={cityName}&appid={OpenWeatherApiKey}";

        return await GetResponseFromOpenWeatherAPI<IEnumerable<CoordinatesInfo>>(url);
    }

    /// <summary>
    ///     Receiving response from OpenWeatherAPI
    /// </summary>
    /// <typeparam name="T">Describing type for deserializing</typeparam>
    /// <param name="url">Url for request</param>
    /// <returns>T as deserialized response from request</returns>
    /// <exception cref="HttpRequestException">If response`s status code isn`t 200</exception>
    public async ValueTask<T> GetResponseFromOpenWeatherAPI<T>(string url)
    {
        var response = await HttpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException("Some troubles happened with your request!");
        }

        return await response.Content.ReadFromJsonAsync<T>();
    }

    /// <summary>
    ///     Generating data for response for the user
    /// </summary>
    /// <param name="userMessage">Message sent by the user</param>
    /// <returns>WeatherResponseForUser</returns>
    public async ValueTask<WeatherResponseForUser> SendWeatherByUserMessageAsync(string userMessage)
    {
        var splittedUserMessage = userMessage.Trim().Split(' ', 2);

        if (!splittedUserMessage[0].ToLower().StartsWith("/weather"))
        {
            return new WeatherResponseForUser { ErrorMessage = @"Format of the input was wrong\!" };
        }

        var coordinatesInfo = await GetLattitudeAndLongitudeByCityNameAsync(splittedUserMessage[1]);

        if (coordinatesInfo.Count() <= 0)
        {
            return new WeatherResponseForUser { ErrorMessage = @"No data was found for your request\!" };
        }

        var coordinatesInfoFirst = coordinatesInfo.First();

        var temperatureInfo = await GetCurrentWeatherByCoordinatesAsync(coordinatesInfoFirst.Lattitude, coordinatesInfoFirst.Longitude);

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

        return new WeatherResponseForUser
        {
            CityName = temperatureInfo.Name,
            Temperature = temperatureInfo.TemperatureInfo.Temperature - 273.15f,
            FeelsLike = temperatureInfo.TemperatureInfo.FeelsLike - 273.15f,
            WeatherInfo = temperatureInfo.WeatherInfo.First().TypeOfWeather
        };
    }
}
