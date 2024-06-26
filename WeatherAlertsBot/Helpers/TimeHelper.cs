﻿namespace WeatherAlertsBot.Helpers;

public static class TimeHelper
{
    public static TimeZoneInfo GetUkraineTimeZoneInfo()
    {
        const string ukraineTimeId = "Europe/Kiev";

        return TimeZoneInfo.FindSystemTimeZoneById(ukraineTimeId);
    }

    public static DateTime GetUkraineCurrentTime()
    {
        var timeZoneInfo = GetUkraineTimeZoneInfo();
        var ukraineTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo);

        return ukraineTime;
    }

    public static TimeSpan CalculateOffset(DateTime executeAt)
    {
        var result = executeAt.ToUniversalTime().AddHours(-3) - DateTime.UtcNow;
        return result;
    }
}
