using Microsoft.EntityFrameworkCore;
using WeatherAlertsBot.DAL.Context;
using WeatherAlertsBot.DAL.Entities;
using WeatherAlertsBot.UserServices.Models;

namespace WeatherAlertsBot.UserServices;

/// <summary>
///     Service for working with subscribers db
/// </summary>
public static class SubscriberService
{
    /// <summary>
    ///     EF Core DB context
    /// </summary>
    private static readonly BotContext _botContext = new();

    /// <summary>
    ///     Adding subscriber
    /// </summary>
    /// <param name="subscriber">Subscriber which will be added</param>
    /// <param name="commandName">Command name which will be added</param>
    /// <returns>Ammount of added entities</returns>
    public static async Task<int> AddSubscriberAsync(Subscriber subscriber, string commandName)
    {
        var subscriberCommandDto = new SubscriberCommandDto { CommandName = commandName };

        await AddCommandAsync(new SubscriberCommand { CommandName = commandName });

        var foundSubscriber = await FindSubscriberAsync(subscriber.ChatId);

        if (foundSubscriber != null)
        {
            return await AddCommandToSubscriberAsync(foundSubscriber, commandName);
        }

        subscriber.Commands.Add(await FindCommandAsync(subscriberCommandDto));
        await _botContext.Subscribers.AddAsync(subscriber);

        return await _botContext.SaveChangesAsync();
    }

    /// <summary>
    ///     Adding command for subscriber
    /// </summary>
    /// <param name="subscriber">Subsriber for adding</param>
    /// <param name="commandName">Command name which will be added</param>
    /// <returns>Ammount of added entities</returns>
    public static async Task<int> AddCommandToSubscriberAsync(Subscriber subscriber, string commandName)
    {
        await AddCommandAsync(new SubscriberCommand { CommandName = commandName });

        var foundSubscriberCommand = FindSubscriberCommand(subscriber, commandName);

        if (foundSubscriberCommand == null)
        {
            subscriber.Commands.Add(await FindCommandAsync(new SubscriberCommandDto { CommandName = commandName }));
        }

        return await _botContext.SaveChangesAsync();
    }

    /// <summary>
    ///     Removing command for subscriber
    /// </summary>
    /// <param name="subscriberChatId">Id of the subsriber chat</param>
    /// <param name="commandName">Command name which will be removed</param>
    /// <returns>Ammount of removed entities</returns>
    public static async Task<int> RemoveCommandFromSubscriberAsync(long subscriberChatId, string commandName)
    {
        var foundSubscriber = await FindSubscriberAsync(subscriberChatId);

        if (foundSubscriber == null)
        {
            return 0;
        }

        var foundSubscriberCommand = FindSubscriberCommand(foundSubscriber, commandName);

        if (foundSubscriberCommand == null)
        {
            return 0;
        }

        foundSubscriber.Commands.Remove(foundSubscriberCommand);

        return await _botContext.SaveChangesAsync();
    }
    
    /// <summary>
    ///     Removing command for subscriber
    /// </summary>
    /// <param name="subscriberChatId">Id of the subsriber chat</param>
    /// <param name="commandName">Command name which will be updated</param>
    /// <returns>Ammount of removed entities</returns>
    public static async Task<int> UpdateSubscriberCommandAsync(long subscriberChatId, string commandName, string commandForUpdate)
    {
        var foundSubscriber = await FindSubscriberAsync(subscriberChatId);

        if (foundSubscriber == null)
        {
            return 0;
        }

        var foundSubscriberCommand = FindSubscriberCommand(foundSubscriber, commandName);

        if (foundSubscriberCommand == null)
        {
            return await AddCommandToSubscriberAsync(foundSubscriber, commandForUpdate);
        }

        await RemoveCommandFromSubscriberAsync(subscriberChatId, commandName);

        return await AddCommandToSubscriberAsync(foundSubscriber, commandForUpdate);
    }

    /// <summary>
    ///     Removing subscriber entity
    /// </summary>
    /// <param name="subscriber">Subscriber which will be removed</param>
    /// <returns>Ammount of removed entities</returns>
    public static async Task<int> RemoveSubscriberAsync(Subscriber subscriber)
    {
        if (!await IsSubscriberExistAsync(subscriber.ChatId))
        {
            return 0;
        }

        _botContext.Subscribers.Remove(subscriber);


        return await _botContext.SaveChangesAsync();
    }

    /// <summary>
    ///     Updating subscriber
    /// </summary>
    /// <param name="subscriber">Subscriber for update</param>
    /// <returns>Updated subscriber</returns>
    public static async Task<Subscriber?> UpdateSubscriberAsync(Subscriber subscriber)
    {
        if (!await IsSubscriberExistAsync(subscriber.ChatId))
        {
            return null;
        }

        _botContext.Subscribers.Update(subscriber);
        await _botContext.SaveChangesAsync();

        return await FindSubscriberAsync(subscriber.ChatId);
    }

    /// <summary>
    ///     Receiving list of subscribers
    /// </summary>
    /// <returns>List of subscribers</returns>
    public static async Task<List<Subscriber>> GetSubscribersAsync()
    {
        return await _botContext.Subscribers.Include(subscriber => subscriber.Commands).ToListAsync();
    }

    /// <summary>
    ///     Adding command
    /// </summary>
    /// <param name="command">Command which will be added</param>
    /// <returns>Ammount of added entities</returns>
    public static async Task<int> AddCommandAsync(SubscriberCommand command)
    {
        if (await IsCommandExistAsync(new SubscriberCommandDto { CommandName = command.CommandName }))
        {
            return 0;
        }

        await _botContext.SubscriberCommands.AddAsync(command);

        return await _botContext.SaveChangesAsync();
    }

    /// <summary>
    ///     Removing command entity
    /// </summary>
    /// <param name="command">Command which will be removed</param>
    /// <returns>Ammount of removed entities</returns>
    public static async Task<int> RemoveCommandAsync(SubscriberCommand command)
    {
        if (!await IsCommandExistAsync(new SubscriberCommandDto { CommandName = command.CommandName }))
        {
            return 0;
        }

        _botContext.SubscriberCommands.Remove(command);

        return await _botContext.SaveChangesAsync();
    }

    /// <summary>
    ///     Updating command
    /// </summary>
    /// <param name="command">Command for update</param>
    /// <returns>Updated command</returns>
    public static async Task<SubscriberCommand?> UpdateCommandAsync(SubscriberCommand command)
    {
        if (!await IsCommandExistAsync(new SubscriberCommandDto { CommandName = command.CommandName }))
        {
            return null;
        }

        _botContext.SubscriberCommands.Update(command);
        await _botContext.SaveChangesAsync();

        return await FindCommandAsync(new SubscriberCommandDto { Id = command.Id, CommandName = command.CommandName });
    }

    /// <summary>
    ///     Checking if subscriber exists
    /// </summary>
    /// <param name="subscriberChatId">Id of the subscriber chat</param>
    /// <returns>True if subscriber exists, false if not</returns>
    public static async Task<bool> IsSubscriberExistAsync(long subscriberChatId)
    {
        return await _botContext.Subscribers.AnyAsync(subscriber => subscriber.ChatId == subscriberChatId);
    }

    /// <summary>
    ///     Finding if subscriber
    /// </summary>
    /// <param name="subscriberChatId">Id of the subscriber chat</param>
    /// <returns>Found subsriber</returns>
    public static async Task<Subscriber?> FindSubscriberAsync(long subscriberChatId)
    {
        return await _botContext.Subscribers.Include(subscriber => subscriber.Commands)
            .FirstOrDefaultAsync(subscriber => subscriber.ChatId == subscriberChatId);
    }

    /// <summary>
    ///     Checking if user has such command
    /// </summary>
    /// <param name="subscriber">Subscriber given for check</param>
    /// <param name="commandName">Name of the command to find</param>
    /// <returns>Found command for selected user</returns>
    public static SubscriberCommand? FindSubscriberCommand(Subscriber subscriber, string commandName)
    {
        return subscriber.Commands.FirstOrDefault(command => command.CommandName.Equals(commandName));
    }

    /// <summary>
    ///     Checking if command exists
    /// </summary>
    /// <param name="subscriberCommand">Subscriber command for looking for</param>
    /// <returns>True if command exists, false if not</returns>
    public static async Task<bool> IsCommandExistAsync(SubscriberCommandDto subscriberCommand)
    {
        return await _botContext.SubscriberCommands
             .Where(command => !subscriberCommand.Id.HasValue || command.Id == subscriberCommand.Id)
             .Where(command => string.IsNullOrEmpty(subscriberCommand.CommandName) ||
             command.CommandName.Equals(subscriberCommand.CommandName))
             .AnyAsync();
    }

    /// <summary>
    ///     Finding if subscriber command
    /// </summary>
    /// <param name="subscriberCommand">Subscriber command for looking for</param>
    /// <returns>Found subscriber command</returns>
    public static async Task<SubscriberCommand?> FindCommandAsync(SubscriberCommandDto subscriberCommand)
    {
        return await _botContext.SubscriberCommands
            .Where(command => !subscriberCommand.Id.HasValue || command.Id == subscriberCommand.Id)
            .Where(command => string.IsNullOrEmpty(subscriberCommand.CommandName) ||
            command.CommandName.Equals(subscriberCommand.CommandName))
            .FirstOrDefaultAsync();
    }
}
