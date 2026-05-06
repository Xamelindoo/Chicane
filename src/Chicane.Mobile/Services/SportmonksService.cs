using System.Net.Http.Json;
using System.Text.Json;
using Chicane.Mobile.Models;
using Chicane.Mobile.Models.Api;

namespace Chicane.Mobile.Services;

public class SportmonksService : IF1DataService
{
    private readonly HttpClient _http;
    private const string BaseUrl = "https://api.sportmonks.com/v3/motorsport";

    // Sportmonks league IDs (discovered via API)
    private const int MotoGpLeagueId = 1; // MotoGP league ID
    private static int? _currentSeasonId;

    public SportmonksService(HttpClient http)
    {
        _http = http;
    }

    public async Task<RaceEvent> GetNextEventAsync(string categoryId)
    {
        if (categoryId != "motogp") return DefaultRaceEvent();

        try
        {
            var from = DateTime.UtcNow.ToString("yyyy-MM-dd");
            var to = DateTime.UtcNow.AddMonths(3).ToString("yyyy-MM-dd");

            var response = await _http.GetFromJsonAsync<SportmonksResponse<SportmonksFixture>>(
                $"{BaseUrl}/fixtures/between/{from}/{to}?api_token={SecretsConfig.SportmonksApiKey}&league_id={MotoGpLeagueId}&include=venue.country");

            var fixture = response?.Data.FirstOrDefault();
            if (fixture is null) return DefaultRaceEvent();

            DateTime.TryParse(fixture.StartingAt, out var date);

            return new RaceEvent
            {
                EventName = fixture.Name.ToUpper(),
                ScheduledDateTime = date,
                Circuit = new CircuitInfo
                {
                    Name = fixture.Venue?.Name ?? "TBD",
                    Country = fixture.Venue?.Country?.Name ?? "",
                },
                Weather = new WeatherInfo { Condition = "N/A", TemperatureC = 25, IconGlyph = "\u2600" }
            };
        }
        catch
        {
            return DefaultRaceEvent();
        }
    }

    public async Task<LiveSessionData> GetLiveSessionDataAsync(string categoryId)
    {
        return new LiveSessionData();
    }

    public async Task<List<StandingsEntry>> GetStandingsAsync(string categoryId)
    {
        if (categoryId != "motogp") return new();

        try
        {
            var seasonId = await GetCurrentSeasonIdAsync();
            if (seasonId is null) return new();

            var response = await _http.GetFromJsonAsync<SportmonksResponse<SportmonksStanding>>(
                $"{BaseUrl}/standings/seasons/{seasonId}?api_token={SecretsConfig.SportmonksApiKey}&include=participant.team");

            return response?.Data.Select(s => new StandingsEntry
            {
                Position = s.Position,
                DriverName = s.Participant?.ShortCode ?? s.Participant?.Name ?? $"#{s.ParticipantId}",
                TeamName = s.Participant?.Team?.Name ?? "Unknown",
                Points = s.Points,
            }).ToList() ?? new();
        }
        catch
        {
            return new();
        }
    }

    public async Task<List<ConstructorStandingsEntry>> GetConstructorStandingsAsync(string categoryId)
    {
        if (categoryId != "motogp") return new();

        try
        {
            var seasonId = await GetCurrentSeasonIdAsync();
            if (seasonId is null) return new();

            var response = await _http.GetFromJsonAsync<SportmonksResponse<SportmonksStanding>>(
                $"{BaseUrl}/standings/seasons/{seasonId}?api_token={SecretsConfig.SportmonksApiKey}&include=team");

            var teamPoints = new Dictionary<string, int>();
            foreach (var s in response?.Data ?? new())
            {
                var teamName = s.Participant?.Team?.Name ?? s.Team?.Name;
                if (string.IsNullOrWhiteSpace(teamName)) continue;

                if (!teamPoints.ContainsKey(teamName))
                    teamPoints[teamName] = 0;
                teamPoints[teamName] += s.Points;
            }

            return teamPoints
                .OrderByDescending(kv => kv.Value)
                .Select((kv, i) => new ConstructorStandingsEntry
                {
                    Position = i + 1,
                    TeamName = kv.Key,
                    Points = kv.Value,
                })
                .Take(10)
                .ToList();
        }
        catch
        {
            return new();
        }
    }

    public async Task<List<TrackPosition>> GetTrackPositionsAsync(string categoryId)
    {
        return new();
    }

    private async Task<int?> GetCurrentSeasonIdAsync()
    {
        if (_currentSeasonId.HasValue) return _currentSeasonId;

        try
        {
            var response = await _http.GetFromJsonAsync<SportmonksResponse<SportmonksSeason>>(
                $"{BaseUrl}/seasons?api_token={SecretsConfig.SportmonksApiKey}&league_id={MotoGpLeagueId}");

            var current = response?.Data
                .Where(s => s.Name.Contains("2025"))
                .FirstOrDefault();

            _currentSeasonId = current?.Id;
            return _currentSeasonId;
        }
        catch
        {
            return null;
        }
    }

    private static RaceEvent DefaultRaceEvent() => new()
    {
        EventName = "NEXT RACE",
        ScheduledDateTime = DateTime.UtcNow.AddDays(7),
        Circuit = new CircuitInfo { Name = "TBD" },
        Weather = new WeatherInfo { Condition = "N/A", TemperatureC = 20, IconGlyph = "\u2600" }
    };
}
