namespace WeatherAlertsBot.OpenWeatherAPI;

public sealed class WeatherResponseForUser
{
    public string CityName { get; set; } = string.Empty;

    public float Lattitude { get; set; }

    public float Longitude { get; set; }

    public float Temperature { get; set; }

    public float FeelsLike { get; set; }

    public string ErrorMessage { get; set; } = string.Empty;
}
