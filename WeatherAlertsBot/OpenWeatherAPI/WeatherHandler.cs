using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using Telegram.Bot.Types;

namespace WeatherAlertsBot.OpenWeatherAPI;

public sealed class WeatherHandler
{
    private static readonly HttpClient HttpClient = new();

    private string OpenWeatherApiKey { get; init; }

    public WeatherHandler()
    {
        var configuration = new ConfigurationBuilder()
                 .AddJsonFile($"appsettings.json", true, true).Build();

        OpenWeatherApiKey = configuration["OpenWeatherApiKey"];
    }

    public async ValueTask<WeatherForecastResult> GetCurrentWeatherByCoordinatesAsync(float lattitude, float longitude)
    {
        string url = @$"https://api.openweathermap.org/data/2.5/weather?lat={lattitude}&lon={longitude}&appid={OpenWeatherApiKey}";

        return await GetResponseFromOpenWeatherAPI<WeatherForecastResult>(url);
    }

    public async ValueTask<IEnumerable<CoordinatesInfo>> GetLattitudeAndLongitudeByCityNameAsync(string cityName)
    {
        string url = @$"http://api.openweathermap.org/geo/1.0/direct?q={cityName}&appid={OpenWeatherApiKey}";

        return await GetResponseFromOpenWeatherAPI<IEnumerable<CoordinatesInfo>>(url);
    }

    public async ValueTask<T> GetResponseFromOpenWeatherAPI<T>(string url)
    {
        var response = await HttpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException("Some troubles happened with your request!");
        }

        return await response.Content.ReadFromJsonAsync<T>();
    }

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
            CityName = coordinatesInfoFirst.Name,
            Temperature = temperatureInfo.TemperatureInfo.Temperature - 273.15f,
            FeelsLike = temperatureInfo.TemperatureInfo.FeelsLike - 273.15f,
            Longitude = coordinatesInfoFirst.Longitude,
            Lattitude = coordinatesInfoFirst.Lattitude
        };
    }

    public async ValueTask<WeatherResponseForUser> SendWeatherByUserLocationAsync(Location userLocation)
    {
        var weatherResponseForUser = await GetCurrentWeatherByCoordinatesAsync((float)userLocation.Latitude, (float)userLocation.Longitude); ;

        return new WeatherResponseForUser
        {
            CityName = weatherResponseForUser.Name,
            Temperature = weatherResponseForUser.TemperatureInfo.Temperature - 273.15f,
            FeelsLike = weatherResponseForUser.TemperatureInfo.FeelsLike - 273.15f
        };
    }
}
