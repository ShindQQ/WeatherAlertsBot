﻿using Telegram.Bot.Types;
using WeatherAlertsBot.Configuration;
using WeatherAlertsBot.Helpers;
using WeatherAlertsBot.OpenWeatherApi.Models;
using WeatherAlertsBot.OpenWeatherApi.Models.CurrentWeather;
using WeatherAlertsBot.OpenWeatherApi.Models.GeoCodingAPI;
using WeatherAlertsBot.OpenWeatherApi.Models.WeatherForecast;
using WeatherAlertsBot.RequestHandlers;

namespace WeatherAlertsBot.OpenWeatherApi;

/// <summary>
///     Class which represents work with OpenWeatherAPI
/// </summary>
public static class WeatherHandler
{
    /// <summary>
    ///     Receiving current weather info by lattitude and longitude
    /// </summary>
    /// <param name="lattitude">Location lattitude</param>
    /// <param name="longitude">Location longitude</param>
    /// <returns>WeatherResult</returns>
    public static async Task<WeatherResult?> GetCurrentWeatherByCoordinatesAsync(float lattitude, float longitude)
    {
        var url = ApIsLinks.OpenWeatherApiUrl + ApIsLinks.CurrentWeatherUrl
                                              + $"?units=metric&lat={lattitude}&lon={longitude}&appid={BotConfiguration.OpenWeatherApiKey}";

        return await ApisRequestsHandler.GetResponseFromApiAsync<WeatherResult>(url);
    }

    /// <summary>
    ///     Receiving current weather forecast info by lattitude and longitude
    /// </summary>
    /// <param name="lattitude">Location lattitude</param>
    /// <param name="longitude">Location longitude</param>
    /// <returns>WeatherForecastResult</returns>
    public static async Task<WeatherForecastResult?> GetWeatherForecastByCoordinatesAsync(float lattitude,
        float longitude)
    {
        var url = ApIsLinks.OpenWeatherApiUrl + ApIsLinks.WeatherForecastUrl
                                              + $"?units=metric&cnt=8&lat={lattitude}&lon={longitude}&appid={BotConfiguration.OpenWeatherApiKey}";

        return await ApisRequestsHandler.GetResponseFromApiAsync<WeatherForecastResult>(url);
    }

    /// <summary>
    ///     Receiving coordinates of city
    /// </summary>
    /// <param name="cityName">Name of the city</param>
    /// <returns>CoordinatesInfo</returns>
    private static async Task<IEnumerable<CoordinatesInfo>?> GetLattitudeAndLongitudeByCityNameAsync(string cityName)
    {
        var url = ApIsLinks.OpenWeatherApiUrl + ApIsLinks.GeoAPIUrl +
                  $"?q={cityName}&appid={BotConfiguration.OpenWeatherApiKey}";

        return await ApisRequestsHandler.GetResponseFromApiAsync<IEnumerable<CoordinatesInfo>>(url);
    }

    /// <summary>
    ///     Receiving coordinates for user request
    /// </summary>
    /// <param name="userMessage">UserMessage including command and city name</param>
    /// <returns>Coordinates with longitude and latitude</returns>
    private static async Task<CoordinatesInfo?> GetUserCoordinatesAsync(string userMessage)
    {
        var splittedUserMessage = userMessage.Trim().Split(' ', 2);

        if (splittedUserMessage.Length != 2) return null;

        var coordinatesInfo = await GetLattitudeAndLongitudeByCityNameAsync(splittedUserMessage[1]);

        if (coordinatesInfo is null || !coordinatesInfo.Any()) return null;

        return coordinatesInfo.First();
    }

    /// <summary>
    ///     Generating data for response for the user by current weather requset
    /// </summary>
    /// <param name="userMessage">Message sent by the user</param>
    /// <returns>WeatherResponseForUser</returns>
    public static async Task<WeatherResponseForUser> SendWeatherByUserMessageAsync(string userMessage)
    {
        var coordinatesInfo = await GetUserCoordinatesAsync(userMessage);

        if (coordinatesInfo is null)
            return new WeatherResponseForUser { ErrorMessage = "Check correctness of your input!" };

        var temperatureInfo =
            await GetCurrentWeatherByCoordinatesAsync(coordinatesInfo.Latitude, coordinatesInfo.Longitude);

        return new WeatherResponseForUser
        {
            CityName = coordinatesInfo.CityName,
            Temperature = temperatureInfo!.TemperatureInfo.Temperature,
            FeelsLike = temperatureInfo.TemperatureInfo.FeelsLike,
            Longitude = coordinatesInfo.Longitude,
            Latitude = coordinatesInfo.Latitude,
            TypeOfWeather = temperatureInfo.WeatherInfo.First().TypeOfWeather,
            IconType = temperatureInfo.WeatherInfo.First().IconType
        };
    }

    /// <summary>
    ///     Generating data for response for the user by weather forecast requset
    /// </summary>
    /// <param name="userMessage">Message sent by the user</param>
    /// <returns>WeatherForecastResult</returns>
    public static async Task<WeatherForecastResult> SendWeatherForecastByUserMessageAsync(string userMessage)
    {
        var coordinatesInfo = await GetUserCoordinatesAsync(userMessage);

        if (coordinatesInfo is null)
            return new WeatherForecastResult { ErrorMessage = "No data was found for your request!" };

        var result = await GetWeatherForecastByCoordinatesAsync(coordinatesInfo.Latitude, coordinatesInfo.Longitude);

        result!.WeatherForecastCity.CityName = coordinatesInfo.CityName;

        return result;
    }

    /// <summary>
    ///     Receiving weather by user location
    /// </summary>
    /// <param name="userLocation">Location sent by user</param>
    /// <returns>WeatherResponseForUser</returns>
    public static async Task<WeatherResponseForUser> SendWeatherByUserLocationAsync(Location userLocation)
    {
        var temperatureInfo =
            await GetCurrentWeatherByCoordinatesAsync((float)userLocation.Latitude, (float)userLocation.Longitude);

        if (temperatureInfo is null)
            return new WeatherResponseForUser { ErrorMessage = "No data was found for your request!" };

        return new WeatherResponseForUser
        {
            CityName = temperatureInfo.Name,
            Temperature = temperatureInfo.TemperatureInfo.Temperature,
            FeelsLike = temperatureInfo.TemperatureInfo.FeelsLike,
            TypeOfWeather = temperatureInfo.WeatherInfo.First().TypeOfWeather
        };
    }
}