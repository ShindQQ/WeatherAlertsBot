using Microsoft.VisualBasic;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using WeatherAlertsBot.OpenWeatherAPI;

namespace WeatherAlertsBot.TelegramBotHandlers;

public sealed class UpdateHandler
{
    ITelegramBotClient BotClient { get; set; }

    Update Update { get; set; }

    CancellationToken CancellationToken { get; set; }

    WeatherHandler weatherHandler = new();

    public UpdateHandler(ITelegramBotClient telegramBotClient, Update update, CancellationToken cancellationToken)
    {
        BotClient = telegramBotClient;
        Update = update;
        CancellationToken = cancellationToken;
    }

    public async Task HandleMessageTextAsync()
    {
        if (Update.Message.Text != null)
        {
            var userMessageText = Update.Message.Text;

            if (userMessageText.ToLower().Contains("/start"))
            {
                await BotClient.SendTextMessageAsync(Update.Message.Chat.Id,
                    "`Hello!\nTo receive weather by city name send me the message in format: weather [city_name]!`",
                    ParseMode.MarkdownV2, cancellationToken: CancellationToken);
            }
            else if (userMessageText.ToLower().Contains("weather"))
            {
                var weatherResponseForUser = await weatherHandler.SendWeatherByUserMessageAsync(userMessageText);
                var errorMessage = weatherResponseForUser.ErrorMessage;

                if (string.IsNullOrEmpty(errorMessage))
                {
                    var location = await BotClient.SendLocationAsync(Update.Message.Chat.Id,
                       weatherResponseForUser.Lattitude, weatherResponseForUser.Longitude,
                       cancellationToken: CancellationToken);
                    await BotClient.SendTextMessageAsync(location.Chat.Id,
                       @$"`Current weather in {weatherResponseForUser.CityName} is {weatherResponseForUser.Temperature} °C, " +
                       "feels like {weatherResponseForUser.FeelsLike} °C.`",
                       ParseMode.MarkdownV2, cancellationToken: CancellationToken, replyToMessageId: location.MessageId);
                    return;
                }

                await BotClient.SendTextMessageAsync(Update.Message.Chat.Id,
                       errorMessage, ParseMode.MarkdownV2, cancellationToken: CancellationToken);
            };
        }
    }
    public async Task HandleMessageLocationAsync()
    {
        if (Update.Message.Location != null)
        {
            var userLocation = Update.Message.Location;

            var weatherResponseForUser = await weatherHandler.SendWeatherByUserLocationAsync(userLocation);

            await BotClient.SendTextMessageAsync(Update.Message.Chat.Id,
                       @$"`Current weather in {weatherResponseForUser.CityName} is {weatherResponseForUser.Temperature} °C, " +
                       "feels like {weatherResponseForUser.FeelsLike} °C.`",
                       ParseMode.MarkdownV2, cancellationToken: CancellationToken);
        }
    }
}
