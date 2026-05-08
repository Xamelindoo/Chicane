using System.Text.Json.Serialization;

namespace Chicane.Mobile.Models.Api;

public class OpenF1Session
{
    [JsonPropertyName("session_key")]
    public int SessionKey { get; set; }

    [JsonPropertyName("session_name")]
    public string? SessionName { get; set; }

    [JsonPropertyName("session_type")]
    public string? SessionType { get; set; }

    [JsonPropertyName("date_start")]
    public DateTime DateStart { get; set; }

    [JsonPropertyName("date_end")]
    public DateTime DateEnd { get; set; }

    [JsonPropertyName("circuit_short_name")]
    public string? CircuitShortName { get; set; }

    [JsonPropertyName("country_name")]
    public string? CountryName { get; set; }

    [JsonPropertyName("location")]
    public string? Location { get; set; }

    [JsonPropertyName("meeting_name")]
    public string? MeetingName { get; set; }

    [JsonPropertyName("meeting_key")]
    public int MeetingKey { get; set; }

    [JsonPropertyName("year")]
    public int Year { get; set; }

    [JsonPropertyName("is_cancelled")]
    public bool IsCancelled { get; set; }
}

public class OpenF1SessionResult
{
    [JsonPropertyName("position")]
    public int Position { get; set; }

    [JsonPropertyName("driver_number")]
    public int DriverNumber { get; set; }

    [JsonPropertyName("dnf")]
    public bool Dnf { get; set; }

    [JsonPropertyName("dns")]
    public bool Dns { get; set; }

    [JsonPropertyName("dsq")]
    public bool Dsq { get; set; }

    [JsonPropertyName("duration")]
    public double? Duration { get; set; }

    [JsonPropertyName("gap_to_leader")]
    public double? GapToLeader { get; set; }

    [JsonPropertyName("number_of_laps")]
    public int NumberOfLaps { get; set; }

    [JsonPropertyName("session_key")]
    public int SessionKey { get; set; }
}

public class OpenF1Position
{
    [JsonPropertyName("position")]
    public int Position { get; set; }

    [JsonPropertyName("driver_number")]
    public int DriverNumber { get; set; }

    [JsonPropertyName("session_key")]
    public int SessionKey { get; set; }

    [JsonPropertyName("points")]
    public int Points { get; set; }

    [JsonPropertyName("meeting_key")]
    public int MeetingKey { get; set; }
}

public class OpenF1Meeting
{
    [JsonPropertyName("meeting_key")]
    public int MeetingKey { get; set; }

    [JsonPropertyName("meeting_name")]
    public string? MeetingName { get; set; }

    [JsonPropertyName("meeting_official_name")]
    public string? MeetingOfficialName { get; set; }

    [JsonPropertyName("circuit_image")]
    public string? CircuitImage { get; set; }

    [JsonPropertyName("date_start")]
    public DateTime DateStart { get; set; }

    [JsonPropertyName("date_end")]
    public DateTime DateEnd { get; set; }

    [JsonPropertyName("circuit_short_name")]
    public string? CircuitShortName { get; set; }

    [JsonPropertyName("country_name")]
    public string? CountryName { get; set; }
}

public class OpenF1ChampionshipDriver
{
    [JsonPropertyName("driver_number")]
    public int DriverNumber { get; set; }

    [JsonPropertyName("points_current")]
    public double PointsCurrent { get; set; }

    [JsonPropertyName("position_current")]
    public int PositionCurrent { get; set; }

    [JsonPropertyName("position_start")]
    public int PositionStart { get; set; }

    [JsonPropertyName("session_key")]
    public int SessionKey { get; set; }
}

public class OpenF1ChampionshipTeam
{
    [JsonPropertyName("team_name")]
    public string? TeamName { get; set; }

    [JsonPropertyName("points_current")]
    public double PointsCurrent { get; set; }

    [JsonPropertyName("position_current")]
    public int PositionCurrent { get; set; }

    [JsonPropertyName("session_key")]
    public int SessionKey { get; set; }
}

public class OpenF1Driver
{
    [JsonPropertyName("driver_number")]
    public int DriverNumber { get; set; }

    [JsonPropertyName("broadcast_name")]
    public string? BroadcastName { get; set; }

    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }

    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }

    [JsonPropertyName("full_name")]
    public string? FullName { get; set; }

    [JsonPropertyName("team_name")]
    public string? TeamName { get; set; }

    [JsonPropertyName("team_colour")]
    public string? TeamColour { get; set; }

    [JsonPropertyName("headshot_url")]
    public string? HeadshotUrl { get; set; }

    [JsonPropertyName("name_acronym")]
    public string? NameAcronym { get; set; }
}

public class OpenF1Lap
{
    [JsonPropertyName("driver_number")]
    public int DriverNumber { get; set; }

    [JsonPropertyName("lap_number")]
    public int LapNumber { get; set; }

    [JsonPropertyName("lap_duration")]
    public double LapDuration { get; set; }

    [JsonPropertyName("session_key")]
    public int SessionKey { get; set; }
}
