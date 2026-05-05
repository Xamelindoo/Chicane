using System.Text.Json.Serialization;

namespace Chicane.Mobile.Models.Api;

public class OpenF1Session
{
    [JsonPropertyName("session_key")]
    public int SessionKey { get; set; }

    [JsonPropertyName("session_name")]
    public string SessionName { get; set; } = string.Empty;

    [JsonPropertyName("date_start")]
    public DateTime DateStart { get; set; }

    [JsonPropertyName("date_end")]
    public DateTime DateEnd { get; set; }

    [JsonPropertyName("circuit_short_name")]
    public string CircuitShortName { get; set; } = string.Empty;

    [JsonPropertyName("country_name")]
    public string CountryName { get; set; } = string.Empty;

    [JsonPropertyName("meeting_name")]
    public string MeetingName { get; set; } = string.Empty;

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
