using System.Net.Http.Json;
using WeatherAlertsBot.Помічники;
using WeatherAlertsBot.РосійськийКорабль.AlarmsInfo;

namespace WeatherAlertsBot.ОбробникиЗапитів;

/// <summary>
///     Class for logic of calling APIs
/// </summary>
public sealed class ОбробникАПІЗапитів
{
    /// <summary>
    ///     HttpClient
    /// </summary>
    private static readonly HttpClient HttpКлієнт = new();

    /// <summary>
    ///     Time of the last request for alerts
    /// </summary>
    private static DateTime ОстаннійЗапитТривоги { get; set; } = DateTime.MinValue;

    /// <summary>
    ///     Data of the last request for alerts
    /// </summary>
    private static Dictionary<string, ОбєктОбласті> ОстаннеЗначенняТривог { get; set; } = new();

    /// <summary>
    ///     Cached response for alerts data
    /// </summary>
    /// <returns>Dictionary where key represents name of the region and value which is data about alerts</returns>
    public static async Task<Dictionary<string, ОбєктОбласті>> ВзятиВідповідьДляТривогЗакешованоАсінх()
    {
        var датаІЧас = DateTime.UtcNow;
        if ((датаІЧас - ОстаннійЗапитТривоги).TotalMinutes >= 1)
        {
            ОстаннійЗапитТривоги = DateTime.UtcNow;
            ОстаннеЗначенняТривог = (await ВзятиВідгукВідАпіАсінх<ІнформаціяТривогОбластей>(ПосиланняАПІ.AlarmsInUkraineInfoUrl
                                                                              + $"&dt{датаІЧас.Day}{датаІЧас.Month}{датаІЧас.Year}"))
                !.Області;
        }

        return ОстаннеЗначенняТривог;
    }

    /// <summary>
    ///     Receiving response from OpenWeatherAPI and from Russian Warship APIs
    /// </summary>
    /// <typeparam name="T">Describing type for deserializing</typeparam>
    /// <param name="url">Url for request</param>
    /// <returns>T as deserialized response from request</returns>
    /// <exception cref="HttpRequestException">If response`s status code isn`t 200</exception>
    public static async Task<T?> ВзятиВідгукВідАпіАсінх<T>(string url)
    {
        var відгук = await HttpКлієнт.GetAsync(url);

        if (!відгук.IsSuccessStatusCode) return default;

        return await відгук.Content.ReadFromJsonAsync<T>();
    }
}