namespace WeatherAlertsBot.Helpers;

public static class TimeHelper
{
    public static TimeZoneInfo GetUkraineTimeZoneInfo()
    {
        const string ukraineTimeId = "FLE Standard Time";

        return TimeZoneInfo.FindSystemTimeZoneById(ukraineTimeId);
    }

    public static DateTime GetUkraineCurrentTime()
    {
        var timeZoneInfo = GetUkraineTimeZoneInfo();
        var ukraineTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo);

        return ukraineTime;
    }

    public static TimeSpan CalculateOffset(string timeZoneName, DateTime executeAt)
    {
        var dateTimeUtcNow = DateTime.Now;
        var userLocalTime = dateTimeUtcNow.ConvertToUserLocalTime(timeZoneName);

        var result = executeAt.ConvertToUserLocalTime(timeZoneName) - userLocalTime;
        return result;
    }
}