using Microsoft.EntityFrameworkCore;
using WeatherAlertsBot.DAL.Contexts;
using WeatherAlertsBot.DAL.Entities;
using WeatherAlertsBot.СервісиКористувача.Моделі;

namespace WeatherAlertsBot.СервісиКористувача;

/// <summary>
///     Service for working with subscribers db
/// </summary>
public sealed class РепозіторійПідписника : ІРепозіторійПідписника
{
    /// <summary>
    ///     EF Core DB context
    /// </summary>
    private readonly КонтекстБота _контекстБота;

    /// <summary>
    ///     Constructor for di
    /// </summary>
    /// <param name="botContext">Bot db context</param>
    public РепозіторійПідписника(КонтекстБота botContext)
    {
        _контекстБота = botContext;
    }

    /// <summary>
    ///     Adding subscriber
    /// </summary>
    /// <param name="subscriber">Subscriber which will be added</param>
    /// <param name="commandName">Command name which will be added</param>
    /// <returns>Amount of added entities</returns>
    public async ValueTask<int> ДодатиПідпісникаАсінх(Підписник підпісник, string назваКоманди)
    {
        var командаПідпісникаОПД = new КомандаПідпісникаОПД { НазваКоманди = назваКоманди };

        await ДодатиКомандуАсінх(new КомандаПідписника { НазваКоманди = назваКоманди });

        var знайденийПідпісник = await ЗнайтиПідпісникаАсінх(підпісник.ИдЧата);

        if (знайденийПідпісник is not null)
            return await ДодатиКомандуДоПідпісникаАсінх(знайденийПідпісник, назваКоманди);

        var команда = await FindCommandAsync(командаПідпісникаОПД);

        if (команда is null)
            return 0;

        підпісник.Команди.Add(команда);
        await _контекстБота.Підпісники.AddAsync(підпісник);

        return await _контекстБота.SaveChangesAsync();
    }

    /// <summary>
    ///     Removing command for subscriber
    /// </summary>
    /// <param name="идЧатаПідпісника">Id of the subscriber chat</param>
    /// <param name="назваКоманди">Command name which will be removed</param>
    /// <returns>Amount of removed entities</returns>
    public async ValueTask<int> ВидалитиКомандуВідПідпісникаАсінх(long идЧатаПідпісника, string назваКоманди)
    {
        var знайденийПідпісник = await ЗнайтиПідпісникаАсінх(идЧатаПідпісника);

        if (знайденийПідпісник is null)
            return 0;

        var знайденаКомандаПідпісника = FindSubscriberCommand(знайденийПідпісник, назваКоманди);

        if (знайденаКомандаПідпісника is null)
            return 0;

        знайденийПідпісник.Команди.Remove(знайденаКомандаПідпісника);

        return await _контекстБота.SaveChangesAsync();
    }

    /// <summary>
    ///     Receiving list of subscribers
    /// </summary>
    /// <returns>List of subscribers</returns>
    public async Task<List<Підписник>> ВзятиПідпісниківАсінх()
    {
        return await _контекстБота.Підпісники.Include(підпісник => підпісник.Команди).ToListAsync();
    }

    /// <summary>
    ///     Finding if subscriber
    /// </summary>
    /// <param name="идЧатаПідпісника">Id of the subscriber chat</param>
    /// <returns>Found subscriber</returns>
    public async Task<Підписник?> ЗнайтиПідпісникаАсінх(long идЧатаПідпісника)
    {
        return await _контекстБота.Підпісники.Include(підпісник => підпісник.Команди)
            .FirstOrDefaultAsync(підпісник => підпісник.ИдЧата == идЧатаПідпісника);
    }

    /// <summary>
    ///     Adding command for subscriber
    /// </summary>
    /// <param name="підпісник">Subscriber for adding</param>
    /// <param name="назваКоманди">Command name which will be added</param>
    /// <returns>Amount of added entities</returns>
    private async ValueTask<int> ДодатиКомандуДоПідпісникаАсінх(Підписник підпісник, string назваКоманди)
    {
        await ДодатиКомандуАсінх(new КомандаПідписника { НазваКоманди = назваКоманди });

        var знайденаКомандаПідпісника = FindSubscriberCommand(підпісник, назваКоманди);

        if (знайденаКомандаПідпісника is null)
        {
            var команда = await FindCommandAsync(new КомандаПідпісникаОПД { НазваКоманди = назваКоманди });

            if (команда is null)
                return 0;

            підпісник.Команди.Add(команда);
        }

        return await _контекстБота.SaveChangesAsync();
    }

    /// <summary>
    ///     Adding command
    /// </summary>
    /// <param name="command">Command which will be added</param>
    /// <returns>Amount of added entities</returns>
    private async ValueTask<int> ДодатиКомандуАсінх(КомандаПідписника command)
    {
        if (await IsCommandExistAsync(new КомандаПідпісникаОПД { НазваКоманди = command.НазваКоманди }))
            return 0;

        await _контекстБота.КомандиПідпісників.AddAsync(command);

        return await _контекстБота.SaveChangesAsync();
    }

    /// <summary>
    ///     Checking if user has such command
    /// </summary>
    /// <param name="підпісник">Subscriber given for check</param>
    /// <param name="назваКоманди">Name of the command to find</param>
    /// <returns>Found command for selected user</returns>
    public static КомандаПідписника? FindSubscriberCommand(Підписник підпісник, string назваКоманди)
    {
        return підпісник.Команди.FirstOrDefault(команда => команда.НазваКоманди.Equals(назваКоманди));
    }

    /// <summary>
    ///     Checking if command exists
    /// </summary>
    /// <param name="командаПідпісника">Subscriber command for looking for</param>
    /// <returns>True if command exists, false if not</returns>
    private async ValueTask<bool> IsCommandExistAsync(КомандаПідпісникаОПД командаПідпісника)
    {
        return await _контекстБота.КомандиПідпісників
            .Where(command => !командаПідпісника.Ид.HasValue || command.Ид == командаПідпісника.Ид)
            .Where(command => string.IsNullOrEmpty(командаПідпісника.НазваКоманди) ||
                              command.НазваКоманди.Equals(командаПідпісника.НазваКоманди))
            .AnyAsync();
    }

    /// <summary>
    ///     Finding if subscriber command
    /// </summary>
    /// <param name="командаПідпісника">Subscriber command for looking for</param>
    /// <returns>Found subscriber command</returns>
    private async Task<КомандаПідписника?> FindCommandAsync(КомандаПідпісникаОПД командаПідпісника)
    {
        return await _контекстБота.КомандиПідпісників
            .Where(command => !командаПідпісника.Ид.HasValue || command.Ид == командаПідпісника.Ид)
            .Where(command => string.IsNullOrEmpty(командаПідпісника.НазваКоманди) ||
                              command.НазваКоманди.Equals(командаПідпісника.НазваКоманди))
            .FirstOrDefaultAsync();
    }
}