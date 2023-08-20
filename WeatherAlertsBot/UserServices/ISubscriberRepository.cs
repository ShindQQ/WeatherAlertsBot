using WeatherAlertsBot.DAL.Entities;
using WeatherAlertsBot.UserServices.Models;

namespace WeatherAlertsBot.UserServices;

/// <summary>
///     Service for working with subscribers db
/// </summary>
public interface ISubscriberRepository
{
    /// <summary>
    ///     Adding subscriber
    /// </summary>
    /// <param name="subscriber">Subscriber which will be added</param>
    /// <param name="commandName">Command name which will be added</param>
    /// <returns>Ammount of added entities</returns>
    ValueTask<int> AddSubscriberAsync(Subscriber subscriber, string commandName);

    /// <summary>
    ///     Adding command for subscriber
    /// </summary>
    /// <param name="subscriber">Subsriber for adding</param>
    /// <param name="commandName">Command name which will be added</param>
    /// <returns>Ammount of added entities</returns>
    ValueTask<int> AddCommandToSubscriberAsync(Subscriber subscriber, string commandName);

    /// <summary>
    ///     Removing command for subscriber
    /// </summary>
    /// <param name="subscriberChatId">Id of the subsriber chat</param>
    /// <param name="commandName">Command name which will be removed</param>
    /// <returns>Ammount of removed entities</returns>
    ValueTask<int> RemoveCommandFromSubscriberAsync(long subscriberChatId, string commandName);

    /// <summary>
    ///     Receiving list of subscribers
    /// </summary>
    /// <returns>List of subscribers</returns>
    Task<List<Subscriber>> GetSubscribersAsync();

    /// <summary>
    ///     Adding command
    /// </summary>
    /// <param name="command">Command which will be added</param>
    /// <returns>Ammount of added entities</returns>
    ValueTask<int> AddCommandAsync(SubscriberCommand command);

    /// <summary>
    ///     Finding if subscriber
    /// </summary>
    /// <param name="subscriberChatId">Id of the subscriber chat</param>
    /// <returns>Found subsriber</returns>
    Task<Subscriber?> FindSubscriberAsync(long subscriberChatId);

    /// <summary>
    ///     Checking if command exists
    /// </summary>
    /// <param name="subscriberCommand">Subscriber command for looking for</param>
    /// <returns>True if command exists, false if not</returns>
    ValueTask<bool> IsCommandExistAsync(SubscriberCommandDto subscriberCommand);

    /// <summary>
    ///     Finding if subscriber command
    /// </summary>
    /// <param name="subscriberCommand">Subscriber command for looking for</param>
    /// <returns>Found subscriber command</returns>
    Task<SubscriberCommand?> FindCommandAsync(SubscriberCommandDto subscriberCommand);
}
