using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Chicane.Backend.Models.Api;

public class OpenF1Session
{
    [JsonPropertyName("session_key")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int SessionKey { get; set; }

    [JsonPropertyName("session_type")]
    public string? SessionType { get; set; }

    [JsonPropertyName("session_name")]
    public string? SessionName { get; set; }

    [JsonPropertyName("date_start")]
    public DateTime DateStart { get; set; }

    [JsonPropertyName("date_end")]
    public DateTime DateEnd { get; set; }

    [JsonPropertyName("meeting_key")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int MeetingKey { get; set; }

    [JsonPropertyName("circuit_key")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int CircuitKey { get; set; }

    [JsonPropertyName("circuit_short_name")]
    public string? CircuitShortName { get; set; }

    [JsonPropertyName("country_key")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int CountryKey { get; set; }

    [JsonPropertyName("country_code")]
    public string? CountryCode { get; set; }

    [JsonPropertyName("country_name")]
    public string? CountryName { get; set; }

    [JsonPropertyName("location")]
    public string? Location { get; set; }

    [JsonPropertyName("year")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int Year { get; set; }

    [JsonPropertyName("is_cancelled")]
    public bool IsCancelled { get; set; }
}

public class OpenF1SessionResult
{
    [JsonPropertyName("position")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int Position { get; set; }

    [JsonPropertyName("driver_number")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int DriverNumber { get; set; }

    [JsonPropertyName("dnf")]
    public bool Dnf { get; set; }

    [JsonPropertyName("dns")]
    public bool Dns { get; set; }

    [JsonPropertyName("dsq")]
    public bool Dsq { get; set; }

    [JsonPropertyName("duration")]
    [JsonConverter(typeof(SafeDoubleConverter))]
    public double? Duration { get; set; }

    [JsonPropertyName("gap_to_leader")]
    [JsonConverter(typeof(SafeDoubleConverter))]
    public double? GapToLeader { get; set; }

    [JsonPropertyName("number_of_laps")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int NumberOfLaps { get; set; }

    [JsonPropertyName("session_key")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int SessionKey { get; set; }
}

public class OpenF1Position
{
    [JsonPropertyName("position")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int Position { get; set; }

    [JsonPropertyName("driver_number")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int DriverNumber { get; set; }

    [JsonPropertyName("session_key")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int SessionKey { get; set; }

    [JsonPropertyName("points")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int Points { get; set; }

    [JsonPropertyName("meeting_key")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int MeetingKey { get; set; }
}

public class OpenF1Meeting
{
    [JsonPropertyName("meeting_key")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int MeetingKey { get; set; }

    [JsonPropertyName("meeting_name")]
    public string? MeetingName { get; set; }

    [JsonPropertyName("circuit_short_name")]
    public string? CircuitShortName { get; set; }

    [JsonPropertyName("country_name")]
    public string? CountryName { get; set; }

    [JsonPropertyName("date_start")]
    public DateTime DateStart { get; set; }

    [JsonPropertyName("date_end")]
    public DateTime DateEnd { get; set; }

    [JsonPropertyName("circuit_image")]
    public string? CircuitImage { get; set; }
}

public class OpenF1ChampionshipDriver
{
    [JsonPropertyName("meeting_key")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int MeetingKey { get; set; }

    [JsonPropertyName("session_key")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int SessionKey { get; set; }

    [JsonPropertyName("driver_number")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int DriverNumber { get; set; }

    [JsonPropertyName("position_start")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int PositionStart { get; set; }

    [JsonPropertyName("position_current")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int PositionCurrent { get; set; }

    [JsonPropertyName("points_start")]
    [JsonConverter(typeof(SafeNonNullDoubleConverter))]
    public double PointsStart { get; set; }

    [JsonPropertyName("points_current")]
    [JsonConverter(typeof(SafeNonNullDoubleConverter))]
    public double PointsCurrent { get; set; }
}

public class OpenF1ChampionshipTeam
{
    [JsonPropertyName("meeting_key")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int MeetingKey { get; set; }

    [JsonPropertyName("session_key")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int SessionKey { get; set; }

    [JsonPropertyName("team_name")]
    public string? TeamName { get; set; }

    [JsonPropertyName("position_start")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int PositionStart { get; set; }

    [JsonPropertyName("position_current")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int PositionCurrent { get; set; }

    [JsonPropertyName("points_start")]
    [JsonConverter(typeof(SafeNonNullDoubleConverter))]
    public double PointsStart { get; set; }

    [JsonPropertyName("points_current")]
    [JsonConverter(typeof(SafeNonNullDoubleConverter))]
    public double PointsCurrent { get; set; }
}

public class OpenF1Driver
{
    [JsonPropertyName("driver_number")]
    [JsonConverter(typeof(SafeIntConverter))]
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
    [JsonConverter(typeof(SafeIntConverter))]
    public int DriverNumber { get; set; }

    [JsonPropertyName("lap_number")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int LapNumber { get; set; }

    [JsonPropertyName("lap_duration")]
    [JsonConverter(typeof(SafeNonNullDoubleConverter))]
    public double LapDuration { get; set; }

    [JsonPropertyName("session_key")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int SessionKey { get; set; }
}

/// <summary>
/// JSON converter for <c>double?</c> that never throws.
/// Handles: numbers (12.3), string numbers ("12.3"), arrays (takes first element),
/// empty strings ("" → null), non-numeric strings ("N/A" → null), and JSON null (→ null).
/// </summary>
public class SafeDoubleConverter : JsonConverter<double?>
{
    public override double? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.StartArray => ReadFirstFromArray(ref reader),
            JsonTokenType.Number when reader.TryGetDouble(out var d) => d,
            JsonTokenType.String => double.TryParse(reader.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ? d : null,
            _ => null
        };
    }

    /// <summary>Reads the first element of a JSON array as a nullable double.</summary>
    private static double? ReadFirstFromArray(ref Utf8JsonReader reader)
    {
        if (!reader.Read() || reader.TokenType == JsonTokenType.EndArray) return null;
        var result = reader.TokenType switch
        {
            JsonTokenType.Number when reader.TryGetDouble(out var d) => d,
            JsonTokenType.String when double.TryParse(reader.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var d) => d,
            _ => (double?)null
        };
        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray) { } // skip rest
        return result;
    }

    public override void Write(Utf8JsonWriter writer, double? value, JsonSerializerOptions options)
    {
        if (value.HasValue) writer.WriteNumberValue(value.Value);
        else writer.WriteNullValue();
    }
}

/// <summary>
/// JSON converter for <c>int</c> that never throws.
/// Handles: numbers (12), string numbers ("12"), empty strings ("" → 0),
/// non-numeric strings ("N/A" → 0), and JSON null (→ 0).
/// </summary>
public class SafeIntConverter : JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Number when reader.TryGetInt32(out var i) => i,
            JsonTokenType.String => int.TryParse(reader.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var i) ? i : 0,
            _ => 0
        };
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}

/// <summary>
/// JSON converter for non-nullable <c>double</c> that never throws.
/// Same as SafeDoubleConverter but returns 0 instead of null for invalid inputs.
/// Also handles JSON arrays by taking the first element.
/// Required for DTO fields declared as <c>double</c> (not <c>double?</c>).
/// </summary>
public class SafeNonNullDoubleConverter : JsonConverter<double>
{
    public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.StartArray => ReadFirstFromArray(ref reader),
            JsonTokenType.Number when reader.TryGetDouble(out var d) => d,
            JsonTokenType.String => double.TryParse(reader.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ? d : 0,
            _ => 0
        };
    }

    /// <summary>Reads the first element of a JSON array as a non-nullable double.</summary>
    private static double ReadFirstFromArray(ref Utf8JsonReader reader)
    {
        if (!reader.Read() || reader.TokenType == JsonTokenType.EndArray) return 0;
        var result = reader.TokenType switch
        {
            JsonTokenType.Number when reader.TryGetDouble(out var d) => d,
            JsonTokenType.String when double.TryParse(reader.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var d) => d,
            _ => 0d
        };
        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray) { } // skip rest
        return result;
    }

    public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}

// ── Live Data DTOs ──

public class OpenF1CarData
{
    [JsonPropertyName("driver_number")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int DriverNumber { get; set; }

    [JsonPropertyName("session_key")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int SessionKey { get; set; }

    [JsonPropertyName("speed")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int Speed { get; set; }

    [JsonPropertyName("rpm")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int Rpm { get; set; }

    [JsonPropertyName("n_gear")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int NGear { get; set; }

    [JsonPropertyName("throttle")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int Throttle { get; set; }

    [JsonPropertyName("brake")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int Brake { get; set; }

    [JsonPropertyName("drs")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int Drs { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }
}

public class OpenF1Interval
{
    [JsonPropertyName("driver_number")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int DriverNumber { get; set; }

    [JsonPropertyName("session_key")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int SessionKey { get; set; }

    [JsonPropertyName("gap_to_leader")]
    [JsonConverter(typeof(SafeDoubleConverter))]
    public double? GapToLeader { get; set; }

    [JsonPropertyName("interval")]
    [JsonConverter(typeof(SafeDoubleConverter))]
    public double? Interval { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }
}

public class OpenF1Location
{
    [JsonPropertyName("driver_number")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int DriverNumber { get; set; }

    [JsonPropertyName("session_key")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int SessionKey { get; set; }

    [JsonPropertyName("x")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int X { get; set; }

    [JsonPropertyName("y")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int Y { get; set; }

    [JsonPropertyName("z")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int Z { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }
}

public class OpenF1RaceControl
{
    [JsonPropertyName("driver_number")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int DriverNumber { get; set; }

    [JsonPropertyName("session_key")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int SessionKey { get; set; }

    [JsonPropertyName("category")]
    public string? Category { get; set; }

    [JsonPropertyName("flag")]
    public string? Flag { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("lap_number")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int LapNumber { get; set; }

    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }
}

public class OpenF1Pit
{
    [JsonPropertyName("driver_number")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int DriverNumber { get; set; }

    [JsonPropertyName("session_key")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int SessionKey { get; set; }

    [JsonPropertyName("lap_number")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int LapNumber { get; set; }

    [JsonPropertyName("stop_duration")]
    [JsonConverter(typeof(SafeNonNullDoubleConverter))]
    public double StopDuration { get; set; }

    [JsonPropertyName("lane_duration")]
    [JsonConverter(typeof(SafeNonNullDoubleConverter))]
    public double LaneDuration { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }
}

public class OpenF1Stint
{
    [JsonPropertyName("driver_number")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int DriverNumber { get; set; }

    [JsonPropertyName("session_key")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int SessionKey { get; set; }

    [JsonPropertyName("compound")]
    public string? Compound { get; set; }

    [JsonPropertyName("lap_start")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int LapStart { get; set; }

    [JsonPropertyName("lap_end")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int LapEnd { get; set; }

    [JsonPropertyName("stint_number")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int StintNumber { get; set; }

    [JsonPropertyName("tyre_age_at_start")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int TyreAgeAtStart { get; set; }
}

public class OpenF1Overtake
{
    [JsonPropertyName("overtaking_driver_number")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int OvertakingDriverNumber { get; set; }

    [JsonPropertyName("overtaken_driver_number")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int OvertakenDriverNumber { get; set; }

    [JsonPropertyName("session_key")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int SessionKey { get; set; }

    [JsonPropertyName("position")]
    [JsonConverter(typeof(SafeIntConverter))]
    public int Position { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }
}
