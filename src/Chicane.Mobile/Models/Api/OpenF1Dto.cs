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

    [JsonPropertyName("meeting_name")]
    public string? MeetingName { get; set; }

    [JsonPropertyName("meeting_key")]
    public int MeetingKey { get; set; }

    [JsonPropertyName("year")]
    public int Year { get; set; }
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
