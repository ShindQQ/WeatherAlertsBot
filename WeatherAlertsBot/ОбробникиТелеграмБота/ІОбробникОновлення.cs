using Telegram.Bot.Types;

namespace WeatherAlertsBot.ОбробникиТелеграмБота;

/// <summary>
///     Telegram Bot Update Handler
/// </summary>
public interface ІОбробникОновлення
{
    /// <summary>
    ///     Handling user message
    /// </summary>
    /// <param name="оновлення">Variable for handling user chat info and messages</param>
    /// <returns>Task</returns>
    Task ОбробитиПовідомленняАсінх(Update оновлення);

    /// <summary>
    ///     Handling sending notifications to users
    /// </summary>
    /// <returns>Task</returns>
    Task ОбробитиПовідомленняПідпісниківАсінх();
}