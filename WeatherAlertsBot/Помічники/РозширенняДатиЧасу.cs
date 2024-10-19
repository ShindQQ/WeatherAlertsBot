using TimeZoneConverter;

namespace WeatherAlertsBot.Помічники;

public static class РозширенняДатиЧасу
{
    public static DateTime СконвертуватиДоЛокальногоЧасуКористувача(this DateTime utcДатаЧас, string часовийПоясКористувача)
    {
        if (string.IsNullOrEmpty(часовийПоясКористувача))
            часовийПоясКористувача = "Europe/Kiev";
        return TimeZoneInfo.ConvertTime(utcДатаЧас, TZConvert.GetTimeZoneInfo(часовийПоясКористувача));
    }

    public static DateTime СконвертуватиДоЧасовогоПоясу(this DateTime utcДатаЧас, TimeZoneInfo інформаціяЧасовогоПоясу)
    {
        return TimeZoneInfo.ConvertTime(utcДатаЧас, інформаціяЧасовогоПоясу);
    }
}