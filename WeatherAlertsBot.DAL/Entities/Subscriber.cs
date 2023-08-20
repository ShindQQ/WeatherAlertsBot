using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeatherAlertsBot.DAL.Entities;

/// <summary>
///     Subscriber on the user
/// </summary>
public sealed class Subscriber
{
    /// <summary>
    ///     Id of the chat for the user
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public long ChatId { get; set; }

    /// <summary>
    ///     List of the commands for the user
    /// </summary>
    public List<SubscriberCommand> Commands { get; set; } = new();
}
