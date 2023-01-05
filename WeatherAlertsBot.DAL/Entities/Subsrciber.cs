using System.ComponentModel.DataAnnotations;

namespace WeatherAlertsBot.DAL.Entities;

public sealed class Subsrciber
{
    [Key]
    public long ChatId { get; set; }

    public List<Command> Commands { get; set; }
}
