using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

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

    public async ValueTask<WeatherForecastResult> GetCurrentWeatherByCoordinatesAsync(double lattitude, double longitude)
    {
        string url = $"https://api.openweathermap.org/data/2.5/weather?lat={lattitude}&lon={longitude}&appid={OpenWeatherApiKey}";

        return await GetResponseFromOpenWeatherAPI<WeatherForecastResult>(url);
    }

    public async ValueTask<IEnumerable<CoordinatesInfo>> GetLattitudeAndLongitudeByCityNameAsync(string cityName)
    {
        string url = $"http://api.openweathermap.org/geo/1.0/direct?q={cityName}&appid={OpenWeatherApiKey}";

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
}
