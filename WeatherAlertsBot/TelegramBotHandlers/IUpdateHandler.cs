using Telegram.Bot.Types;

namespace WeatherAlertsBot.TelegramBotHandlers;

/// <summary>
///     Telegram Bot Update Handler
/// </summary>
public interface IUpdateHandler
{
    /// <summary>
    ///     Handling user message
    /// </summary>
    /// <param name="update">Variable for handling user chat info and messages</param>
    /// <returns>Task</returns>
    Task HandleMessageAsync(Update update);
}
