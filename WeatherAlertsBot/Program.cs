using Hangfire;
using Hangfire.MySql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using WeatherAlertsBot.BackgroundServices;
using WeatherAlertsBot.Configuration;
using WeatherAlertsBot.DAL.Contexts;
using WeatherAlertsBot.TelegramBotHandlers;
using WeatherAlertsBot.UserServices;
using IUpdateHandler = WeatherAlertsBot.TelegramBotHandlers.IUpdateHandler;

var telegramBotClient = new TelegramBotClient(BotConfiguration.BotAccessToken);

using CancellationTokenSource cancellationTokenSource = new();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;
        var dbConnectionString = configuration.GetConnectionString("DbConnection")!;

        services.AddSingleton<ITelegramBotClient>(telegramBotClient);
        services.AddScoped<IUpdateHandler, UpdateHandler>();
        services.AddSingleton(cancellationTokenSource);
        services.AddScoped<ISubscriberRepository, SubscriberRepository>().AddDbContext<BotContext>(options =>
            options.UseMySql(dbConnectionString,
                new MySqlServerVersion(new Version(8, 0, 30))));
        
        services.Configure<BackgroundJobOptions>(configuration.GetSection(BackgroundJobOptions.ConfigName));
        services.AddHostedService<NotifySubscribersJob>();

        services.AddHangfire(options => options
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSerializerSettings(new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            })
            .UseStorage(new MySqlStorage(dbConnectionString,
                new MySqlStorageOptions
                {
                    QueuePollInterval = TimeSpan.FromSeconds(10),
                    TablesPrefix = "Hangfire",
                    PrepareSchemaIfNecessary = true,
                    InvisibilityTimeout = TimeSpan.FromHours(24)
                })));
        
        GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });
        
        services.AddHangfireServer();
    }).Build();

telegramBotClient.StartReceiving(
    HandleUpdateAsync,
    HandlePollingErrorAsync,
    new ReceiverOptions(),
    cancellationTokenSource.Token
);

await host.RunAsync();

// Wait for eternity
await Task.Delay(-1);

cancellationTokenSource.Cancel();
return;

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    var updateHandler = host.Services.GetRequiredService<IUpdateHandler>();

    await updateHandler.HandleMessageAsync(update);
}

async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception,
    CancellationToken cancellationToken)
{
    var errorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(errorMessage);
    await Task.CompletedTask;
}