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

    public async Task<Subsrciber?> AddSubscriberAsync(Subsrciber subsrciber)
    {
        if (!await IsSubscriberExistAsync(subsrciber.ChatId))
        {
            return null;
        }

        await _botContext.Subsrcibers.AddAsync(subsrciber);
        await _botContext.SaveChangesAsync();

        return subsrciber;
    }

    public async Task<Subsrciber?> AddCommandToSubscriberAsync(long subscrberId, string commandName)
    {
        var foundSubscriber = await FindSubscriberAsync(subscrberId);

        if (foundSubscriber != null)
        {
            var foundCommand = await FindCommandAsync(null, commandName);
            if (foundSubscriber != null && foundCommand != null) 
            {
                foundSubscriber.Commands.Add(foundCommand);
            }
        }

        return foundSubscriber;
    }

    public async Task<Command?> AddCommandAsync(Command command)
    {
        if (!await IsCommandExistAsync(command.Id))
        {
            return null;
        }

        await _botContext.Commands.AddAsync(command);
        await _botContext.SaveChangesAsync();

        return command;
    }

    public async Task<bool> IsSubscriberExistAsync(long subscriberChatId)
    {
        return await _botContext.Subsrcibers.AnyAsync(subscriber => subscriber.ChatId == subscriberChatId);
    }
    
    public async Task<Subsrciber?> FindSubscriberAsync(long subscriberChatId)
    {
        return await _botContext.Subsrcibers
            .Where(subscriber => subscriber.ChatId == subscriberChatId).FirstOrDefaultAsync();
    }
    public async Task<bool> IsCommandExistAsync(int commandId)
    {
        return await _botContext.Commands.AnyAsync(command => command.Id == commandId);
    }
    
    public async Task<Command?> FindCommandAsync(long? commandId, string? commandName)
    {
        return await _botContext.Commands
            .Where(command => !commandId.HasValue || command.Id == commandId)
            .Where(command => string.IsNullOrEmpty(commandName) || command.CommandName.Equals(commandName))
            .FirstOrDefaultAsync();
    }

}
