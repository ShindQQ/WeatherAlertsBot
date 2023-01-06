namespace WeatherAlertsBot.UserServices.Models;

/// <summary>
///     Dto for filtration and other cases
/// </summary>
public sealed class SubscriberCommandDto
{
    /// <summary>
    ///     Id of the subscriber command
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    ///     Name of the command
    /// </summary>
    public string? CommandName { get; set; }
}
