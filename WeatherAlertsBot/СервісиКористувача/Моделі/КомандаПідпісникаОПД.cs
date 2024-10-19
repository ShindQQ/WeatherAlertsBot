namespace WeatherAlertsBot.СервісиКористувача.Моделі;

/// <summary>
///     Dto for filtration and other cases
/// </summary>
public sealed class КомандаПідпісникаОПД
{
    /// <summary>
    ///     Id of the subscriber command
    /// </summary>
    public int? Ид { get; set; }

    /// <summary>
    ///     Name of the command
    /// </summary>
    public string? НазваКоманди { get; set; }
}