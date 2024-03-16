using Microsoft.EntityFrameworkCore;
using WeatherAlertsBot.DAL.Entities;

namespace WeatherAlertsBot.DAL.Contexts;

/// <summary>
///     Db context for MySQL
/// </summary>
public class BotContext : DbContext
{
    /// <summary>
    ///     Empty db context constructor
    /// </summary>
    public BotContext()
    {
    }

    /// <summary>
    ///     Constructor for di
    /// </summary>
    /// <param name="options">Configuring db context</param>
    public BotContext(DbContextOptions<BotContext> options) : base(options)
    {
    }

    /// <summary>
    ///     Table for commands of subscribers
    /// </summary>
    public DbSet<SubscriberCommand> SubscriberCommands { get; set; } = null!;

    /// <summary>
    ///     Table for subscribers, their commands and chat id
    /// </summary>
    public DbSet<Subscriber> Subscribers { get; set; } = null!;
}