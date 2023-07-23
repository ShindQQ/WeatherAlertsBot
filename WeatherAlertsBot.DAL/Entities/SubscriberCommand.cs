using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeatherAlertsBot.DAL.Entities;

/// <summary>
///     Entity describing commands for subscribers
/// </summary>
public class SubscriberCommand
{
    /// <summary>
    ///     Id of the command for the user
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    ///     Name of the command
    /// </summary>
    public string CommandName { get; set; } = string.Empty;

    /// <summary>
    ///     Users subscribed on this command
    /// </summary>
    public List<Subscriber> Subsrcibers { get; set; } = new();
}
