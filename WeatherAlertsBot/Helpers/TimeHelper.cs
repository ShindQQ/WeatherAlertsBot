namespace WeatherAlertsBot.Helpers;

public static class TimeHelper
{
    public static TimeZoneInfo GetUkraineTimeZoneInfo()
    {
        const string jerusalemTimeZoneId = "FLE Standard Time";

        return TimeZoneInfo.FindSystemTimeZoneById(jerusalemTimeZoneId);
    }

    public static DateTime GetUkraineCurrentTime()
    {
        var timeZoneInfo = GetUkraineTimeZoneInfo();
        var jerusalemTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo);

        return jerusalemTime;
    }

    public static TimeSpan CalculateOffset(string timeZoneName, DateTime executeAt)
    {
        var dateTimeUtcNow = DateTime.UtcNow;
        var userLocalTime = dateTimeUtcNow.ConvertToUserLocalTime(timeZoneName);

        var result = executeAt.ToUniversalTime().ConvertToUserLocalTime(timeZoneName) - userLocalTime;
        return result;
    }
}