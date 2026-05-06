using System.Text.Json.Serialization;

namespace Chicane.Mobile.Models.Api;

// ──────── Sportmonks v3 Response ────────
public class SportmonksResponse<T>
{
    [JsonPropertyName("data")]
    public List<T> Data { get; set; } = new();
}

// ──────── Standings ────────
public class SportmonksStanding
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("participant_id")]
    public int ParticipantId { get; set; }

    [JsonPropertyName("position")]
    public int Position { get; set; }

    [JsonPropertyName("points")]
    public int Points { get; set; }

    [JsonPropertyName("participant")]
    public SportmonksParticipant? Participant { get; set; }

    // For constructor standings
    [JsonPropertyName("team")]
    public SportmonksTeam? Team { get; set; }
}

public class SportmonksParticipant
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("short_code")]
    public string ShortCode { get; set; } = string.Empty;

    [JsonPropertyName("team")]
    public SportmonksTeam? Team { get; set; }
}

public class SportmonksTeam
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

// ──────── Fixtures / Schedule ────────
public class SportmonksFixture
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("starting_at")]
    public string StartingAt { get; set; } = string.Empty;

    [JsonPropertyName("venue")]
    public SportmonksVenue? Venue { get; set; }

    [JsonPropertyName("has_results")]
    public bool HasResults { get; set; }

    [JsonPropertyName("league")]
    public SportmonksLeague? League { get; set; }
}

public class SportmonksVenue
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("city")]
    public string City { get; set; } = string.Empty;

    [JsonPropertyName("country")]
    public SportmonksCountry? Country { get; set; }
}

public class SportmonksCountry
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public class SportmonksLeague
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

// ──────── Season ────────
public class SportmonksSeason
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("league_id")]
    public int LeagueId { get; set; }
}
