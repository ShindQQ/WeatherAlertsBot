using WeatherAlertsBot.DAL.Entities;

namespace WeatherAlertsBot.СервісиКористувача;

/// <summary>
///     Service for working with subscribers db
/// </summary>
public interface ІРепозіторійПідписника
{
    /// <summary>
    ///     Adding subscriber
    /// </summary>
    /// <param name="підпісник">Subscriber which will be added</param>
    /// <param name="назваКоманди">Command name which will be added</param>
    /// <returns>Amount of added entities</returns>
    ValueTask<int> ДодатиПідпісникаАсінх(Підписник підпісник, string назваКоманди);

    /// <summary>
    ///     Removing command for subscriber
    /// </summary>
    /// <param name="идЧатаПідпісника">Id of the subscriber chat</param>
    /// <param name="назваКоманди">Command name which will be removed</param>
    /// <returns>Amount of removed entities</returns>
    ValueTask<int> ВидалитиКомандуВідПідпісникаАсінх(long идЧатаПідпісника, string назваКоманди);

    /// <summary>
    ///     Receiving list of subscribers
    /// </summary>
    /// <returns>List of subscribers</returns>
    Task<List<Підписник>> ВзятиПідпісниківАсінх();

    /// <summary>
    ///     Finding if subscriber
    /// </summary>
    /// <param name="идЧатаПідпісника">Id of the subscriber chat</param>
    /// <returns>Found subscriber</returns>
    Task<Підписник?> ЗнайтиПідпісникаАсінх(long идЧатаПідпісника);
}