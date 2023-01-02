using System.Data;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using WeatherAlertsBot.Commands;
using WeatherAlertsBot.Helpers;
using WeatherAlertsBot.OpenWeatherAPI;
using WeatherAlertsBot.Requesthandlers;
using WeatherAlertsBot.RussianWarship;

namespace WeatherAlertsBot.TelegramBotHandlers;

/// <summary>
///     Telegram Bot Update Handler
/// </summary>
public sealed class UpdateHandler
{
    /// <summary>
    ///     A client interface to use Telegram Bot API
    /// </summary>
    private readonly ITelegramBotClient _botClient;

    /// <summary>
    ///     Incoming update from user
    /// </summary>
    private readonly Update _update;

    /// <summary>
    ///     Cancellation Token
    /// </summary>
    private readonly CancellationToken _cancellationToken;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="telegramBotClient">A client interface to use Telegram Bot API</param>
    /// <param name="update">Incoming update from user</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    public UpdateHandler(ITelegramBotClient telegramBotClient, Update update, CancellationToken cancellationToken)
    {
        _botClient = telegramBotClient;
        _update = update;
        _cancellationToken = cancellationToken;
    }

    /// <summary>
    ///     Handling user message
    /// </summary>
    /// <returns>Task</returns>
    public async Task HandleMessageAsync()
    {
        if (_update.Message != null)
        {
            var userMessageText = _update.Message.Text;

            if (userMessageText != null)
            {
                Task command = userMessageText switch
                {
                    BotCommands.StartCommand => HandleStartMessage(),
                    _ when userMessageText.StartsWith(BotCommands.WeatherCommand) => HandleWeatherMessageAsync(userMessageText),
                    _ when userMessageText.StartsWith(BotCommands.AlertsMapCommand) => HandleAlertsInfo(),
                    _ when userMessageText.StartsWith(BotCommands.AlertsLostCommand) => HandleRussianInvasionInfo(),
                    _ => HandleErrorMessage()
                };

                await command;
            }

            await HandleLocationMessageAsync();
        }
    }

    /// <summary>
    ///     Handling /start message
    /// </summary>
    /// <returns>True if user`s message equals /start, false if not</returns>
    private Task HandleStartMessage()
    {
        return HandleErrorMessage();
    }

    /// <summary>
    ///     Handling /weather [city_name] message
    /// </summary>
    /// <param name="userMessageText">Message sent by user</param>
    /// <returns>True if user`s message starts with /weather and there were no troubles with request, false if there was troubleshooting</returns>
    private async Task HandleWeatherMessageAsync(string userMessageText)
    {
        var weatherResponseForUser = await WeatherHandler.SendWeatherByUserMessageAsync(userMessageText);
        var errorMessage = weatherResponseForUser.ErrorMessage;

        if (!string.IsNullOrEmpty(errorMessage))
        {
            await _botClient.SendTextMessageAsync(_update.Message!.Chat.Id,
              $"""
              `{errorMessage}`
              """,
              ParseMode.MarkdownV2, cancellationToken: _cancellationToken);

            return;
        }

        await _botClient.SendVenueAsync(_update.Message!.Chat.Id,
            weatherResponseForUser.Lattitude, weatherResponseForUser.Longitude,
            string.Empty,
            $"""
            Current weather in {weatherResponseForUser.CityName} is {weatherResponseForUser.Temperature:N2} °C.
            Feels like {weatherResponseForUser.FeelsLike:N2} °C. Type of weather: {weatherResponseForUser.WeatherInfo}.
            """,
            cancellationToken: _cancellationToken);
    }

    /// <summary>
    ///     Handling user location message
    /// </summary>
    /// <returns>Task</returns>
    private async Task HandleLocationMessageAsync()
    {
        var userLocation = _update.Message!.Location;

        if (userLocation != null)
        {
            var weatherResponseForUser = await WeatherHandler.SendWeatherByUserLocationAsync(userLocation);

            await _botClient.SendTextMessageAsync(_update.Message.Chat.Id,
                $"""
                `Current weather in {weatherResponseForUser.CityName} is {weatherResponseForUser.Temperature:N2} °C.
                Feels like {weatherResponseForUser.FeelsLike:N2} °C. Type of weather: {weatherResponseForUser.WeatherInfo}.`
                """,
                ParseMode.MarkdownV2, cancellationToken: _cancellationToken);
        }
    }

    /// <summary>
    ///     Handling error message type
    /// </summary>
    /// <returns>Task</returns>
    private Task HandleErrorMessage()
    {
        return _botClient.SendTextMessageAsync(_update.Message!.Chat.Id,
            """
            `Hello!
            To receive weather by city name send me the message in format: /weather [city_name]!
            Or just send me your location!
            For map of alerts use /alerts_map!
            For liquidations information /alerts_lost!`
            """,
            ParseMode.MarkdownV2, cancellationToken: _cancellationToken);
    }

    /// <summary>
    ///     Receiving info about liquidations in russian invasion
    /// </summary>
    /// <param name="userMessageText">Message sent by user</param>
    /// no troubles with request, false if there was troubleshooting</returns>
    private async Task HandleRussianInvasionInfo()
    {
        var russianInvasion = (await APIsRequestsHandler.GetResponseFromAPI<RussianInvasion>(APIsLinks.RussianWarshipUrl))!.RussianWarshipInfo;

        await _botClient.SendTextMessageAsync(_update.Message!.Chat.Id,
        russianInvasion.ToString(),
        ParseMode.MarkdownV2, cancellationToken: _cancellationToken);
    }

    /// <summary>
    ///     Receiving info about alerts in Ukraine regions
    /// </summary>
    /// <returns>True if user`s message equals with /alerts_map and there were 
    /// no troubles with request, false if there was troubleshooting</returns>
    private async Task HandleAlertsInfo()
    {
        string messageForUser = $"`Information about current alerts in Ukraine:\n";

        var regions = await APIsRequestsHandler.GetResponseForAlertsCachedAsync();

        var bytes = AlarmsMapGenerator.DrawAlertsMap(regions);

        await _botClient.SendPhotoAsync(_update.Message!.Chat.Id, new InputOnlineFile(new MemoryStream(bytes)),
            messageForUser + string.Join("\n",
            regions.Where(region => region.Value.Enabled)
            .Select(region => $"🚨 {region.Key.Trim('\'')}; Enabled at: " +
            $"{DateTime.Parse(region.Value.EnabledAt):MM/dd/yyyy HH:mm}")) + "`",
            ParseMode.MarkdownV2, cancellationToken: _cancellationToken);
    }
}
