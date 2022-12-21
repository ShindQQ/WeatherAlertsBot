using System.Text.Json.Serialization;

namespace WeatherAlertsBot.RussianWarship;

/// <summary>
///     Stats about russian invasion looses
/// </summary>
public sealed class LiquidatedStats
{
    /// <summary>
    ///     How much personnel units was lost
    /// </summary>
    [JsonPropertyName("personnel_units")]
    public int PersonnelUnits { get; set; }

    /// <summary>
    ///     How much tanks was lost
    /// </summary>
    [JsonPropertyName("tanks")]
    public int Tanks { get; set; }

    /// <summary>
    ///     How much armoured fighting vehicles was lost
    /// </summary>
    [JsonPropertyName("armoured_fighting_vehicles")]
    public int ArmouredFightingVehicles { get; set; }

    /// <summary>
    ///     How much artillery systems was lost
    /// </summary>
    [JsonPropertyName("artillery_systems")]
    public int ArtillerySystems { get; set; }

    /// <summary>
    ///     How much MLRS was lost
    /// </summary>
    [JsonPropertyName("mlrs")]
    public int MLRS { get; set; }

    /// <summary>
    ///     How much AA warfare systems was lost
    /// </summary>
    [JsonPropertyName("aa_warfare_systems")]
    public int AaWarfareSystems { get; set; }

    /// <summary>
    ///     How much planes was lost
    /// </summary>
    [JsonPropertyName("planes")]
    public int Planes { get; set; }

    /// <summary>
    ///     How much helicopters was lost
    /// </summary>
    [JsonPropertyName("helicopters")]
    public int Helicopters { get; set; }

    /// <summary>
    ///     How much vehicles fuel tanks was lost
    /// </summary>
    [JsonPropertyName("vehicles_fuel_tanks")]
    public int VehiclesFuelTanks { get; set; }

    /// <summary>
    ///     How much warships cutters was lost
    /// </summary>
    [JsonPropertyName("warships_cutters")]
    public int WarshipsCutters { get; set; }

    /// <summary>
    ///     How much cruise missiles was lost
    /// </summary>
    [JsonPropertyName("cruise_missiles")]
    public int CruiseMissiles { get; set; }

    /// <summary>
    ///     How much UAV systems was lost
    /// </summary>
    [JsonPropertyName("uav_systems")]
    public int UavSystems { get; set; }

    /// <summary>
    ///     How much special military equip was lost
    /// </summary>
    [JsonPropertyName("special_military_equip")]
    public int SpecialMilitaryEquip { get; set; }

    /// <summary>
    ///     How much ATGM SRBM systems was lost
    /// </summary>
    [JsonPropertyName("atgm_srbm_systems")]
    public int AtgmSrbmSystems { get; set; }
}
