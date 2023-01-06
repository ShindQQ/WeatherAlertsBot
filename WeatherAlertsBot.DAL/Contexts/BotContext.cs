using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WeatherAlertsBot.DAL.Entities;

namespace WeatherAlertsBot.DAL.Context;

/// <summary>
///     Db context for MySQL
/// </summary>
public class BotContext : DbContext
{
    /// <summary>
    ///     Table for commands of subscribers
    /// </summary>
    public DbSet<SubscriberCommand> SubscriberCommands { get; set; } = null!;

    /// <summary>
    ///     Table for subscribers, their commands and chat id
    /// </summary>
    public DbSet<Subscriber> Subscribers { get; set; } = null!;

    /// <summary>
    ///     Empty db context constructor
    /// </summary>
    public BotContext()
    {
    }

    /// <summary>
    ///     Overriden method for configuring our db connection
    /// </summary>
    /// <param name="optionsBuilder">Param for configuring our connection</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"appsettings.json").Build();

        optionsBuilder.UseMySql(configuration.GetConnectionString("DbConnection"),
            new MySqlServerVersion(new Version(8, 0, 30)));
    }
}