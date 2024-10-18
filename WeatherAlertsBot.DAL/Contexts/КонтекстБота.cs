using Microsoft.EntityFrameworkCore;
using WeatherAlertsBot.DAL.Entities;

namespace WeatherAlertsBot.DAL.Contexts;

/// <summary>
///     Db context for MySQL
/// </summary>
public class КонтекстБота : DbContext
{
    /// <summary>
    ///     Empty db context constructor
    /// </summary>
    public КонтекстБота()
    {
    }

    /// <summary>
    ///     Constructor for di
    /// </summary>
    /// <param name="options">Configuring db context</param>
    public КонтекстБота(DbContextOptions<КонтекстБота> options) : base(options)
    {
    }

    /// <summary>
    ///     Table for commands of subscribers
    /// </summary>
    public DbSet<КомандаПідписника> КомандиПідпісників { get; set; } = null!;

    /// <summary>
    ///     Table for subscribers, their commands and chat id
    /// </summary>
    public DbSet<Підписник> Підпісники { get; set; } = null!;
}