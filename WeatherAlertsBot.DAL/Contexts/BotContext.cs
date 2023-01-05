using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WeatherAlertsBot.DAL.Entities;

namespace WeatherAlertsBot.DAL.Context;

public class BotContext : DbContext
{

    public DbSet<Command> Commands { get; set; } = null!;

    public DbSet<Subsrciber> Subsrcibers { get; set; } = null!;

    public BotContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"appsettings.json").Build();

        optionsBuilder.UseMySql(configuration.GetConnectionString("DbConnection"), new MySqlServerVersion(new Version(8, 0, 30)));
    }
}