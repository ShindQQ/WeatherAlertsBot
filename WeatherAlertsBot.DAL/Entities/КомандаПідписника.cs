using System.ComponentModel.DataAnnotations;
using Система.КомпонентнаМодель.АннотаціїДаних.Схема;

namespace WeatherAlertsBot.DAL.Entities;

/// <summary>
///     Entity describing commands for subscribers
/// </summary>
[Таблиця("SubscriberCommand")]
public sealed class КомандаПідписника
{
    /// <summary>
    ///     Id of the command for the user
    /// </summary>
    [Key]
    [ГенеруєтьсяБазоюДаних(ОпціяГенераціїБазоюДаних.Ідентичність)]
    [Стовпчик("Id")]
    public int Ид { get; set; }

    /// <summary>
    ///     Name of the command
    /// </summary>
    [Стовпчик("CommandName")]
    public string НазваКоманди { get; set; } = string.Empty;

    /// <summary>
    ///     Users subscribed on this command
    /// </summary>
    public List<Підписник> Підписники { get; set; } = new();
}