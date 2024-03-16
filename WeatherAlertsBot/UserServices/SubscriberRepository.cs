using Microsoft.EntityFrameworkCore;
using WeatherAlertsBot.DAL.Contexts;
using WeatherAlertsBot.DAL.Entities;
using WeatherAlertsBot.UserServices.Models;

namespace WeatherAlertsBot.UserServices;

/// <summary>
///     Service for working with subscribers db
/// </summary>
public sealed class SubscriberRepository : ISubscriberRepository
{
    /// <summary>
    ///     EF Core DB context
    /// </summary>
    private readonly BotContext _botContext;

    /// <summary>
    ///     Constructor for di
    /// </summary>
    /// <param name="botContext">Bot db context</param>
    public SubscriberRepository(BotContext botContext)
    {
        _botContext = botContext;
    }

    /// <summary>
    ///     Adding subscriber
    /// </summary>
    /// <param name="subscriber">Subscriber which will be added</param>
    /// <param name="commandName">Command name which will be added</param>
    /// <returns>Amount of added entities</returns>
    public async ValueTask<int> AddSubscriberAsync(Subscriber subscriber, string commandName)
    {
        var subscriberCommandDto = new SubscriberCommandDto { CommandName = commandName };

        await AddCommandAsync(new SubscriberCommand { CommandName = commandName });

        var foundSubscriber = await FindSubscriberAsync(subscriber.ChatId);

        if (foundSubscriber is not null)
            return await AddCommandToSubscriberAsync(foundSubscriber, commandName);

        var command = await FindCommandAsync(subscriberCommandDto);

        if (command is null)
            return 0;

        subscriber.Commands.Add(command);
        await _botContext.Subscribers.AddAsync(subscriber);

        return await _botContext.SaveChangesAsync();
    }

    /// <summary>
    ///     Removing command for subscriber
    /// </summary>
    /// <param name="subscriberChatId">Id of the subscriber chat</param>
    /// <param name="commandName">Command name which will be removed</param>
    /// <returns>Amount of removed entities</returns>
    public async ValueTask<int> RemoveCommandFromSubscriberAsync(long subscriberChatId, string commandName)
    {
        var foundSubscriber = await FindSubscriberAsync(subscriberChatId);

        if (foundSubscriber is null)
            return 0;

        var foundSubscriberCommand = FindSubscriberCommand(foundSubscriber, commandName);

        if (foundSubscriberCommand is null)
            return 0;

        foundSubscriber.Commands.Remove(foundSubscriberCommand);

        return await _botContext.SaveChangesAsync();
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
    ///     Finding if subscriber
    /// </summary>
    /// <param name="subscriberChatId">Id of the subscriber chat</param>
    /// <returns>Found subscriber</returns>
    public async Task<Subscriber?> FindSubscriberAsync(long subscriberChatId)
    {
        return await _botContext.Subscribers.Include(subscriber => subscriber.Commands)
            .FirstOrDefaultAsync(subscriber => subscriber.ChatId == subscriberChatId);
    }

    /// <summary>
    ///     Adding command for subscriber
    /// </summary>
    /// <param name="subscriber">Subscriber for adding</param>
    /// <param name="commandName">Command name which will be added</param>
    /// <returns>Amount of added entities</returns>
    private async ValueTask<int> AddCommandToSubscriberAsync(Subscriber subscriber, string commandName)
    {
        await AddCommandAsync(new SubscriberCommand { CommandName = commandName });

        var foundSubscriberCommand = FindSubscriberCommand(subscriber, commandName);

        if (foundSubscriberCommand is null)
        {
            var command = await FindCommandAsync(new SubscriberCommandDto { CommandName = commandName });

            if (command is null)
                return 0;

            subscriber.Commands.Add(command);
        }

        return await _botContext.SaveChangesAsync();
    }

    /// <summary>
    ///     Adding command
    /// </summary>
    /// <param name="command">Command which will be added</param>
    /// <returns>Amount of added entities</returns>
    private async ValueTask<int> AddCommandAsync(SubscriberCommand command)
    {
        if (await IsCommandExistAsync(new SubscriberCommandDto { CommandName = command.CommandName }))
            return 0;

        await _botContext.SubscriberCommands.AddAsync(command);

        return await _botContext.SaveChangesAsync();
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
    private async ValueTask<bool> IsCommandExistAsync(SubscriberCommandDto subscriberCommand)
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
    private async Task<SubscriberCommand?> FindCommandAsync(SubscriberCommandDto subscriberCommand)
    {
        return await _botContext.SubscriberCommands
            .Where(command => !subscriberCommand.Id.HasValue || command.Id == subscriberCommand.Id)
            .Where(command => string.IsNullOrEmpty(subscriberCommand.CommandName) ||
                              command.CommandName.Equals(subscriberCommand.CommandName))
            .FirstOrDefaultAsync();
    }
}