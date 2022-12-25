using Telegram.Bot.Types;
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
    private string openWeatherApiKey { get; init; }

    /// <summary>
    ///     Class of logic for calling APIs
    /// </summary>
    private readonly APIsRequestsHandler aPIsRequestsHandler = new();

    /// <summary>
    ///     String for weather command
    /// </summary>
    private const string WeatherCommand = "/weather";

    /// <summary>
    ///     Open Weather API url
    /// </summary>
    private const string OpenWeatherAPIUrl = "https://api.openweathermap.org";

    /// <summary>
    ///     Open Weather API`s Current weather url
    /// </summary>
    private const string CurrentWeatherUrl = "/data/2.5/weather";

    /// <summary>
    ///     Open Weather API`s Geo API url
    /// </summary>
    private const string GeoAPIUrl = "/geo/1.0/direct";

    /// <summary>
    ///     Constructor for api key inicializing 
    /// </summary>
    public WeatherHandler()
    {
        openWeatherApiKey = BotConfiguration.OpenWeatherApiKey;
    }

    /// <summary>
    ///     Receiving current weather info by lattitude and longitude
    /// </summary>
    /// <param name="lattitude">Location lattitude</param>
    /// <param name="longitude">Location longitude</param>
    /// <returns>WeatherForecastResult</returns>
    public async ValueTask<WeatherForecastResult?> GetCurrentWeatherByCoordinatesAsync(float lattitude, float longitude)
    {
        string url = OpenWeatherAPIUrl + CurrentWeatherUrl + $"?lat={lattitude}&lon={longitude}&appid={openWeatherApiKey}";

        return await aPIsRequestsHandler.GetResponseFromAPI<WeatherForecastResult>(url);
    }

    /// <summary>
    ///     Receiving coordinates of city
    /// </summary>
    /// <param name="cityName">Name of the city</param>
    /// <returns>CoordinatesInfo</returns>
    public async ValueTask<IEnumerable<CoordinatesInfo>?> GetLattitudeAndLongitudeByCityNameAsync(string cityName)
    {
        string url = OpenWeatherAPIUrl + GeoAPIUrl + $"?q={cityName}&appid={openWeatherApiKey}";

        return await aPIsRequestsHandler.GetResponseFromAPI<IEnumerable<CoordinatesInfo>>(url);
    }

    /// <summary>
    ///     Generating data for response for the user
    /// </summary>
    /// <param name="userMessage">Message sent by the user</param>
    /// <returns>WeatherResponseForUser</returns>
    public async ValueTask<WeatherResponseForUser> SendWeatherByUserMessageAsync(string userMessage)
    {
        if (userMessage.Equals(WeatherCommand))
        {
            return new WeatherResponseForUser { ErrorMessage = @"`Format of the input was wrong\!`" };
        }

        var splittedUserMessage = userMessage.Trim().Split(' ', 2);

        if (!splittedUserMessage[0].ToLower().StartsWith(WeatherCommand))
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
            Temperature = temperatureInfo.TemperatureInfo.Temperature,
            FeelsLike = temperatureInfo.TemperatureInfo.FeelsLike,
            WeatherInfo = temperatureInfo.WeatherInfo.First().TypeOfWeather
        };
    }
}
