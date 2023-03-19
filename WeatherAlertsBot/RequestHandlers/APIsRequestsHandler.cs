using System.Net.Http.Json;
using WeatherAlertsBot.Helpers;
using WeatherAlertsBot.RussianWarship.AlarmsInfo;

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
    ///     Time of the last request for alerts
    /// </summary>
    private static DateTime LastAlertsRequest { get; set; } = DateTime.MinValue;

    /// <summary>
    ///     Data of the last request for alerts
    /// </summary>
    private static Dictionary<string, StateObject> LastAlertsValue { get; set; } = new();

    /// <summary>
    ///     Cached response for alerts data
    /// </summary>
    /// <returns>Dictionary where key represents name of the region and value which is data about alerts</returns>
    public static async Task<Dictionary<string, StateObject>> GetResponseForAlertsCachedAsync()
    {
        var states = (await GetResponseFromAPIAsync<AlarmsStateInfo>(APIsLinks.AlarmsInUkraineInfoUrl)).States;

        if ((DateTime.UtcNow - LastAlertsRequest).TotalMinutes >= 1)
        {
            LastAlertsRequest = DateTime.UtcNow;
            LastAlertsValue = states;
        }

        return LastAlertsValue;
    }

    /// <summary>
    ///     Receiving response from OpenWeatherAPI and from Russian Warship APIs
    /// </summary>
    /// <typeparam name="T">Describing type for deserializing</typeparam>
    /// <param name="url">Url for request</param>
    /// <returns>T as deserialized response from request</returns>
    /// <exception cref="HttpRequestException">If response`s status code isn`t 200</exception>
    public static async Task<T?> GetResponseFromAPIAsync<T>(string url)
    {
        var response = await HttpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            return default;
        }

        return await response.Content.ReadFromJsonAsync<T>();
    }
}
