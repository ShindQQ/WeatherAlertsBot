namespace WeatherAlertsBot.Помічники;

public static class ПомічникЧасу
{
    public static TimeZoneInfo ВзятиІнформаціюУкраїнськогоЧасовигоПоясу()
    {
        const string идУкраїнськогоЧасу = "Europe/Kiev";

        return TimeZoneInfo.FindSystemTimeZoneById(идУкраїнськогоЧасу);
    }

    public static DateTime ВзятиПоточнийУкраїнськийЧас()
    {
        var інформаціяЧасовогоПоясу = ВзятиІнформаціюУкраїнськогоЧасовигоПоясу();
        var українськийЧас = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, інформаціяЧасовогоПоясу);

        return українськийЧас;
    }

    public static TimeSpan ПорахуватиЗдвиг(DateTime виконаноВ)
    {
        var результат = виконаноВ.ToUniversalTime().AddHours(-3) - DateTime.UtcNow;
        return результат;
    }
}
