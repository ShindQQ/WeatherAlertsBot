using System.Net.Http.Json;

namespace WeatherAlertsBot.Requesthandlers;

/// <summary>
///     Class for logic of calling APIs
/// </summary>
public sealed class APIsRequestsHandler
{
    /// <summary>
    ///     HttpClient 
    /// </summary>
    private static readonly HttpClient HttpClient = new();

    /// <summary>
    ///     Receiving response from OpenWeatherAPI and from Russian Warship APIs
    /// </summary>
    /// <typeparam name="T">Describing type for deserializing</typeparam>
    /// <param name="url">Url for request</param>
    /// <returns>T as deserialized response from request</returns>
    /// <exception cref="HttpRequestException">If response`s status code isn`t 200</exception>
    public async ValueTask<T> GetResponseFromAPI<T>(string url)
    {
        var response = await HttpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException("Some troubles happened with your request!");
        }

        return await response.Content.ReadFromJsonAsync<T>();
    }
}
