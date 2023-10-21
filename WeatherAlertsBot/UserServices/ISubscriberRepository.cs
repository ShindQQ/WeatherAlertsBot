using WeatherAlertsBot.DAL.Entities;

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
    /// <returns>Amount of added entities</returns>
    ValueTask<int> AddSubscriberAsync(Subscriber subscriber, string commandName);

    /// <summary>
    ///     Removing command for subscriber
    /// </summary>
    /// <param name="subscriberChatId">Id of the subscriber chat</param>
    /// <param name="commandName">Command name which will be removed</param>
    /// <returns>Amount of removed entities</returns>
    ValueTask<int> RemoveCommandFromSubscriberAsync(long subscriberChatId, string commandName);

    /// <summary>
    ///     Receiving list of subscribers
    /// </summary>
    /// <returns>List of subscribers</returns>
    Task<List<Subscriber>> GetSubscribersAsync();

    /// <summary>
    ///     Finding if subscriber
    /// </summary>
    /// <param name="subscriberChatId">Id of the subscriber chat</param>
    /// <returns>Found subscriber</returns>
    Task<Subscriber?> FindSubscriberAsync(long subscriberChatId);
}
