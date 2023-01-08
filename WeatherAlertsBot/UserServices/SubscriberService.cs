using Microsoft.EntityFrameworkCore;
using WeatherAlertsBot.DAL.Context;
using WeatherAlertsBot.DAL.Entities;
using WeatherAlertsBot.UserServices.Models;

namespace WeatherAlertsBot.UserServices;

/// <summary>
///     Service for working with subscribers db
/// </summary>
public sealed class SubscriberService
{
    /// <summary>
    ///     EF Core DB context
    /// </summary>
    private readonly BotContext _botContext = new();

    /// <summary>
    ///     Adding subscriber
    /// </summary>
    /// <param name="subscriber">Subscriber which will be added</param>
    /// <param name="commandName">Command name which will be added</param>
    /// <returns>Returns true in case of adding, false if it doesn`t exist</returns>
    public async Task<bool> AddSubscriberAsync(Subscriber subscriber, string commandName)
    {
        if (!await IsSubscriberExistAsync(subscriber.ChatId))
        {
            return false;
        }

        if (!await IsCommandExistAsync(new SubscriberCommandDto { CommandName = commandName }))
        {
            await AddCommandAsync(new SubscriberCommand { CommandName = commandName });
        }

        subscriber.Commands.Add(await FindCommandAsync(new SubscriberCommandDto { CommandName = commandName }));
        await _botContext.Subscribers.AddAsync(subscriber);
        await _botContext.SaveChangesAsync();

        return true;
    }

    /// <summary>
    ///     Adding command for subscriber
    /// </summary>
    /// <param name="subscriberChatId">Id of the subsriber chat</param>
    /// <param name="commandName">Command name which will be added</param>
    /// <returns>Returns true in case of adding, false if it doesn`t exist</returns>
    public async Task<bool> AddCommandToSubscriberAsync(long subscriberChatId, string commandName)
    {
        var foundSubscriber = await FindSubscriberAsync(subscriberChatId);

        if (foundSubscriber == null)
        {
            return false;
        }

        if (!await IsCommandExistAsync(new SubscriberCommandDto { CommandName = commandName }))
        {
            await AddCommandAsync(new SubscriberCommand { CommandName = commandName });
        }

        foundSubscriber.Commands.Add(await FindCommandAsync(new SubscriberCommandDto { CommandName = commandName }));
        await _botContext.SaveChangesAsync();

        return true;
    }

    /// <summary>
    ///     Removing command for subscriber
    /// </summary>
    /// <param name="subscriberChatId">Id of the subsriber chat</param>
    /// <param name="commandName">Command name which will be added</param>
    /// <returns>Returns true in case of adding, false if it doesn`t exist</returns>
    public async Task<bool> RemoveCommandFromSubscriberAsync(long subscriberChatId, string commandName)
    {
        var foundSubscriber = await FindSubscriberAsync(subscriberChatId);
        var check = IsSubscriberCommandExist(foundSubscriber, commandName);

        if (foundSubscriber == null || !check)
        {
            return false;
        }

        if (check)
        {
            foundSubscriber.Commands.Remove(await FindCommandAsync(new SubscriberCommandDto { CommandName = commandName }));
            await _botContext.SaveChangesAsync();
        }

        return true;
    }

    /// <summary>
    ///     Removing subscriber entity
    /// </summary>
    /// <param name="subscriber">Subscriber which will be removed</param>
    /// <returns>Returns true in case of removing, false if it doesn`t exist</returns>
    public async Task<bool> RemoveSubscriberAsync(Subscriber subscriber)
    {
        if (!await IsSubscriberExistAsync(subscriber.ChatId))
        {
            return false;
        }

        _botContext.Subscribers.Remove(subscriber);
        await _botContext.SaveChangesAsync();

        return true;
    }

    /// <summary>
    ///     Updating subscriber
    /// </summary>
    /// <param name="subscriber">Subscriber for update</param>
    /// <returns>Updated subscriber</returns>
    public async Task<Subscriber?> UpdateSubscriberAsync(Subscriber subscriber)
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
    public async Task<List<Subscriber>> GetSubscribersAsync()
    {
        return await _botContext.Subscribers.Include(subscriber => subscriber.Commands).ToListAsync();
    }

    /// <summary>
    ///     Adding command
    /// </summary>
    /// <param name="command">Command which will be added</param>
    /// <returns>Returns true in case of adding, false if it doesn`t exist</returns>
    public async Task<bool> AddCommandAsync(SubscriberCommand command)
    {
        if (!await IsCommandExistAsync(new SubscriberCommandDto { Id = command.Id }))
        {
            return false;
        }

        await _botContext.SubscriberCommands.AddAsync(command);
        await _botContext.SaveChangesAsync();

        return true;
    }

    /// <summary>
    ///     Removing command entity
    /// </summary>
    /// <param name="command">Command which will be removed</param>
    /// <returns>Returns true in case of removing, false if it doesn`t exist</returns>
    public async Task<bool> RemoveCommandAsync(SubscriberCommand command)
    {
        if (!await IsCommandExistAsync(new SubscriberCommandDto { Id = command.Id }))
        {
            return false;
        }

        _botContext.SubscriberCommands.Remove(command);
        await _botContext.SaveChangesAsync();

        return true;
    }

    /// <summary>
    ///     Updating command
    /// </summary>
    /// <param name="command">Command for update</param>
    /// <returns>Updated command</returns>
    public async Task<SubscriberCommand?> UpdateCommandAsync(SubscriberCommand command)
    {
        if (!await IsCommandExistAsync(new SubscriberCommandDto { Id = command.Id }))
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
    public async Task<bool> IsSubscriberExistAsync(long subscriberChatId)
    {
        return await _botContext.Subscribers.AnyAsync(subscriber => subscriber.ChatId == subscriberChatId);
    }

    /// <summary>
    ///     Finding if subscriber
    /// </summary>
    /// <param name="subscriberChatId">Id of the subscriber chat</param>
    /// <returns>Found subsriber</returns>
    public async Task<Subscriber?> FindSubscriberAsync(long subscriberChatId)
    {
        return await _botContext.Subscribers
            .FirstOrDefaultAsync(subscriber => subscriber.ChatId == subscriberChatId);
    }

    /// <summary>
    ///     Checking if user has such command
    /// </summary>
    /// <param name="subscriber">Subscriber given for check</param>
    /// <param name="commandName">Name of the command to find</param>
    /// <returns>True if found, false if not</returns>
    public static bool IsSubscriberCommandExist(Subscriber subscriber, string commandName)
    {
        return subscriber.Commands.Any(command => command.CommandName.Equals(commandName));
    }

    /// <summary>
    ///     Checking if command exists
    /// </summary>
    /// <param name="subscriberCommand">Subscriber command for looking for</param>
    /// <returns>True if command exists, false if not</returns>
    public async Task<bool> IsCommandExistAsync(SubscriberCommandDto subscriberCommand)
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
    public async Task<SubscriberCommand?> FindCommandAsync(SubscriberCommandDto subscriberCommand)
    {
        return await _botContext.SubscriberCommands
            .Where(command => !subscriberCommand.Id.HasValue || command.Id == subscriberCommand.Id)
            .Where(command => string.IsNullOrEmpty(subscriberCommand.CommandName) ||
            command.CommandName.Equals(subscriberCommand.CommandName))
            .FirstOrDefaultAsync();
    }

}
