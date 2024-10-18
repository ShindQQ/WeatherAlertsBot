using Microsoft.Extensions.Configuration;

namespace WeatherAlertsBot.Configuration;

/// <summary>
///     Static class with configuration needed for bot
/// </summary>
public static class КонфігураціяБота
{
    /// <summary>
    ///     Api key for Open Weather API
    /// </summary>
    public static readonly string OpenWeatherApiKey;

    /// <summary>
    ///     Bot access token
    /// </summary>
    public static readonly string BotAccessToken;

    /// <summary>
    ///     Constructor
    /// </summary>
    static КонфігураціяБота()
    {
        var конфігурація = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true, true).Build();

        OpenWeatherApiKey = конфігурація["OpenWeatherApiKey"]!;
        BotAccessToken = конфігурація["BotAccessToken"]!;
    }
}