using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
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
    private ITelegramBotClient BotClient { get; init; }

    /// <summary>
    ///     Incoming update from user
    /// </summary>
    private Update Update { get; init; }

    /// <summary>
    ///     Cancellation Token
    /// </summary>
    private CancellationToken CancellationToken { get; init; }

    /// <summary>
    ///     Weather Hanlder
    /// </summary>
    private readonly WeatherHandler weatherHandler = new();

    /// <summary>
    ///     Class of logic for calling APIs
    /// </summary>
    private readonly APIsRequestsHandler APIsRequestsHandler = new();

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
                if (!await HandleStartMessageAsync(userMessageText) &&
                    !await HandleWeatherMessageAsync(userMessageText) &&
                    !await HandleRussianInvasionInfo(userMessageText))
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
                "`Hello!\nTo receive weather by city name send me the message in format: /weather [city_name]!\n`" +
                "`Or just send me your location!`",
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
                   $"`Current weather in {weatherResponseForUser.CityName} is {weatherResponseForUser.Temperature} °C.\n" +
                   $"feels like {weatherResponseForUser.FeelsLike} °C. Type of weather: {weatherResponseForUser.WeatherInfo}.`",
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
                $"`Current weather in {weatherResponseForUser.CityName} is {weatherResponseForUser.Temperature} °C.\n" +
                $"feels like {weatherResponseForUser.FeelsLike} °C. Type of weather: {weatherResponseForUser.WeatherInfo}.`",
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
            "`Hello!\nTo receive weather by city name send me the message in format: /weather [city_name]!\n`" +
            "`Or just send me your location!`",
            ParseMode.MarkdownV2, cancellationToken: CancellationToken);
    }

    /// <summary>
    ///     Receiving info about liquidations in russian invasion
    /// </summary>
    /// <param name="userMessageText">Message sent by user</param>
    /// <returns>RussianInvasion</returns>
    private async Task<bool> HandleRussianInvasionInfo(string userMessageText)
    {
        if (userMessageText.ToLower().Equals("/alerts_lost"))
        {
            string url = "https://russianwarship.rip/api/v1/statistics/latest";

            var RussianInvasion = (await APIsRequestsHandler.GetResponseFromAPI<RussianInvasion>(url)).RussianWarshipInfo;
            var IncreaseLiquidatedStats = RussianInvasion.IncreaseLiquidatedStats;
            var LiquidatedStats = RussianInvasion.LiquidatedStats;

            await BotClient.SendTextMessageAsync(Update.Message.Chat.Id,
                $"`Information about enemy losses on {RussianInvasion.Date}, day {RussianInvasion.Day}:\n" +
                $"Personnel units: {LiquidatedStats.PersonnelUnits} (+{IncreaseLiquidatedStats.PersonnelUnits})\n" +
                $"Tanks: {LiquidatedStats.Tanks} (+{IncreaseLiquidatedStats.Tanks})\n" +
                $"Armoured fighting vehicles: {LiquidatedStats.ArmouredFightingVehicles} (+{IncreaseLiquidatedStats.ArmouredFightingVehicles})\n" +
                $"Artillery systems: {LiquidatedStats.ArtillerySystems} (+{IncreaseLiquidatedStats.ArtillerySystems})\n" +
                $"MLRS: {LiquidatedStats.MLRS} (+{IncreaseLiquidatedStats.MLRS})\n" +
                $"AA warfare systems: {LiquidatedStats.AaWarfareSystems} (+{IncreaseLiquidatedStats.AaWarfareSystems})\n" +
                $"Planes: {LiquidatedStats.Planes} (+{IncreaseLiquidatedStats.Planes})\n" +
                $"Helicopters: {LiquidatedStats.Helicopters} (+{IncreaseLiquidatedStats.Helicopters})\n" +
                $"Vehicles fuel tanks: {LiquidatedStats.VehiclesFuelTanks} (+{IncreaseLiquidatedStats.VehiclesFuelTanks})\n" +
                $"Warships cutters: {LiquidatedStats.WarshipsCutters} (+{IncreaseLiquidatedStats.WarshipsCutters})\n" +
                $"Cruise missiles: {LiquidatedStats.CruiseMissiles} (+{IncreaseLiquidatedStats.CruiseMissiles})\n" +
                $"UAV systems: {LiquidatedStats.UavSystems} (+{IncreaseLiquidatedStats.UavSystems})\n" +
                $"Special military equip: {LiquidatedStats.SpecialMilitaryEquip} (+{IncreaseLiquidatedStats.SpecialMilitaryEquip})\n" +
                $"ATGM SRBM systems: {LiquidatedStats.AtgmSrbmSystems} (+{IncreaseLiquidatedStats.AtgmSrbmSystems})`",
                ParseMode.MarkdownV2, cancellationToken: CancellationToken);

            return true;
        }

        return false;
    }
}
