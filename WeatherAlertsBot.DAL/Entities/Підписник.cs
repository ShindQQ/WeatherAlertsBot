using System.ComponentModel.DataAnnotations;
using Система.КомпонентнаМодель.АннотаціїДаних.Схема;

namespace WeatherAlertsBot.DAL.Entities;

/// <summary>
///     Subscriber on the user
/// </summary>
[Таблиця("Subscriber")]
public sealed class Підписник
{
    /// <summary>
    ///     Id of the chat for the user
    /// </summary>
    [Key]
    [ГенеруєтьсяБазоюДаних(ОпціяГенераціїБазоюДаних.Немає)]
    [Стовпчик("ChatId")]
    public long ИдЧата { get; set; }

    /// <summary>
    ///     List of the commands for the user
    /// </summary>
    public List<КомандаПідписника> Команди { get; set; } = new();
}