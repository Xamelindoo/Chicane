using System.Diagnostics;
using System.Net.Http.Json;
using Chicane.Mobile.Models;
using Chicane.Mobile.Models.Api;

namespace Chicane.Mobile.Services;

public class SportmonksService : IF1DataService, IRaceCalendarService
{
    private readonly HttpClient _http;
    private const string BaseUrl = "https://api.sportmonks.com/v3/motorsport";
    private const int F1LeagueId = 3468;
    private static int? _currentSeasonId;

    public SportmonksService(HttpClient http)
    {
        _http = http;
    }

    public async Task<RaceEvent> GetNextEventAsync(string categoryId)
    {
        try
        {
            var from = DateTime.UtcNow.ToString("yyyy-MM-dd");
            var to = DateTime.UtcNow.AddMonths(3).ToString("yyyy-MM-dd");

            var response = await _http.GetFromJsonAsync<SportmonksResponse<SportmonksFixture>>(
                $"{BaseUrl}/fixtures/between/{from}/{to}?api_token={SecretsConfig.SportmonksApiKey}&league_id={F1LeagueId}&include=venue.country");

            var fixture = response?.Data.FirstOrDefault();
            if (fixture is null) return DefaultEvent();

            DateTime.TryParse(fixture.StartingAt, out var date);
            return new RaceEvent
            {
                EventName = fixture.Name.ToUpper().Replace("FORMULA 1 ", ""),
                ScheduledDateTime = date,
                Circuit = new CircuitInfo
                {
                    Name = fixture.Venue?.Name ?? "TBD",
                    Country = fixture.Venue?.Country?.Name ?? "",
                },
                Weather = new WeatherInfo { Condition = "N/A", TemperatureC = 20, IconGlyph = "\u2600" }
            };
        }
        catch (Exception ex) { Debug.WriteLine($"[SM] GetNextEvent: {ex.Message}"); return DefaultEvent(); }
    }

    public async Task<LiveSessionData> GetLiveSessionDataAsync(string categoryId)
    {
        try { var s = await GetSeasonAsync(); if (s is null) return new(); }
        catch { return new(); }
        return new LiveSessionData
        {
            SessionName = "RACE", Status = "LIVE", CurrentLap = 28, TotalLaps = 52,
            PositionBoard = new List<DriverGap>
            {
                new() { Position = 1, DriverName = "M. VERSTAPPEN", TeamName = "Red Bull Racing", GapDisplay = "Leader" },
                new() { Position = 2, DriverName = "L. NORRIS", TeamName = "McLaren", GapDisplay = "+0.892" },
                new() { Position = 3, DriverName = "C. LECLERC", TeamName = "Ferrari", GapDisplay = "+2.134" },
            }
        };
    }

    public async Task<List<StandingsEntry>> GetStandingsAsync(string categoryId)
    {
        try
        {
            var seasonId = await GetSeasonAsync();
            if (seasonId is null) return new();

            var response = await _http.GetFromJsonAsync<SportmonksResponse<SportmonksStanding>>(
                $"{BaseUrl}/standings/seasons/{seasonId}?api_token={SecretsConfig.SportmonksApiKey}&include=participant.team");

            return (response?.Data ?? new()).Select(s => new StandingsEntry
            {
                Position = s.Position,
                DriverName = s.Participant?.Name ?? $"#{s.ParticipantId}",
                TeamName = s.Participant?.Team?.Name ?? "Unknown",
                Points = s.Points,
            }).ToList();
        }
        catch { return new(); }
    }

    public async Task<List<ConstructorStandingsEntry>> GetConstructorStandingsAsync(string categoryId)
    {
        try
        {
            var seasonId = await GetSeasonAsync();
            if (seasonId is null) return new();

            var response = await _http.GetFromJsonAsync<SportmonksResponse<SportmonksStanding>>(
                $"{BaseUrl}/standings/seasons/{seasonId}?api_token={SecretsConfig.SportmonksApiKey}&include=participant.team");

            var teamPoints = new Dictionary<string, int>();
            foreach (var s in response?.Data ?? new())
            {
                var teamName = s.Participant?.Team?.Name;
                if (string.IsNullOrWhiteSpace(teamName)) continue;
                if (!teamPoints.ContainsKey(teamName)) teamPoints[teamName] = 0;
                teamPoints[teamName] += s.Points;
            }
            return teamPoints.OrderByDescending(kv => kv.Value)
                .Select((kv, i) => new ConstructorStandingsEntry { Position = i + 1, TeamName = kv.Key, Points = kv.Value })
                .Take(10).ToList();
        }
        catch { return new(); }
    }

    public Task<List<TrackPosition>> GetTrackPositionsAsync(string categoryId) =>
        Task.FromResult(new List<TrackPosition>
        {
            new() { Position = 1, DriverName = "M. VERSTAPPEN", Sector = 3, LastTime = "1:24.332" },
            new() { Position = 2, DriverName = "L. NORRIS", Sector = 3, LastTime = "1:24.510" },
            new() { Position = 3, DriverName = "C. LECLERC", Sector = 2, LastTime = "1:24.810" },
        });

    private async Task<int?> GetSeasonAsync()
    {
        if (_currentSeasonId.HasValue) return _currentSeasonId;
        try
        {
            var response = await _http.GetFromJsonAsync<SportmonksResponse<SportmonksSeason>>(
                $"{BaseUrl}/seasons?api_token={SecretsConfig.SportmonksApiKey}&league_id={F1LeagueId}");
            _currentSeasonId = response?.Data.FirstOrDefault()?.Id;
            return _currentSeasonId;
        }
        catch { return null; }
    }

    private static RaceEvent DefaultEvent() => new()
    {
        EventName = "NEXT RACE", ScheduledDateTime = DateTime.UtcNow.AddDays(7),
        Circuit = new CircuitInfo { Name = "TBD" },
        Weather = new WeatherInfo { Condition = "N/A", TemperatureC = 20, IconGlyph = "\u2600" }
    };

    // ─────────── IRaceCalendarService for F1 ───────────
    public async Task<List<RaceScheduleEntry>> GetUpcomingRacesAsync(string categoryId)
    {
        try
        {
            var from = DateTime.UtcNow.ToString("yyyy-MM-dd");
            var to = DateTime.UtcNow.AddMonths(6).ToString("yyyy-MM-dd");

            var response = await _http.GetFromJsonAsync<SportmonksResponse<SportmonksFixture>>(
                $"{BaseUrl}/fixtures/between/{from}/{to}?api_token={SecretsConfig.SportmonksApiKey}&league_id={F1LeagueId}&include=venue.country");

            return (response?.Data ?? new()).Select(f =>
            {
                DateTime.TryParse(f.StartingAt, out var date);
                return new RaceScheduleEntry
                {
                    EventName = f.Name.ToUpper().Replace("FORMULA 1 ", ""),
                    CircuitName = f.Venue?.Name ?? "TBD",
                    Country = f.Venue?.Country?.Name ?? "",
                    EventDate = date,
                    WeatherCondition = "N/A",
                    WeatherIcon = "\u2600",
                    TemperatureC = 20,
                    IsCompleted = f.HasResults,
                };
            }).OrderBy(r => r.EventDate).ToList();
        }
        catch { return new(); }
    }

    public async Task<List<RaceScheduleEntry>> GetPastRacesAsync(string categoryId)
    {
        try
        {
            var to = DateTime.UtcNow.ToString("yyyy-MM-dd");
            var from = DateTime.UtcNow.AddMonths(-6).ToString("yyyy-MM-dd");

            var response = await _http.GetFromJsonAsync<SportmonksResponse<SportmonksFixture>>(
                $"{BaseUrl}/fixtures/between/{from}/{to}?api_token={SecretsConfig.SportmonksApiKey}&league_id={F1LeagueId}&include=venue.country");

            return (response?.Data ?? new())
                .Where(f => f.HasResults)
                .Select(f =>
                {
                    DateTime.TryParse(f.StartingAt, out var date);
                    return new RaceScheduleEntry
                    {
                        EventName = f.Name.ToUpper().Replace("FORMULA 1 ", ""),
                        CircuitName = f.Venue?.Name ?? "TBD",
                        Country = f.Venue?.Country?.Name ?? "",
                        EventDate = date,
                        WeatherCondition = "N/A",
                        WeatherIcon = "\u2600",
                        TemperatureC = 20,
                        IsCompleted = true,
                    };
                }).OrderByDescending(r => r.EventDate).ToList();
        }
        catch { return new(); }
    }

    public async Task<PastRaceDetail> GetPastRaceDetailAsync(string eventName, string categoryId)
    {
        try
        {
            var to = DateTime.UtcNow.ToString("yyyy-MM-dd");
            var from = DateTime.UtcNow.AddMonths(-6).ToString("yyyy-MM-dd");

            var fixturesResponse = await _http.GetFromJsonAsync<SportmonksResponse<SportmonksFixture>>(
                $"{BaseUrl}/fixtures/between/{from}/{to}?api_token={SecretsConfig.SportmonksApiKey}&league_id={F1LeagueId}");

            var fixture = fixturesResponse?.Data
                .FirstOrDefault(f => f.HasResults && f.Name.ToUpper().Contains(eventName.ToUpper()));

            if (fixture is null) return EmptyDetail(eventName);

            var resultsResponse = await _http.GetFromJsonAsync<SportmonksResponse<SportmonksFixtureResult>>(
                $"{BaseUrl}/fixtures/{fixture.Id}/results?api_token={SecretsConfig.SportmonksApiKey}&include=participant.team");

            DateTime.TryParse(fixture.StartingAt, out var date);
            var results = (resultsResponse?.Data ?? new())
                .Select(r => new RaceResultEntry
                {
                    Position = r.Position,
                    DriverName = r.Participant?.Name ?? $"#{r.ParticipantId}",
                    TeamName = r.Participant?.Team?.Name ?? "Unknown",
                    TimeOrDnf = r.Status == "Retired" || r.Status == "Did not finish" ? "DNF" : r.Time?.Value ?? "--:--",
                    IsDnf = r.Status != "Finished",
                    IsFastestLap = r.FastestLapRank == 1,
                })
                .OrderBy(r => r.IsDnf ? 1 : 0)
                .ThenBy(r => r.IsDnf ? r.DriverName : "")
                .ThenBy(r => r.Position)
                .ToList();

            return new PastRaceDetail
            {
                EventName = eventName,
                EventDate = date,
                YellowFlagCount = 0,
                Results = results,
                DriverStandings = new(),
                ConstructorStandings = new(),
            };
        }
        catch { return EmptyDetail(eventName); }
    }

    private static PastRaceDetail EmptyDetail(string name) => new()
    {
        EventName = name, EventDate = DateTime.UtcNow.AddDays(-30),
        Results = new() { new() { Position = 1, DriverName = "Loading...", TeamName = "TBD", TimeOrDnf = "--:--" } },
    };
}
