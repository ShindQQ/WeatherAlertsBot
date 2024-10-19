using Hangfire;
using Hangfire.MySql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Мікрософт.Розширення.Хостінг;
using Система;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using WeatherAlertsBot.ФоновіСервіси;
using WeatherAlertsBot.Configuration;
using WeatherAlertsBot.DAL.Contexts;
using WeatherAlertsBot.ОбробникиТелеграмБота;
using WeatherAlertsBot.СервісиКористувача;
using ІОбробникОновлення = WeatherAlertsBot.ОбробникиТелеграмБота.ІОбробникОновлення;

var клієнтТелеграмБоту = new TelegramBotClient(КонфігураціяБота.BotAccessToken);

using CancellationTokenSource джерелоЖетонівВідміни = new();

var хост = Хост.СтворитиУсталенийБудівник(args)
    .СконфігуруватиСервіси((контекстХоста, сервіси) =>
    {
        var конфігурація = контекстХоста.Configuration;
        var бдРядокПідключення = конфігурація.GetConnectionString("DbConnection")!;

        сервіси.AddSingleton<ITelegramBotClient>(клієнтТелеграмБоту);
        сервіси.AddScoped<ІОбробникОновлення, ОбробникОновлення>();
        сервіси.AddSingleton(джерелоЖетонівВідміни);
        сервіси.AddScoped<ІРепозіторійПідписника, РепозіторійПідписника>().AddDbContext<КонтекстБота>(options =>
            options.UseMySql(бдРядокПідключення,
                new MySqlServerVersion(new Version(8, 0, 30))));
        
        сервіси.Configure<ОпціїФоновоїЗадачи>(конфігурація.GetSection(ОпціїФоновоїЗадачи.НазваКонфіга));
        сервіси.AddHostedService<ЗадачаПовідомленняПідписників>();

        сервіси.AddHangfire(options => options
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSerializerSettings(new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            })
            .UseStorage(new MySqlStorage(бдРядокПідключення,
                new MySqlStorageOptions
                {
                    QueuePollInterval = TimeSpan.FromSeconds(10),
                    TablesPrefix = "Hangfire",
                    PrepareSchemaIfNecessary = true,
                    InvisibilityTimeout = TimeSpan.FromHours(24)
                })));
        
        GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });
        
        сервіси.AddHangfireServer();
    }).Побудувати();

клієнтТелеграмБоту.StartReceiving(
    ОбробитиОновленняАсінх,
    ОбробитиПомилкуОпитуванняАсінх,
    new ReceiverOptions(),
    джерелоЖетонівВідміни.Token
);

await хост.RunAsync();

// Wait for eternity
await Task.Delay(-1);

джерелоЖетонівВідміни.Cancel();
return;

async Task ОбробитиОновленняАсінх(ITelegramBotClient клієнтБота, Update оновлення, CancellationToken жетонВідміни)
{
    var обробникОновлення = хост.Services.GetRequiredService<ІОбробникОновлення>();

    await обробникОновлення.ОбробитиПовідомленняАсінх(оновлення);
}

async Task ОбробитиПомилкуОпитуванняАсінх(ITelegramBotClient клієнтБота, Exception виключння,
    CancellationToken жетонВідміни)
{
    var повідомленняПроПомилку = виключння switch
    {
        ApiRequestException виключенняАпіЗапиту
            => $"Помилка Telegram API:\n[{виключенняАпіЗапиту.ErrorCode}]\n{виключенняАпіЗапиту.Message}",
        _ => виключння.ToString()
    };

    Консоль.НаписатиРядок(повідомленняПроПомилку);
    await Task.CompletedTask;
}