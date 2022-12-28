using Telegram.Bot.Types;
using WeatherAlertsBot.Commands;
using WeatherAlertsBot.Configuration;
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
    private readonly APIsRequestsHandler _aPIsRequestsHandler = new();

    /// <summary>
    ///     Open Weather API url
    /// </summary>
    private const string _openWeatherAPIUrl = "https://api.openweathermap.org";

    /// <summary>
    ///     Open Weather API`s Current weather url
    /// </summary>
    private const string _currentWeatherUrl = "/data/2.5/weather";

    /// <summary>
    ///     Open Weather API`s Geo API url
    /// </summary>
    private const string _geoAPIUrl = "/geo/1.0/direct";

    /// <summary>
    ///     Constructor for api key inicializing 
    /// </summary>
    public WeatherHandler()
    {
        OpenWeatherApiKey = BotConfiguration.OpenWeatherApiKey;
    }

    /// <summary>
    ///     Receiving current weather info by lattitude and longitude
    /// </summary>
    /// <param name="lattitude">Location lattitude</param>
    /// <param name="longitude">Location longitude</param>
    /// <returns>WeatherForecastResult</returns>
    public async Task<WeatherForecastResult?> GetCurrentWeatherByCoordinatesAsync(float lattitude, float longitude)
    {
        string url = _openWeatherAPIUrl + _currentWeatherUrl + $"?lat={lattitude}&lon={longitude}&appid={OpenWeatherApiKey}";

        return await _aPIsRequestsHandler.GetResponseFromAPI<WeatherForecastResult>(url);
    }

    /// <summary>
    ///     Receiving coordinates of city
    /// </summary>
    /// <param name="cityName">Name of the city</param>
    /// <returns>CoordinatesInfo</returns>
    public async Task<IEnumerable<CoordinatesInfo>?> GetLattitudeAndLongitudeByCityNameAsync(string cityName)
    {
        string url = _openWeatherAPIUrl + _geoAPIUrl + $"?q={cityName}&appid={OpenWeatherApiKey}";

        return await _aPIsRequestsHandler.GetResponseFromAPI<IEnumerable<CoordinatesInfo>>(url);
    }

    /// <summary>
    ///     Generating data for response for the user
    /// </summary>
    /// <param name="userMessage">Message sent by the user</param>
    /// <returns>WeatherResponseForUser</returns>
    public async Task<WeatherResponseForUser> SendWeatherByUserMessageAsync(string userMessage)
    {
        if (userMessage.Equals(BotCommands.WeatherCommand))
        {
            return new WeatherResponseForUser { ErrorMessage = @"`Format of the input was wrong\!`" };
        }

        var splittedUserMessage = userMessage.Trim().Split(' ', 2);

        if (!splittedUserMessage[0].ToLower().StartsWith(BotCommands.WeatherCommand))
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
    public async Task<WeatherResponseForUser> SendWeatherByUserLocationAsync(Location userLocation)
    {
        var temperatureInfo = await GetCurrentWeatherByCoordinatesAsync((float)userLocation.Latitude, (float)userLocation.Longitude); ;

        if (temperatureInfo == null)
        {
            return new WeatherResponseForUser { ErrorMessage = @"`No data was found for your request\!`" };
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
