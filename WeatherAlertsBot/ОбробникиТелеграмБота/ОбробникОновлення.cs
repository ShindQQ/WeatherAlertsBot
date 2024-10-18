using System.Globalization;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using WeatherAlertsBot.DAL.Entities;
using WeatherAlertsBot.Помічники;
using WeatherAlertsBot.OpenWeatherApi;
using WeatherAlertsBot.ОбробникиЗапитів;
using WeatherAlertsBot.РосійськийКорабль;
using WeatherAlertsBot.РосійськийКорабль.LiquidationsInfo;
using WeatherAlertsBot.СервісиКористувача;

namespace WeatherAlertsBot.ОбробникиТелеграмБота;

/// <summary>
///     Telegram Bot Update Handler
/// </summary>
public sealed class ОбробникОновлення : ІОбробникОновлення
{
    private static readonly char[] Роздільники = [' '];

    /// <summary>
    ///     A client interface to use Telegram Bot API
    /// </summary>
    private readonly ITelegramBotClient _клієнтБота;

    /// <summary>
    ///     Cancellation Token
    /// </summary>
    private readonly CancellationTokenSource _джерелоЖетонівВідміни;

    /// <summary>
    ///     Subscriber service to work with db
    /// </summary>
    private readonly ІРепозіторійПідписника _репозіторійПідписника;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="клентТелеграмБота">A client interface to use Telegram Bot API</param>
    /// <param name="репозіторійПідписника">Service for work with db context</param>
    /// <param name="джерелоТокенівВідміни">Cancellation Token Source</param>
    public ОбробникОновлення(
        ITelegramBotClient клентТелеграмБота,
        ІРепозіторійПідписника репозіторійПідписника,
        CancellationTokenSource джерелоТокенівВідміни)
    {
        _клієнтБота = клентТелеграмБота;
        _репозіторійПідписника = репозіторійПідписника;
        _джерелоЖетонівВідміни = джерелоТокенівВідміни;
    }

    /// <summary>
    ///     Handling user message
    /// </summary>
    /// <param name="оновлення">Variable for handling user chat info and messages</param>
    /// <returns>Task</returns>
    public async Task ОбробитиПовідомленняАсінх(Update оновлення)
    {
        var повідомленняКористувача = оновлення.Message;

        if (повідомленняКористувача is null)
            return;

        var текстПовідомленняКористувача = повідомленняКористувача.Text;
        var идЧата = повідомленняКористувача.Chat.Id;

        if (текстПовідомленняКористувача is not null)
        {
            var обробленийТекст = текстПовідомленняКористувача
                .Replace("*", @"\*")
                .Replace(",", @"\,")
                .Replace(",", @"\,")
                .Replace("~", @"\~")
                .Replace("`", @"\`")
                .Replace(">", @"\>")
                .Replace("<", @"\<")
                .Replace("#", @"\#")
                .Replace("+", @"\+")
                .Replace("=", @"\=")
                .Replace("|", @"\|")
                .Replace("{", @"\}")
                .Replace("{", @"\{")
                .Replace("!", @"\!")
                .Replace(".", @"\.")
                .Replace("-", @"\-");
            await ОбробитиКомандуПовідомлення(идЧата, обробленийТекст);
        }

        
        var місцезнаходженняКористувачаПовідомлення = повідомленняКористувача.Location;

        if (місцезнаходженняКористувачаПовідомлення is not null)
            await ОбробитиМісцезнаходженняПовідомленняАсінх(идЧата, місцезнаходженняКористувачаПовідомлення);

        if (текстПовідомленняКористувача is null && місцезнаходженняКористувачаПовідомлення is null && повідомленняКористувача.Type == MessageType.Text)
            await ОбробитиПовідомленняПомилки(идЧата);
    }

    /// <summary>
    ///     Handling sending notifications to users
    /// </summary>
    /// <returns>Task</returns>
    public async Task ОбробитиПовідомленняПідпісниківАсінх()
    {
        var підпісники = await _репозіторійПідписника.ВзятиПідпісниківАсінх();

        foreach (var підпісник in підпісники)
        foreach (var командаПідпісника in підпісник.Команди)
            await ОбробитиКомандуПовідомлення(підпісник.ИдЧата, командаПідпісника.НазваКоманди);
    }

    /// <summary>
    ///     Handling user`s commands
    /// </summary>
    /// <param name="идЧата">User chat id</param>
    /// <param name="повідомленняКористувача">Command sent by user</param>
    /// <returns>Command for user`s request</returns>
    private Task ОбробитиКомандуПовідомлення(long идЧата, string повідомленняКористувача)
    {
        return повідомленняКористувача switch
        {
            КомандиБота.КомандаСтарт => ОбробитиПовідомленняПомилки(идЧата),
            КомандиБота.КомандаДопомога => ОбробитиПовідомленняПомилки(идЧата),
            _ when повідомленняКористувача.StartsWith(КомандиБота.КомандаПрогнозПогоди) => ОбробитиПовідомленняПрогнозПогодиАсінх(
                идЧата, повідомленняКористувача),
            _ when повідомленняКористувача.StartsWith(КомандиБота.КомандаПогода) => ОбробитиПовідомленняПогодиАсінх(идЧата, повідомленняКористувача),
            _ when повідомленняКористувача.StartsWith(КомандиБота.КомандаКартаТривогСтікер) => ОбробитиІнформаціюСтікераТривоги(идЧата),
            _ when повідомленняКористувача.StartsWith(КомандиБота.КомандаКартаТривог) => ОбробитиІнформаціюФотоТривоги(идЧата),
            _ when повідомленняКористувача.StartsWith(КомандиБота.КомандаПовідомленняВтрат) => ОбробитиІнформаціюРосійськогоВторгнення(идЧата),
            _ when повідомленняКористувача.StartsWith(КомандиБота.КомандаВзятиСписокПідпісок) =>
                ОбробитиІнфомаціюСпискаПідпісниківАсінх(идЧата),
            _ when повідомленняКористувача.StartsWith(КомандиБота.КомандаПідписатися) => ОбробитиПовідомленняПідписатисяАсінх(идЧата,
                повідомленняКористувача),
            _ when повідомленняКористувача.StartsWith(КомандиБота.КомандаВідписатися) => ОбробитиПовідомленняВідписатисяАсінх(идЧата,
                повідомленняКористувача),
            _ when повідомленняКористувача.StartsWith(КомандиБота.Нагадування) => ОбробитиПовідомленняНалагоджуванняНагадуванняАсінх(идЧата,
                повідомленняКористувача),
            _ => Task.CompletedTask
        };
    }

    /// <summary>
    ///     Handling subscription string results
    /// </summary>
    /// <param name="повідомленняКористувача">Message sent by user</param>
    /// <returns>Command for editing db</returns>
    private static string ОбробитиПовідоленняПідписатися(string повідомленняКористувача)
    {
        return повідомленняКористувача.Trim().Split(' ', 2) switch
        {
            _ when повідомленняКористувача.Equals(КомандиБота.КомандаПідписатисяНаПовідомленняВтрат) ||
                   повідомленняКористувача.Equals(КомандиБота.КомандаВідписатисяВідПовідомленьВтрат) => КомандиБота.КомандаПовідомленняВтрат,
            [{ } командаКористувача, { } назваМістаКористувача] when командаКористувача.StartsWith(КомандиБота
                                                         .КомандаПідписатисяНаПрогнозПогоди) ||
                                                     командаКористувача.StartsWith(КомандиБота
                                                         .КомандаВідписатисяВідПрогнозаПогоди)
                => КомандиБота.КомандаПрогнозПогоди + " " + назваМістаКористувача,
            _ => string.Empty
        };
    }

    /// <summary>
    ///     Handling receiving subscriber commands list
    /// </summary>
    /// <param name="идЧата">User chat id</param>
    /// <returns>List of user`s commands</returns>
    private async Task ОбробитиІнфомаціюСпискаПідпісниківАсінх(long идЧата)
    {
        var підпісник = await _репозіторійПідписника.ЗнайтиПідпісникаАсінх(идЧата);
        var повідомлення = "You are not subscribed to any services yet!";

        if (підпісник is not null)
            повідомлення = "Your subscription list:\n" +
                      string.Join("\n", підпісник.Команди.Select(command => $"{command.НазваКоманди}"));

        await ОбробитиТекстовеПовідомленняАсінх(идЧата, $"`{повідомлення}`");
    }

    /// <summary>
    ///     Handling user`s commands
    /// </summary>
    /// <param name="идЧата">User chat id</param>
    /// <param name="повідомленняКористувача">Command sent by user</param>
    /// <returns>Message with subscription status</returns>
    public async Task ОбробитиПовідомленняПідписатисяАсінх(long идЧата, string повідомленняКористувача)
    {
        var команда = ОбробитиПовідоленняПідписатися(повідомленняКористувача);

        if (string.IsNullOrEmpty(команда))
        {
            await ОбробитиПовідомленняПомилки(идЧата);
            return;
        }

        await ОбробитиТекстовеПовідомленняАсінх(идЧата,
            $"`{СгенеруватиПовідомленняДляРезультатуПідписки(
                await _репозіторійПідписника.ДодатиПідпісникаАсінх(new Підписник { ИдЧата = идЧата }, команда))}`");
    }

    /// <summary>
    ///     Handling user`s commands
    /// </summary>
    /// <param name="идЧата">User chat id</param>
    /// <param name="повідомленняКористувача">Command sent by user</param>
    /// <returns>Message with subscription status</returns>
    public async Task ОбробитиПовідомленняВідписатисяАсінх(long идЧата, string повідомленняКористувача)
    {
        var команда = ОбробитиПовідоленняПідписатися(повідомленняКористувача);

        if (string.IsNullOrEmpty(команда))
        {
            await ОбробитиПовідомленняПомилки(идЧата);
            return;
        }

        await ОбробитиТекстовеПовідомленняАсінх(идЧата,
            $"`{СгенеруватиПовідомленняДляРезультатуПідписки(
                await _репозіторійПідписника.ВидалитиКомандуВідПідпісникаАсінх(идЧата, команда))}`");
    }

    /// <summary>
    ///     Generating message response for user on result of affected rows in table
    /// </summary>
    /// <param name="кількістьЗачепленихРядків">Amount of affected rows</param>
    /// <returns>String with message for user</returns>
    public static string СгенеруватиПовідомленняДляРезультатуПідписки(int кількістьЗачепленихРядків)
    {
        return кількістьЗачепленихРядків == 0 ? "Operation unsuccessful!" : "Operation successful!";
    }

    /// <summary>
    ///     Handling /weather [city_name] message
    /// </summary>
    /// <param name="chatId">User chat id</param>
    /// <param name="userMessageText">Message sent by user</param>
    /// <returns>Sent weather to the user</returns>
    private async Task ОбробитиПовідомленняПогодиАсінх(long chatId, string userMessageText)
    {
        var відповідьПогодиДляКористувача = await WeatherHandler.SendWeatherByUserMessageAsync(userMessageText);
        var повідомленняПомилки = відповідьПогодиДляКористувача.ErrorMessage;

        if (!string.IsNullOrEmpty(повідомленняПомилки))
        {
            await ОбробитиТекстовеПовідомленняАсінх(chatId, $"`{повідомленняПомилки}`");
            return;
        }

        await ОбробитиФотоПовідомленняАсінх(chatId, WeatherImageGenerator.GenerateCurrentWeatherImage(відповідьПогодиДляКористувача),
            $"""
             `Current weather in {відповідьПогодиДляКористувача.CityName} is {відповідьПогодиДляКористувача.Temperature} °C.
             Feels like {відповідьПогодиДляКористувача.FeelsLike} °C. {відповідьПогодиДляКористувача.TypeOfWeather}.`
             """);
    }


    /// <summary>
    ///     Handling /remind [time as hours:minutes] [text] message setup
    /// </summary>
    /// <param name="chatId">User chat id</param>
    /// <param name="userMessageText">Message sent by user</param>
    /// <returns>Sent weather to the user</returns>
    private async Task ОбробитиПовідомленняНалагоджуванняНагадуванняАсінх(long chatId, string userMessageText)
    {
        try
        {
            var частини = userMessageText.Split(Роздільники, StringSplitOptions.RemoveEmptyEntries);

            var час = DateTime.ParseExact(частини[1] + ' ' + частини[2], "d/M H:mm", CultureInfo.InvariantCulture);
            
            var сдвиг = ПомічникЧасу.ПорахуватиЗдвиг(час);
            
            if (сдвиг.TotalSeconds < 0)
            {
                await ОбробитиТекстовеПовідомленняАсінх(chatId, "Time cannot be in the past");
                return;
            }
            
            var текст = string.Join(" ", частини.Skip(3));
            текст = WebUtility.UrlEncode(текст);
            
            Task.Run(() => ОбробитиПовідомленняНагадуванняАсінх(chatId, текст, сдвиг));
        }
        catch
        {
            await ОбробитиПовідомленняПомилки(chatId);
        }
    }

    /// <summary>
    ///     Handling /remind [time as hours:minutes] [text] message
    /// </summary>
    /// <param name="идЧата">User chat id</param>
    /// <param name="текстПовідомленняКористувача">Message sent by user</param>
    /// <param name="здвиг">Offset</param>
    /// <returns>Sent weather to the user</returns>
    public async Task ОбробитиПовідомленняНагадуванняАсінх(long идЧата, string текстПовідомленняКористувача,
        TimeSpan здвиг)
    {
        try
        {
            текстПовідомленняКористувача = WebUtility.UrlDecode(текстПовідомленняКористувача);
            var потребуєКрапки = текстПовідомленняКористувача.Length > 30;
            var скороченеПовідомлення = текстПовідомленняКористувача[..Math.Min(текстПовідомленняКористувача.Length, 30)];

            if(потребуєКрапки)
                скороченеПовідомлення += @"\.\.\.";
            
            var текстПовідомлення = $"""
                                    {скороченеПовідомлення}
                                    {здвиг.Hours:00}:{здвиг.Minutes:00}
                                    """;

            var идПовідомлення = await ОбробитиТекстовеПовідомленняАсінх(идЧата, текстПовідомлення, ParseMode.Html);

            скороченеПовідомлення = скороченеПовідомлення.Replace(@"\.\.\.", "...");
            
            await _клієнтБота.PinChatMessageAsync(идЧата, идПовідомлення);

            while (здвиг.Hours > 0 || здвиг.Minutes > 0)
            {
                текстПовідомлення = $"""
                                    {скороченеПовідомлення}
                                    {здвиг.Hours:00}:{здвиг.Minutes:00}
                                    """;

                try
                {
                    await _клієнтБота.EditMessageTextAsync(идЧата, идПовідомлення, текстПовідомлення);
                }
                catch
                {
                    // ignored
                }

                await Task.Delay(60000);
                здвиг += TimeSpan.FromMinutes(-1);
            }

            await _клієнтБота.DeleteMessageAsync(идЧата, идПовідомлення);

            await ОбробитиТекстовеПовідомленняАсінх(идЧата, текстПовідомленняКористувача);
        }
        catch
        {
            // ignored
        }
    }

    /// <summary>
    ///     Handling /weather_forecast [city_name] message
    /// </summary>
    /// <param name="идЧата">User chat id</param>
    /// <param name="текстПовідомленняКористувача">Message sent by user</param>
    /// <returns>Sent weather forecast to the user</returns>
    private async Task ОбробитиПовідомленняПрогнозПогодиАсінх(long идЧата, string текстПовідомленняКористувача)
    {
        var відповідьПогодиДляКористувача = await WeatherHandler.SendWeatherForecastByUserMessageAsync(текстПовідомленняКористувача);
        var повідомленняПомилки = відповідьПогодиДляКористувача.ErrorMessage;

        if (!string.IsNullOrEmpty(повідомленняПомилки))
        {
            await ОбробитиТекстовеПовідомленняАсінх(идЧата, $"`{повідомленняПомилки}`");

            return;
        }

        await ОбробитиФотоПовідомленняАсінх(идЧата, WeatherImageGenerator.GenerateWeatherForecastImage(
                відповідьПогодиДляКористувача),
            $"`Current weather in {відповідьПогодиДляКористувача.WeatherForecastCity.CityName} for next 24 hours:\n\n"
            + string.Join("\n\n", відповідьПогодиДляКористувача.WeatherForecastHoursData.Select(weatherData =>
                $"""
                 Time: {weatherData.Date[..^3]}:
                 Temperature: {weatherData.WeatherForecastTemperatureData.Temperature:N2} °C.
                 Feels like {weatherData.WeatherForecastTemperatureData.FeelsLike:N2} °C.
                 Humidity {weatherData.WeatherForecastTemperatureData.Humidity}%.
                 {weatherData.WeatherForecastCurrentWeather.First().TypeOfWeather}.
                 """)) + "`");
    }

    /// <summary>
    ///     Handling user location message
    /// </summary>
    /// <param name="идЧата">>User chat id</param>
    /// <param name="місцезнаходженняКористувача">Location sent by user</param>
    /// <returns>Task with weather location</returns>
    private async Task ОбробитиМісцезнаходженняПовідомленняАсінх(long идЧата, Location місцезнаходженняКористувача)
    {
        var відповідьПогодиДляКористувача = await WeatherHandler.SendWeatherByUserLocationAsync(місцезнаходженняКористувача);

        await ОбробитиТекстовеПовідомленняАсінх(идЧата,
            $"""
             `Current weather in {відповідьПогодиДляКористувача.CityName} is {відповідьПогодиДляКористувача.Temperature:N2} °C.
             Feels like {відповідьПогодиДляКористувача.FeelsLike:N2} °C. {відповідьПогодиДляКористувача.TypeOfWeather}.`
             """);
    }

    /// <summary>
    ///     Handling error message type
    /// </summary>
    /// <param name="идЧата">User chat id</param>
    /// <returns>Task with error message</returns>
    private Task ОбробитиПовідомленняПомилки(long идЧата)
    {
        return ОбробитиТекстовеПовідомленняАсінх(идЧата,
            $"""
             Hello\!
             To receive weather by city name send me: `{КомандиБота.КомандаПогода}` \[city\_name\]\!
             To receive weather forecast by city name send me: `{КомандиБота.КомандаПрогнозПогоди}` \[city\_name\]\!
             Or just send me your location for receiving current weather\!
             For map of alerts use `{КомандиБота.КомандаКартаТривог}`\!
             To see russian losses use `{КомандиБота.КомандаПовідомленняВтрат}`\!
             For subscribing on `{КомандиБота.КомандаПовідомленняВтрат}` command `{КомандиБота.КомандаПідписатисяНаПовідомленняВтрат}`\!
             For unsubscribing from `{КомандиБота.КомандаПовідомленняВтрат}` command `{КомандиБота.КомандаВідписатисяВідПовідомленьВтрат}`\!
             For subscribing on `{КомандиБота.КомандаПрогнозПогоди}` command `{КомандиБота.КомандаПідписатисяНаПрогнозПогоди}` \[city\_name\]\!
             For unsubscribing from `{КомандиБота.КомандаПрогнозПогоди}` command `{КомандиБота.КомандаВідписатисяВідПрогнозаПогоди}` \[city\_name\]\!
             For receiving list of all your subscriptions send me `{КомандиБота.КомандаВзятиСписокПідпісок}`\!
             Setting up reminder: `{КомандиБота.Нагадування}` \[time as day:\month hours\:minutes\] \[text\]
             My GitHub: `https://github.com/ShindQQ/WeatherAlertsBot`
             """);
    }

    /// <summary>
    ///     Receiving info about liquidations in russian invasion
    /// </summary>
    /// <param name="идЧата">User chat id</param>
    private async Task ОбробитиІнформаціюРосійськогоВторгнення(long идЧата)
    {
        await ОбробитиТекстовеПовідомленняАсінх(идЧата,
            (await ОбробникАПІЗапитів.ВзятиВідгукВідАпіАсінх<RussianInvasion>(ПосиланняАПІ.RussianWarshipUrl))
            !.RussianWarshipInfo.ToString());
    }

    /// <summary>
    ///     Receiving info about alerts in Ukraine regions
    /// </summary>
    /// <param name="идЧата">User chat id</param>
    /// <returns>Map with alerts and list of regions to the user</returns>
    private async Task ОбробитиІнформаціюФотоТривоги(long идЧата)
    {
        var регіони = await ОбробникАПІЗапитів.ВзятиВідповідьДляТривогЗакешованоАсінх();

        await ОбробитиФотоПовідомленняАсінх(идЧата, ГенераторКартиТривог.НамалюватиКартуТривог(регіони, false),
            "`Current alerts in Ukraine:\n" + string.Join("\n",
                регіони.Where(region => region.Value.Включена)
                    .Select(region => $"🚨 {region.Key.Trim('\'')};")) + "`");
    }

    /// <summary>
    ///     Receiving info about alerts in Ukraine regions with sticker
    /// </summary>
    /// <param name="идЧата">User chat id</param>
    /// <returns>Map with alerts and list of regions to the user</returns>
    private async Task ОбробитиІнформаціюСтікераТривоги(long идЧата)
    {
        var регіони = await ОбробникАПІЗапитів.ВзятиВідповідьДляТривогЗакешованоАсінх();

        await ОбробитиСтікерАсінх(идЧата, ГенераторКартиТривог.НамалюватиКартуТривог(регіони));
    }

    /// <summary>
    ///     Handling sending photo message with caption
    /// </summary>
    /// <param name="идЧата">Id og the chat to send to</param>
    /// <param name="байти">Bytes array (photo)</param>
    /// <param name="повідомленняКористувачу">Message which will be sent or not(caption)</param>
    /// <returns>Sent photo message with caption or not</returns>
    private async Task ОбробитиФотоПовідомленняАсінх(long идЧата, byte[] байти, string? повідомленняКористувачу)
    {
        try
        {
            await _клієнтБота.SendPhotoAsync(идЧата, InputFile.FromStream(new MemoryStream(байти)),
                caption: повідомленняКористувачу,
                parseMode: ParseMode.MarkdownV2,
                cancellationToken: _джерелоЖетонівВідміни.Token);
        }
        catch (ApiRequestException)
        {
        }
    }

    /// <summary>
    ///     Handling sending text messages because of Telegram.Bot.Exceptions
    /// </summary>
    /// <param name="идЧата">Id of the chat to send to</param>
    /// <param name="повідомленняКористувачу">Message which will be sent</param>
    /// <param name="режимРозбору">Parse mode</param>
    /// <returns>Sent text message</returns>
    public async Task<int> ОбробитиТекстовеПовідомленняАсінх(long идЧата, string повідомленняКористувачу,
        ParseMode режимРозбору = ParseMode.MarkdownV2)
    {
        try
        {
            var повідомлення = await _клієнтБота.SendTextMessageAsync(идЧата, повідомленняКористувачу,
                parseMode: режимРозбору,
                cancellationToken: _джерелоЖетонівВідміни.Token);

            return повідомлення.MessageId;
        }
        catch (ApiRequestException вик)
        {
            return -1;
        }
    }

    /// <summary>
    ///     Handling sending sticker messages because of Telegram.Bot.Exceptions
    /// </summary>
    /// <param name="идЧата">Id of the chat to send to</param>
    /// <param name="байти">Bytes array (photo)</param>
    /// <returns>Sent text message</returns>
    private async Task ОбробитиСтікерАсінх(long идЧата, byte[] байти)
    {
        try
        {
            await _клієнтБота.SendStickerAsync(идЧата, InputFile.FromStream(new MemoryStream(байти)),
                cancellationToken: _джерелоЖетонівВідміни.Token);
        }
        catch (ApiRequestException)
        {
        }
    }
}