using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using WeatherAlertsBot.OpenWeatherAPI;

namespace WeatherAlertsBot.TelegramBotHandlers;

/// <summary>
///     Telegram Bot Update Handler
/// </summary>
public sealed class UpdateHandler
{
    /// <summary>
    ///     A client interface to use Telegram Bot API
    /// </summary>
    ITelegramBotClient BotClient { get; set; }

    /// <summary>
    ///     Incoming update from user
    /// </summary>
    Update Update { get; set; }

    /// <summary>
    ///     Cancellation Token
    /// </summary>
    CancellationToken CancellationToken { get; set; }

    /// <summary>
    ///     Weather Hanlder
    /// </summary>
    WeatherHandler weatherHandler = new();

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="telegramBotClient">A client interface to use Telegram Bot API</param>
    /// <param name="update">Incoming update from user</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    public UpdateHandler(ITelegramBotClient telegramBotClient, Update update, CancellationToken cancellationToken)
    {
        BotClient = telegramBotClient;
        Update = update;
        CancellationToken = cancellationToken;
    }

    /// <summary>
    ///     Handling user message
    /// </summary>
    /// <returns>Task</returns>
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

    /// <summary>
    ///     Handling /start message
    /// </summary>
    /// <param name="userMessageText">Message sent by user</param>
    /// <returns>True if user`s message equals /start, false if not</returns>
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

    /// <summary>
    ///     Handling /weather [city_name] message
    /// </summary>
    /// <param name="userMessageText">Message sent by user</param>
    /// <returns>True if user`s message starts with /weather and there were no troubles with request, false if there was troubleshooting</returns>
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

    /// <summary>
    ///     Handling user location message
    /// </summary>
    /// <returns>Task</returns>
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

    /// <summary>
    ///     Handling error message type
    /// </summary>
    /// <returns>Task</returns>
    private async Task HandleErrorMessageAsync()
    {
        await BotClient.SendTextMessageAsync(Update.Message.Chat.Id,
            "`Hello!\nTo receive weather by city name send me the message in format: /weather [city_name]!\nOr just send me your location!`",
            ParseMode.MarkdownV2, cancellationToken: CancellationToken);
    }
}
