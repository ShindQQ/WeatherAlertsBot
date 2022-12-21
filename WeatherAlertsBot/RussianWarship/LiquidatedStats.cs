using System.Text.Json.Serialization;

namespace WeatherAlertsBot.RussianWarship;

public sealed class LiquidatedStats
{
    [JsonPropertyName("personnel_units")]
    public int PersonnelUnits { get; set; }

    [JsonPropertyName("tanks")]
    public int Tanks { get; set; }

    [JsonPropertyName("armoured_fighting_vehicles")]
    public int ArmouredFightingVehicles { get; set; }

    [JsonPropertyName("artillery_systems")]
    public int ArtillerySystems { get; set; }

    [JsonPropertyName("mlrs")]
    public int MLRS { get; set; }

    [JsonPropertyName("aa_warfare_systems")]
    public int AaWarfareSystems { get; set; }

    [JsonPropertyName("planes")]
    public int Planes { get; set; }

    [JsonPropertyName("helicopters")]
    public int Helicopters { get; set; }

    [JsonPropertyName("vehicles_fuel_tanks")]
    public int VehiclesFuelTanks { get; set; }

    [JsonPropertyName("warships_cutters")]
    public int WarshipsCutters { get; set; }

    [JsonPropertyName("cruise_missiles")]
    public int CruiseMissiles { get; set; }

    [JsonPropertyName("uav_systems")]
    public int UavSystems { get; set; }

    [JsonPropertyName("special_military_equip")]
    public int SpecialMilitaryEquip { get; set; }

    [JsonPropertyName("atgm_srbm_systems")]
    public int AtgmSrbmSystems { get; set; }
}
