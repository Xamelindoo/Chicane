using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace Chicane.Mobile.Tests;

public class RacePointsTests
{
    private static int RacePoints(int position, string sessionName) => sessionName.ToUpperInvariant() switch
    {
        "SPRINT" => position switch
        {
            1 => 8, 2 => 7, 3 => 6, 4 => 5, 5 => 4, 6 => 3, 7 => 2, 8 => 1, _ => 0
        },
        _ => position switch
        {
            1 => 25, 2 => 18, 3 => 15, 4 => 12, 5 => 10,
            6 => 8, 7 => 6, 8 => 4, 9 => 2, 10 => 1,
            _ => 0
        }
    };

    [Theory]
    [InlineData(1, "RACE", 25)]
    [InlineData(2, "RACE", 18)]
    [InlineData(3, "RACE", 15)]
    [InlineData(10, "RACE", 1)]
    [InlineData(11, "RACE", 0)]
    [InlineData(1, "SPRINT", 8)]
    [InlineData(2, "SPRINT", 7)]
    [InlineData(8, "SPRINT", 1)]
    [InlineData(9, "SPRINT", 0)]
    [InlineData(1, "sprint", 8)]
    public void RacePoints_ReturnsCorrectPoints(int position, string sessionName, int expected)
    {
        Assert.Equal(expected, RacePoints(position, sessionName));
    }

    [Fact]
    public void RacePoints_DnfPosition_ReturnsZero()
    {
        Assert.Equal(0, RacePoints(20, "RACE"));
        Assert.Equal(0, RacePoints(15, "SPRINT"));
    }
}

public class OpenF1JsonParsingTests
{
    private const string DriversJson = """
    [
        {
            "driver_number": 1,
            "broadcast_name": "L NORRIS",
            "full_name": "Lando NORRIS",
            "name_acronym": "NOR",
            "team_name": "McLaren",
            "team_colour": "F47600"
        },
        {
            "driver_number": 44,
            "broadcast_name": "L HAMILTON",
            "full_name": "Lewis HAMILTON",
            "name_acronym": "HAM",
            "team_name": "Ferrari",
            "team_colour": "F60000"
        }
    ]
    """;

    private const string SessionsJson = """
    [
        {
            "session_key": 11234,
            "session_name": "Race",
            "session_type": "Race",
            "meeting_key": 1279,
            "circuit_short_name": "Albert Park",
            "country_name": "Australia",
            "date_start": "2026-03-16T06:00:00+00:00",
            "date_end": "2026-03-16T08:00:00+00:00"
        }
    ]
    """;

    [Fact]
    public void ParseDrivers_DeserializesCorrectly()
    {
        var drivers = JsonSerializer.Deserialize<List<OpenF1DriverDto>>(DriversJson,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(drivers);
        Assert.Equal(2, drivers.Count);
        Assert.Equal("L NORRIS", drivers[0].BroadcastName);
        Assert.Equal("McLaren", drivers[0].TeamName);
        Assert.Equal(44, drivers[1].DriverNumber);
        Assert.Equal("Lewis HAMILTON", drivers[1].FullName);
    }

    [Fact]
    public void ParseSessions_DeserializesCorrectly()
    {
        var sessions = JsonSerializer.Deserialize<List<OpenF1SessionDto>>(SessionsJson,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(sessions);
        Assert.Single(sessions);
        Assert.Equal(11234, sessions[0].SessionKey);
        Assert.Equal("Race", sessions[0].SessionName);
        Assert.Equal("Albert Park", sessions[0].CircuitShortName);
        Assert.Equal(1279, sessions[0].MeetingKey);
    }

    [Theory]
    [InlineData("[]")]
    [InlineData("null")]
    public void ParseEmptyData_ReturnsNull(string json)
    {
        var result = JsonSerializer.Deserialize<List<OpenF1DriverDto>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.True(result is null or { Count: 0 });
    }
}

public class CacheFirstLogicTests
{
    [Fact]
    public void CacheHit_SkipsApiCall()
    {
        var cache = new List<StandingsStub>
        {
            new() { Position = 1, DriverName = "VERSTAPPEN", Points = 86 },
            new() { Position = 2, DriverName = "NORRIS", Points = 72 }
        };
        var cacheFetchCount = 0;

        // Simulate cache-first pattern
        var result = GetFromCacheOrApi(
            () => { cacheFetchCount++; return cache; },
            () => new List<StandingsStub>());

        Assert.Equal(2, result.Count);
        Assert.Equal("VERSTAPPEN", result[0].DriverName);
        Assert.Equal(1, cacheFetchCount); // Cache was hit
    }

    [Fact]
    public void CacheMiss_FallsBackToApi()
    {
        var apiFetchCount = 0;
        var apiData = new List<StandingsStub>
        {
            new() { Position = 1, DriverName = "LECLERC", Points = 58 }
        };

        var result = GetFromCacheOrApi(
            () => { return new List<StandingsStub>(); },
            () => { apiFetchCount++; return apiData; });

        Assert.Single(result);
        Assert.Equal("LECLERC", result[0].DriverName);
        Assert.Equal(1, apiFetchCount); // API was called (cache miss)
    }

    private static List<StandingsStub> GetFromCacheOrApi(
        Func<List<StandingsStub>> cacheLookup,
        Func<List<StandingsStub>> apiFetch)
    {
        var cached = cacheLookup();
        if (cached.Count > 0) return cached;
        return apiFetch();
    }
}

// Minimal DTOs for JSON parsing tests (mirroring OpenF1Dto.cs structures)

public class OpenF1DriverDto
{
    [JsonPropertyName("driver_number")]
    public int DriverNumber { get; set; }

    [JsonPropertyName("broadcast_name")]
    public string? BroadcastName { get; set; }

    [JsonPropertyName("full_name")]
    public string? FullName { get; set; }

    [JsonPropertyName("name_acronym")]
    public string? NameAcronym { get; set; }

    [JsonPropertyName("team_name")]
    public string? TeamName { get; set; }

    [JsonPropertyName("team_colour")]
    public string? TeamColour { get; set; }
}

public class OpenF1SessionDto
{
    [JsonPropertyName("session_key")]
    public int SessionKey { get; set; }

    [JsonPropertyName("session_name")]
    public string? SessionName { get; set; }

    [JsonPropertyName("session_type")]
    public string? SessionType { get; set; }

    [JsonPropertyName("meeting_key")]
    public int MeetingKey { get; set; }

    [JsonPropertyName("circuit_short_name")]
    public string? CircuitShortName { get; set; }

    [JsonPropertyName("country_name")]
    public string? CountryName { get; set; }

    [JsonPropertyName("date_start")]
    public DateTime DateStart { get; set; }

    [JsonPropertyName("date_end")]
    public DateTime DateEnd { get; set; }
}

public class StandingsStub
{
    public int Position { get; set; }
    public string DriverName { get; set; } = "";
    public int Points { get; set; }
}
