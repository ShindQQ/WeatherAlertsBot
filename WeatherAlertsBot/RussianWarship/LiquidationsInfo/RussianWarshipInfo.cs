using System.Text.Json.Serialization;

namespace WeatherAlertsBot.RussianWarship;

/// <summary>
///     Info about looses
/// </summary>
public sealed class RussianWarshipInfo
{
    /// <summary>
    ///     Date of update
    /// </summary>
    [JsonPropertyName("date")]
    public string Date { get; set; }

    /// <summary>
    ///     Day of the war
    /// </summary>
    [JsonPropertyName("day")]
    public int Day { get; set; }

    /// <summary>
    ///     Enemies looses info
    /// </summary>
    [JsonPropertyName("stats")]
    public LiquidatedStats LiquidatedStats { get; set; }

    /// <summary>
    ///     Enemies looses change 
    /// </summary>
    [JsonPropertyName("increase")]
    public LiquidatedStats IncreaseLiquidatedStats { get; set; }

    /// <summary>
    ///     Generating string with information about enemy losses
    /// </summary>
    /// <returns>String with information about enemy losses</returns>
    public override string ToString()
    {
        return $"""
                `Enemy losses on {Date}, day {Day}: 
                Personnel units: {LiquidatedStats.PersonnelUnits} (+{IncreaseLiquidatedStats.PersonnelUnits})
                Tanks: {LiquidatedStats.Tanks} (+{IncreaseLiquidatedStats.Tanks})
                Armoured fighting vehicles: {LiquidatedStats.ArmouredFightingVehicles} (+{IncreaseLiquidatedStats.ArmouredFightingVehicles})
                Artillery systems: {LiquidatedStats.ArtillerySystems} (+{IncreaseLiquidatedStats.ArtillerySystems})
                MLRS: {LiquidatedStats.MLRS} (+{IncreaseLiquidatedStats.MLRS})
                AA warfare systems: {LiquidatedStats.AaWarfareSystems} (+{IncreaseLiquidatedStats.AaWarfareSystems})
                Planes: {LiquidatedStats.Planes} (+{IncreaseLiquidatedStats.Planes})
                Helicopters: {LiquidatedStats.Helicopters} (+{IncreaseLiquidatedStats.Helicopters})
                Vehicles fuel tanks: {LiquidatedStats.VehiclesFuelTanks} (+{IncreaseLiquidatedStats.VehiclesFuelTanks})
                Warships cutters: {LiquidatedStats.WarshipsCutters} (+{IncreaseLiquidatedStats.WarshipsCutters})
                Cruise missiles: {LiquidatedStats.CruiseMissiles} (+{IncreaseLiquidatedStats.CruiseMissiles})
                UAV systems: {LiquidatedStats.UavSystems} (+{IncreaseLiquidatedStats.UavSystems})
                Special military equip: {LiquidatedStats.SpecialMilitaryEquip} (+{IncreaseLiquidatedStats.SpecialMilitaryEquip})
                ATGM SRBM systems: {LiquidatedStats.AtgmSrbmSystems} (+{IncreaseLiquidatedStats.AtgmSrbmSystems})`
                """;
    }
}
