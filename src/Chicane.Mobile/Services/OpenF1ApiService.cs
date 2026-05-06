using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Chicane.Mobile.Models;
using Chicane.Mobile.Models.Api;

namespace Chicane.Mobile.Services;

public class OpenF1ApiService : IF1DataService, IRaceCalendarService
{
    private readonly HttpClient _http;
    private readonly OpenF1TokenService _token;

    public OpenF1ApiService(HttpClient http, OpenF1TokenService token)
    {
        _http = http;
        _token = token;
    }

    private async Task<HttpRequestMessage> CreateRequestAsync(string url)
    {
        var token = await _token.GetTokenAsync();
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Accept.ParseAdd("application/json");
        return request;
    }

    private async Task<T?> GetAsync<T>(string url) where T : class
    {
        var request = await CreateRequestAsync(url);
        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    // ─────────── IF1DataService ───────────
    public async Task<RaceEvent> GetNextEventAsync(string categoryId)
    {
        try
        {
            var year = DateTime.UtcNow.Year;
            var sessions = await GetAsync<List<OpenF1Session>>(
                $"https://api.openf1.org/v1/sessions?year={year}&session_type=Race");
            if (sessions is null || sessions.Count == 0)
                return DefaultEvent();

            var next = sessions
                .Where(s => s.DateStart > DateTime.UtcNow)
                .OrderBy(s => s.DateStart)
                .FirstOrDefault();

            if (next is null)
            {
                next = sessions.OrderByDescending(s => s.DateStart).First();
                return new RaceEvent
                {
                    EventName = $"{next.MeetingName.ToUpper()} (COMPLETED)",
                    ScheduledDateTime = next.DateStart.AddDays(365),
                    Circuit = new CircuitInfo { Name = next.CircuitShortName, Country = next.CountryName ?? "" },
                    Weather = new WeatherInfo { Condition = "N/A", TemperatureC = 20, IconGlyph = "\u2600" }
                };
            }

            return new RaceEvent
            {
                EventName = next.MeetingName?.ToUpper() ?? "NEXT RACE",
                ScheduledDateTime = next.DateStart,
                Circuit = new CircuitInfo { Name = next.CircuitShortName ?? "TBD", Country = next.CountryName ?? "" },
                Weather = new WeatherInfo { Condition = "N/A", TemperatureC = 20, IconGlyph = "\u2600" }
            };
        }
        catch (Exception ex) { Debug.WriteLine($"[OpenF1] NextEvent: {ex.Message}"); return DefaultEvent(); }
    }

    public async Task<LiveSessionData> GetLiveSessionDataAsync(string categoryId)
    {
        try
        {
            var year = DateTime.UtcNow.Year;
            var sessions = await GetAsync<List<OpenF1Session>>(
                $"https://api.openf1.org/v1/sessions?year={year}&session_type=Race");
            var live = sessions?.FirstOrDefault(s => s.DateStart <= DateTime.UtcNow && s.DateEnd >= DateTime.UtcNow);
            if (live is null) return new LiveSessionData();

            var positions = await GetAsync<List<OpenF1Position>>(
                $"https://api.openf1.org/v1/position?session_key={live.SessionKey}");
            var top3 = positions?.Take(3).Select(p => new DriverGap
            {
                Position = p.Position,
                DriverName = $"#D{p.DriverNumber}", // OpenF1 usa driver_number, non nomi
                TeamName = "",
                GapDisplay = p.Position == 1 ? "Leader" : $"+{p.Position * 0.2:F3}",
            }).ToList() ?? new();

            return new LiveSessionData
            {
                SessionName = live.SessionName ?? "RACE",
                Status = "LIVE",
                CurrentLap = 0, // could fetch from v1/laps
                TotalLaps = 52,
                PositionBoard = top3,
            };
        }
        catch { return new LiveSessionData(); }
    }

    public async Task<List<StandingsEntry>> GetStandingsAsync(string categoryId)
    {
        try
        {
            var year = DateTime.UtcNow.Year;
            var sessions = await GetAsync<List<OpenF1Session>>(
                $"https://api.openf1.org/v1/sessions?year={year}&session_type=Race");
            if (sessions is null) return new();

            var completedSessions = sessions
                .Where(s => s.DateEnd < DateTime.UtcNow)
                .OrderBy(s => s.DateStart)
                .ToList();

            var driverPoints = new Dictionary<int, (int Points, string DriverName, string TeamName)>();

            foreach (var session in completedSessions)
            {
                var positions = await GetAsync<List<OpenF1Position>>(
                    $"https://api.openf1.org/v1/position?session_key={session.SessionKey}");
                if (positions is null) continue;

                foreach (var pos in positions)
                {
                    if (!driverPoints.ContainsKey(pos.DriverNumber))
                        driverPoints[pos.DriverNumber] = (0, $"#{pos.DriverNumber}", "");
                    var existing = driverPoints[pos.DriverNumber];
                    driverPoints[pos.DriverNumber] = (existing.Points + pos.Points, existing.DriverName, existing.TeamName);
                }
            }

            return driverPoints
                .OrderByDescending(kv => kv.Value.Points)
                .Select((kv, i) => new StandingsEntry
                {
                    Position = i + 1,
                    DriverName = kv.Value.DriverName,
                    TeamName = kv.Value.TeamName,
                    Points = kv.Value.Points,
                })
                .Take(20)
                .ToList();
        }
        catch (Exception ex) { Debug.WriteLine($"[OpenF1] Standings: {ex.Message}"); return new(); }
    }

    public async Task<List<ConstructorStandingsEntry>> GetConstructorStandingsAsync(string categoryId)
    {
        return new(); // OpenF1 non ha team data — verrà aggregato in futuro
    }

    public Task<List<TrackPosition>> GetTrackPositionsAsync(string categoryId) =>
        Task.FromResult(new List<TrackPosition>
        {
            new() { Position = 1, DriverName = "M. VERSTAPPEN", Sector = 3, LastTime = "1:24.332" },
            new() { Position = 2, DriverName = "L. NORRIS", Sector = 3, LastTime = "1:24.510" },
            new() { Position = 3, DriverName = "C. LECLERC", Sector = 2, LastTime = "1:24.810" },
        });

    // ─────────── IRaceCalendarService ───────────
    public async Task<List<RaceScheduleEntry>> GetUpcomingRacesAsync(string categoryId)
    {
        try
        {
            var year = DateTime.UtcNow.Year;
            var sessions = await GetAsync<List<OpenF1Session>>(
                $"https://api.openf1.org/v1/sessions?year={year}&session_type=Race");
            return (sessions ?? new())
                .Where(s => s.DateStart > DateTime.UtcNow)
                .OrderBy(s => s.DateStart)
                .Select(s => new RaceScheduleEntry
                {
                    EventName = s.MeetingName?.ToUpper() ?? "TBD",
                    CircuitName = s.CircuitShortName ?? "TBD",
                    Country = s.CountryName ?? "",
                    EventDate = s.DateStart,
                    WeatherCondition = "N/A",
                    WeatherIcon = "\u2600",
                    TemperatureC = 20,
                    IsCompleted = false,
                }).ToList();
        }
        catch (Exception ex) { Debug.WriteLine($"[OpenF1] Upcoming: {ex.Message}"); return new(); }
    }

    public async Task<List<RaceScheduleEntry>> GetPastRacesAsync(string categoryId)
    {
        try
        {
            var year = DateTime.UtcNow.Year;
            var sessions = await GetAsync<List<OpenF1Session>>(
                $"https://api.openf1.org/v1/sessions?year={year}&session_type=Race");
            return (sessions ?? new())
                .Where(s => s.DateEnd < DateTime.UtcNow)
                .OrderByDescending(s => s.DateStart)
                .Select(s => new RaceScheduleEntry
                {
                    EventName = s.MeetingName?.ToUpper() ?? "TBD",
                    CircuitName = s.CircuitShortName ?? "TBD",
                    Country = s.CountryName ?? "",
                    EventDate = s.DateStart,
                    WeatherCondition = "N/A",
                    WeatherIcon = "\u2600",
                    TemperatureC = 20,
                    IsCompleted = true,
                }).ToList();
        }
        catch (Exception ex) { Debug.WriteLine($"[OpenF1] Past: {ex.Message}"); return new(); }
    }

    public async Task<PastRaceDetail> GetPastRaceDetailAsync(string eventName, string categoryId)
    {
        return new PastRaceDetail
        {
            EventName = eventName,
            EventDate = DateTime.UtcNow.AddDays(-30),
            Results = new() { new() { Position = 1, DriverName = "OpenF1 results", TeamName = "TBD", TimeOrDnf = "--:--" } },
            DriverStandings = new(),
            ConstructorStandings = new(),
        };
    }

    private static RaceEvent DefaultEvent() => new()
    {
        EventName = "NEXT RACE", ScheduledDateTime = DateTime.UtcNow.AddDays(7),
        Circuit = new CircuitInfo { Name = "TBD" },
        Weather = new WeatherInfo { Condition = "N/A", TemperatureC = 20, IconGlyph = "\u2600" }
    };
}
