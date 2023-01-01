using System.Net.Http.Json;
using WeatherAlertsBot.Helpers;
using WeatherAlertsBot.RussianWarship;
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
    ///     Time of the last request for liquidations
    /// </summary>
    private static DateTime LastLiquidationsRequest { get; set; } = DateTime.MinValue;

    /// <summary>
    ///     Data of the last request for liquidations
    /// </summary>
    private static RussianWarshipInfo LastLiquidationsValue { get; set; } = new();

    /// <summary>
    ///     Cached response for alerts data
    /// </summary>
    /// <returns>Dictionary where key represents name of the region and value which is data about alerts</returns>
    public static async Task<Dictionary<string, StateObject>> GetResponseForAlertsCachedAsync()
    {
        if ((DateTime.UtcNow - LastAlertsRequest).TotalMinutes >= 1)
        {
            LastAlertsRequest = DateTime.UtcNow;
            LastAlertsValue = (await GetResponseFromAPI<AlarmsStateInfo>(APIsLinks.AlarmsInUkraineInfoUrl))!.States;
        }

        return LastAlertsValue;
    }

    /// <summary>
    ///     Cached response for liquidations data
    /// </summary>
    /// <returns>Data about enemies looses</returns>
    public static async Task<RussianWarshipInfo> GetResponseForRussianWarshipInfoCachedAsync()
    {
        if ((DateTime.UtcNow - LastLiquidationsRequest).TotalDays >= 1)
        {
            LastLiquidationsRequest = DateTime.UtcNow;
            LastLiquidationsValue = (await GetResponseFromAPI<RussianInvasion>(APIsLinks.RussianWarshipUrl))!.RussianWarshipInfo;
        }

        return LastLiquidationsValue;
    }

    /// <summary>
    ///     Receiving response from OpenWeatherAPI and from Russian Warship APIs
    /// </summary>
    /// <typeparam name="T">Describing type for deserializing</typeparam>
    /// <param name="url">Url for request</param>
    /// <returns>T as deserialized response from request</returns>
    /// <exception cref="HttpRequestException">If response`s status code isn`t 200</exception>
    public static async Task<T?> GetResponseFromAPI<T>(string url)
    {
        var response = await HttpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            return default;
        }

        return await response.Content.ReadFromJsonAsync<T>();
    }
}
