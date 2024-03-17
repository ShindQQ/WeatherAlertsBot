using TimeZoneConverter;

namespace WeatherAlertsBot.Helpers;

public static class DateTimeExtension
{
    public static DateTime ConvertToUserLocalTime(this DateTime utcDateTime, string userTimeZone)
    {
        if (string.IsNullOrEmpty(userTimeZone))
            userTimeZone = "Europe/Kiev";
        return TimeZoneInfo.ConvertTime(utcDateTime, TZConvert.GetTimeZoneInfo(userTimeZone));
    }

    public static DateTime ConvertToTimeZone(this DateTime utcDateTime, TimeZoneInfo timeZoneInfo)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZoneInfo);
    }

    public static void CheckTimeZone(this string timeZone)
    {
        TimeZoneInfo.ConvertTime(DateTime.UtcNow, TZConvert.GetTimeZoneInfo(timeZone));
    }
}