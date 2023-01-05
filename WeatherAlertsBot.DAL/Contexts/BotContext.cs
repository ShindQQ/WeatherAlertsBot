using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WeatherAlertsBot.DAL.Entities;

namespace WeatherAlertsBot.DAL.Context;

public class BotContext : DbContext
{
    private readonly string _connectionString;

    public DbSet<Command> Commands { get; set; } = null!;

    public DbSet<Subsrciber> Subsrcibers { get; set; } = null!;

    public BotContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //var configuration = new ConfigurationBuilder()
        //    .SetBasePath(Directory.GetCurrentDirectory())
        //    .AddJsonFile($"appsettings.json").Build();

        optionsBuilder.UseMySql(_connectionString, new MySqlServerVersion(new Version(8, 0, 30)));
    }
}