using System.ComponentModel.DataAnnotations;

namespace WeatherAlertsBot.DAL.Entities;

public sealed class Command
{
    [Key]
    public int Id { get; set; }

    public string CommandName { get; set; }
}
