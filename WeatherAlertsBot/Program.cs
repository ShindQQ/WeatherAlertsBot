using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using WeatherAlertsBot.BackgroundServices;
using WeatherAlertsBot.Configuration;
using WeatherAlertsBot.DAL.Context;
using WeatherAlertsBot.TelegramBotHandlers;
using WeatherAlertsBot.UserServices;

var botClient = new TelegramBotClient(BotConfiguration.BotAccessToken);

using CancellationTokenSource cancellationTokenSource = new();

var host = Host.CreateDefaultBuilder(args)
   .ConfigureServices((hostContext, services) =>
   {
       services.AddHostedService<BotHostedService>();
       services.AddSingleton<ITelegramBotClient>(botClient);
       services.AddScoped<UpdateHandler>();
       services.AddSingleton(cancellationTokenSource);
       services.AddScoped<SubscriberService>().AddDbContext<BotContext>(options =>
            options.UseMySql(hostContext.Configuration.GetConnectionString("DbConnection"),
            new MySqlServerVersion(new Version(8, 0, 30))));
   }).Build();


botClient.StartReceiving(
    HandleUpdateAsync,
    HandlePollingErrorAsync,
    new(),
    cancellationTokenSource.Token
    );

await host.RunAsync();

// Wait for eternity
await Task.Delay(-1);

cancellationTokenSource.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    var updateHandler = host.Services.GetRequiredService<UpdateHandler>();

    await updateHandler.HandleMessageAsync(update);
}

async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    await Task.CompletedTask;
}