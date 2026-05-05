using System.Text.Json.Serialization;

namespace Chicane.Mobile.Models.Api;

// ──────── Root Response ────────
public class ErgastResponse
{
    [JsonPropertyName("MRData")]
    public ErgastMrData MrData { get; set; } = new();
}

public class ErgastMrData
{
    [JsonPropertyName("StandingsTable")]
    public ErgastStandingsTable? StandingsTable { get; set; }

    [JsonPropertyName("RaceTable")]
    public ErgastRaceTable? RaceTable { get; set; }
}

// ──────── Standings ────────
public class ErgastStandingsTable
{
    [JsonPropertyName("StandingsLists")]
    public List<ErgastStandingsList> StandingsLists { get; set; } = new();
}

public class ErgastStandingsList
{
    [JsonPropertyName("season")]
    public string Season { get; set; } = string.Empty;

    [JsonPropertyName("round")]
    public string Round { get; set; } = string.Empty;

    [JsonPropertyName("DriverStandings")]
    public List<ErgastDriverStanding>? DriverStandings { get; set; }

    [JsonPropertyName("ConstructorStandings")]
    public List<ErgastConstructorStanding>? ConstructorStandings { get; set; }
}

public class ErgastDriverStanding
{
    [JsonPropertyName("position")]
    public string Position { get; set; } = "0";

    [JsonPropertyName("points")]
    public string Points { get; set; } = "0";

    [JsonPropertyName("wins")]
    public string Wins { get; set; } = "0";

    [JsonPropertyName("Driver")]
    public ErgastDriver Driver { get; set; } = new();

    [JsonPropertyName("Constructors")]
    public List<ErgastConstructor> Constructors { get; set; } = new();
}

public class ErgastDriver
{
    [JsonPropertyName("givenName")]
    public string GivenName { get; set; } = string.Empty;

    [JsonPropertyName("familyName")]
    public string FamilyName { get; set; } = string.Empty;
}

public class ErgastConstructorStanding
{
    [JsonPropertyName("position")]
    public string Position { get; set; } = "0";

    [JsonPropertyName("points")]
    public string Points { get; set; } = "0";

    [JsonPropertyName("wins")]
    public string Wins { get; set; } = "0";

    [JsonPropertyName("Constructor")]
    public ErgastConstructor Constructor { get; set; } = new();
}

public class ErgastConstructor
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

// ──────── Race / Schedule ────────
public class ErgastRaceTable
{
    [JsonPropertyName("Races")]
    public List<ErgastRace> Races { get; set; } = new();
}

public class ErgastRace
{
    [JsonPropertyName("season")]
    public string Season { get; set; } = string.Empty;

    [JsonPropertyName("round")]
    public string Round { get; set; } = string.Empty;

    [JsonPropertyName("raceName")]
    public string RaceName { get; set; } = string.Empty;

    [JsonPropertyName("Circuit")]
    public ErgastCircuit Circuit { get; set; } = new();

    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;

    [JsonPropertyName("Results")]
    public List<ErgastRaceResult>? Results { get; set; }
}

public class ErgastCircuit
{
    [JsonPropertyName("circuitName")]
    public string CircuitName { get; set; } = string.Empty;

    [JsonPropertyName("Location")]
    public ErgastLocation Location { get; set; } = new();
}

public class ErgastLocation
{
    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;
}

public class ErgastRaceResult
{
    [JsonPropertyName("position")]
    public string Position { get; set; } = "0";

    [JsonPropertyName("Driver")]
    public ErgastDriver Driver { get; set; } = new();

    [JsonPropertyName("Constructor")]
    public ErgastConstructor Constructor { get; set; } = new();

    [JsonPropertyName("Time")]
    public ErgastTime? Time { get; set; }

    [JsonPropertyName("FastestLap")]
    public ErgastFastestLap? FastestLap { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}

public class ErgastTime
{
    [JsonPropertyName("time")]
    public string Value { get; set; } = string.Empty;
}

public class ErgastFastestLap
{
    [JsonPropertyName("rank")]
    public string Rank { get; set; } = "0";
}
