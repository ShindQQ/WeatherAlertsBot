using Telegram.Bot.Types;
using WeatherAlertsBot.Configuration;
using WeatherAlertsBot.Helpers;
using WeatherAlertsBot.Requesthandlers;

namespace WeatherAlertsBot.OpenWeatherAPI;

/// <summary>
///     Class which represents work with OpenWeatherAPI
/// </summary>
public static class WeatherHandler
{
    /// <summary>
    ///     Our Api Key
    /// </summary>
    private static string OpenWeatherApiKey { get; set; }

    /// <summary>
    ///     Constructor for api key inicializing 
    /// </summary>
    static WeatherHandler()
    {
        OpenWeatherApiKey = BotConfiguration.OpenWeatherApiKey;
    }

    /// <summary>
    ///     Receiving current weather info by lattitude and longitude
    /// </summary>
    /// <param name="lattitude">Location lattitude</param>
    /// <param name="longitude">Location longitude</param>
    /// <returns>WeatherForecastResult</returns>
    public static async Task<WeatherForecastResult?> GetCurrentWeatherByCoordinatesAsync(float lattitude, float longitude)
    {
        string url = APIsLinks.OpenWeatherAPIUrl + APIsLinks.CurrentWeatherUrl + $"?lat={lattitude}&lon={longitude}&appid={OpenWeatherApiKey}";

        return await APIsRequestsHandler.GetResponseFromAPI<WeatherForecastResult>(url);
    }

    /// <summary>
    ///     Receiving coordinates of city
    /// </summary>
    /// <param name="cityName">Name of the city</param>
    /// <returns>CoordinatesInfo</returns>
    private static async Task<IEnumerable<CoordinatesInfo>?> GetLattitudeAndLongitudeByCityNameAsync(string cityName)
    {
        string url = APIsLinks.OpenWeatherAPIUrl + APIsLinks.GeoAPIUrl + $"?q={cityName}&appid={OpenWeatherApiKey}";

        return await APIsRequestsHandler.GetResponseFromAPI<IEnumerable<CoordinatesInfo>>(url);
    }

    /// <summary>
    ///     Generating data for response for the user
    /// </summary>
    /// <param name="userMessage">Message sent by the user</param>
    /// <returns>WeatherResponseForUser</returns>
    public static async Task<WeatherResponseForUser> SendWeatherByUserMessageAsync(string userMessage)
    {
        var splittedUserMessage = userMessage.Trim().Split(' ', 2);

        var coordinatesInfo = await GetLattitudeAndLongitudeByCityNameAsync(splittedUserMessage[1]);

        if (coordinatesInfo == null || !coordinatesInfo.Any())
        {
            return new WeatherResponseForUser { ErrorMessage = "No data was found for your request!" };
        }

        var coordinatesInfoFirst = coordinatesInfo.First();

        var temperatureInfo = await GetCurrentWeatherByCoordinatesAsync(coordinatesInfoFirst.Lattitude, coordinatesInfoFirst.Longitude);

        if (temperatureInfo == null)
        {
            return new WeatherResponseForUser { ErrorMessage = "No data was found for your request!" };
        }

        return new WeatherResponseForUser
        {
            CityName = coordinatesInfoFirst.CityName,
            Temperature = temperatureInfo.TemperatureInfo.Temperature,
            FeelsLike = temperatureInfo.TemperatureInfo.FeelsLike,
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
    public static async Task<WeatherResponseForUser> SendWeatherByUserLocationAsync(Location userLocation)
    {
        var temperatureInfo = await GetCurrentWeatherByCoordinatesAsync((float)userLocation.Latitude, (float)userLocation.Longitude); ;

        if (temperatureInfo == null)
        {
            return new WeatherResponseForUser { ErrorMessage = "No data was found for your request!" };
        }

        return new WeatherResponseForUser
        {
            CityName = temperatureInfo.Name,
            Temperature = temperatureInfo.TemperatureInfo.Temperature,
            FeelsLike = temperatureInfo.TemperatureInfo.FeelsLike,
            WeatherInfo = temperatureInfo.WeatherInfo.First().TypeOfWeather
        };
    }
}
