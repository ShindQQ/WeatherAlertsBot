using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using WeatherAlertsBot.Configuration;
using WeatherAlertsBot.TelegramBotHandlers;
using WeatherAlertsBot.UserServices;

namespace WeatherAlertsBot.BackgroundServices;

/// <summary>
///     Backgorund service for sending notifications to subscribed users
/// </summary>
public sealed class BotHostedService : BackgroundService
{
    /// <summary>
    ///     Handler for sending messages to users
    /// </summary>
    private UpdateHandler _updateHandler = new(new TelegramBotClient(BotConfiguration.BotAccessToken), new CancellationToken());

    /// <summary>
    ///     Execution of background subscriber service
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var subscribers = await SubscriberService.GetSubscribersAsync();

            subscribers.ForEach(subscriber => subscriber.Commands
                .ForEach(command => _updateHandler.HandleCommandMessage(subscriber.ChatId, command.CommandName)));

            await Task.Delay(86400000);
        }
    }
}
