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

    public async Task HandleMessageAsync()
    {
        if (Update.Message != null)
        {
            var userMessageText = Update.Message.Text;

            if (userMessageText != null)
            {

                if (!await HandleStartMessageAsync(userMessageText) && !await HandleWeatherMessageAsync(userMessageText))
                {
                    await HandleErrorMessageAsync();
                }
            }

            await HandleLocationMessageAsync();
        }
    }

    private async Task<bool> HandleStartMessageAsync(string userMessageText)
    {
        if (userMessageText.ToLower().Equals("/start"))
        {
            await BotClient.SendTextMessageAsync(Update.Message.Chat.Id,
                "`Hello!\nTo receive weather by city name send me the message in format: /weather [city_name]!\nOr just send me your location!`",
                ParseMode.MarkdownV2, cancellationToken: CancellationToken);

            return true;
        }

        return false;
    }

    private async Task<bool> HandleWeatherMessageAsync(string userMessageText)
    {
        if (userMessageText.ToLower().StartsWith("/weather"))
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
                   $"feels like {weatherResponseForUser.FeelsLike} °C.`",
                   ParseMode.MarkdownV2, cancellationToken: CancellationToken, replyToMessageId: location.MessageId);

                return true;
            }

            await BotClient.SendTextMessageAsync(Update.Message.Chat.Id,
                   errorMessage, ParseMode.MarkdownV2, cancellationToken: CancellationToken);
        }

        return false;
    }

    private async Task HandleLocationMessageAsync()
    {
        var userLocation = Update.Message.Location;

        if (userLocation != null)
        {
            var weatherResponseForUser = await weatherHandler.SendWeatherByUserLocationAsync(userLocation);

            await BotClient.SendTextMessageAsync(Update.Message.Chat.Id,
                       @$"`Current weather in {weatherResponseForUser.CityName} is {weatherResponseForUser.Temperature} °C, " +
                       $"feels like {weatherResponseForUser.FeelsLike} °C.`",
                       ParseMode.MarkdownV2, cancellationToken: CancellationToken);
        }
    }

    private async Task HandleErrorMessageAsync()
    {
        await BotClient.SendTextMessageAsync(Update.Message.Chat.Id,
            "`Hello!\nTo receive weather by city name send me the message in format: /weather [city_name]!\nOr just send me your location!`",
            ParseMode.MarkdownV2, cancellationToken: CancellationToken);
    }
}
