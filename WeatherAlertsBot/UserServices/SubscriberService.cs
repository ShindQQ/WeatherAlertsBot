using Microsoft.EntityFrameworkCore;
using WeatherAlertsBot.DAL.Context;
using WeatherAlertsBot.DAL.Entities;

namespace WeatherAlertsBot.UserServices;

public sealed class SubscriberService
{
    /// <summary>
    ///     EF Core DB context
    /// </summary>
    private readonly BotContext _botContext;

    public SubscriberService()
    {
        _botContext = new();
    }

    public async Task<Subscriber?> AddSubscriberAsync(Subscriber subscriber)
    {
        if (!await IsSubscriberExistAsync(subscriber.ChatId))
        {
            return null;
        }

        await _botContext.Subscribers.AddAsync(subscriber);
        await _botContext.SaveChangesAsync();

        return subscriber;
    }

    public async Task<Subscriber?> AddCommandToSubscriberAsync(long subscriberChatId, string commandName)
    {
        var foundSubscriber = await FindSubscriberAsync(subscriberChatId);

        if (foundSubscriber != null)
        {
            var foundCommand = await FindCommandAsync(null, commandName);

            if (foundCommand != null) 
            {
                foundSubscriber.Commands.Add(foundCommand);
            }
        }

        return foundSubscriber;
    }

    public async Task<SubscriberCommands?> AddCommandAsync(SubscriberCommands command)
    {
        if (!await IsCommandExistAsync(command.Id))
        {
            return null;
        }

        await _botContext.SubscriberCommands.AddAsync(command);
        await _botContext.SaveChangesAsync();

        return command;
    }

    public async Task<bool> IsSubscriberExistAsync(long subscriberChatId)
    {
        return await _botContext.Subscribers.AnyAsync(subscriber => subscriber.ChatId == subscriberChatId);
    }
    
    public async Task<Subscriber?> FindSubscriberAsync(long subscriberChatId)
    {
        return await _botContext.Subscribers
            .Where(subscriber => subscriber.ChatId == subscriberChatId).FirstOrDefaultAsync();
    }

    public async Task<bool> IsCommandExistAsync(int commandId)
    {
        return await _botContext.SubscriberCommands.AnyAsync(command => command.Id == commandId);
    }
    
    public async Task<SubscriberCommands?> FindCommandAsync(long? commandId, string? commandName)
    {
        return await _botContext.SubscriberCommands
            .Where(command => !commandId.HasValue || command.Id == commandId)
            .Where(command => string.IsNullOrEmpty(commandName) || command.CommandName.Equals(commandName))
            .FirstOrDefaultAsync();
    }

}
