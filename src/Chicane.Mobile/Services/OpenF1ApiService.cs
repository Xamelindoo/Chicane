using System.Diagnostics;
using System.Text.Json;
using Chicane.Mobile.Models;
using Chicane.Mobile.Models.Api;

namespace Chicane.Mobile.Services;

public class OpenF1ApiService : IF1DataService, IRaceCalendarService
{
    private readonly HttpClient _http;
    private List<OpenF1Meeting>? _meetingsCache;
    private int? _meetingsCacheYear;
    private List<OpenF1Session>? _sessionsCache;
    private int? _sessionsCacheYear;
    private Dictionary<int, OpenF1Driver>? _driverCache;
    private int? _driverCacheSessionKey;

    public OpenF1ApiService(HttpClient http)
    {
        _http = http;
    }

    private async Task<T?> GetAsync<T>(string url) where T : class
    {
        try
        {
            var json = await _http.GetStringAsync(url);
            var preview = json.Length <= 300 ? json : json[..300] + "...";
            Debug.WriteLine($"[OpenF1] {url[..Math.Min(url.Length, 100)]} → {json.Length} bytes: {preview}");
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[OpenF1] {url} → {ex.Message}");
            return null;
        }
    }

    public async Task<Dictionary<int, OpenF1Driver>> GetDriversAsync(int sessionKey)
    {
        if (_driverCache is not null && _driverCacheSessionKey == sessionKey)
            return _driverCache;

        var drivers = await GetAsync<List<OpenF1Driver>>(
            $"https://chicane-api.duckdns.org/api/proxy/drivers?session_key={sessionKey}");
        _driverCache = (drivers ?? new()).ToDictionary(d => d.DriverNumber);
        _driverCacheSessionKey = sessionKey;
        return _driverCache;
    }

    public string DriverName(int driverNumber)
    {
        if (_driverCache?.TryGetValue(driverNumber, out var d) == true)
            return d.BroadcastName ?? d.NameAcronym ?? $"#{driverNumber}";
        return $"#{driverNumber}";
    }

    public string DriverTeam(int driverNumber)
    {
        if (_driverCache?.TryGetValue(driverNumber, out var d) == true)
            return d.TeamName ?? "";
        return "";
    }

    public string DriverTeamColour(int driverNumber)
    {
        if (_driverCache?.TryGetValue(driverNumber, out var d) == true)
            return d.TeamColour ?? "333333";
        return "333333";
    }

    public string DriverHeadshot(int driverNumber)
    {
        if (_driverCache?.TryGetValue(driverNumber, out var d) == true)
            return d.HeadshotUrl ?? "";
        return "";
    }

    public string DriverCode(int driverNumber)
    {
        if (_driverCache?.TryGetValue(driverNumber, out var d) == true)
            return d.NameAcronym ?? d.BroadcastName ?? $"D{driverNumber}";
        return $"D{driverNumber}";
    }

    public string DriverLastName(int driverNumber)
    {
        if (_driverCache?.TryGetValue(driverNumber, out var d) == true)
            return (d.LastName ?? d.FullName ?? $"D{driverNumber}").ToUpperInvariant();
        return $"D{driverNumber}";
    }

    public async Task<List<OpenF1Position>> GetPositionsAsync(int sessionKey)
    {
        try
        {
            var data = await GetAsync<List<OpenF1Position>>(
                $"https://chicane-api.duckdns.org/api/proxy/position?session_key={sessionKey}");
            return data ?? new();
        }
        catch (Exception ex) { Debug.WriteLine($"[OpenF1] Positions: {ex.Message}"); return new(); }
    }

    public async Task<Dictionary<string, string>> GetTeamColorsAsync()
    {
        if (_driverCache is null or { Count: 0 })
        {
            var year = DateTime.UtcNow.Year;
            var sessions = await GetAsync<List<OpenF1Session>>(
                $"https://chicane-api.duckdns.org/api/proxy/sessions?year={year}&session_type=Race");
            var latest = sessions?.FirstOrDefault()
                ?? throw new InvalidOperationException("No sessions found");
            await GetDriversAsync(latest.SessionKey);
        }

        return _driverCache!
            .Where(kv => kv.Value.TeamName is not null && kv.Value.TeamColour is not null)
            .GroupBy(kv => kv.Value.TeamName!)
            .ToDictionary(g => g.Key, g => $"#{g.First().Value.TeamColour}");
    }

    public async Task<DriverDetail?> GetDriverDetailAsync(int driverNumber)
    {
        try
        {
            var year = DateTime.UtcNow.Year;

            // Build session lookup dict (SessionKey → OpenF1Session) for O(1) access
            var sessions = await GetSessionsAsync(year);
            var sessionByKey = (sessions ?? new()).ToDictionary(s => s.SessionKey);

            // Use a Race session to populate the driver cache (all 22 drivers)
            var raceSession = sessions?.FirstOrDefault(s => s.SessionName == "Race");
            if (raceSession is null) return null;

            await GetDriversAsync(raceSession.SessionKey);
            if (_driverCache is null || !_driverCache.TryGetValue(driverNumber, out var driver))
                return null;

            // Fetch standings (all years — filtered client-side) and race results for selected year
            var standings = await GetAsync<List<OpenF1ChampionshipDriver>>(
                $"https://chicane-api.duckdns.org/api/proxy/championship_drivers?driver_number={driverNumber}");
            var currentEntry = standings?.OrderByDescending(s => s.SessionKey).FirstOrDefault();

            var results = await GetAsync<List<OpenF1SessionResult>>(
                $"https://chicane-api.duckdns.org/api/proxy/session_result?driver_number={driverNumber}");

            // Build meeting lookup dict for O(1) meeting name resolution
            var meetings = await GetMeetingsAsync(year);
            var meetingNameByKey = (meetings ?? new()).ToDictionary(m => m.MeetingKey, m => m.MeetingName);

            var raceResults = new List<DriverRaceResult>();
            if (results is not null)
            {
                foreach (var r in results)
                {
                    // Skip results outside the current year (session not in our cache)
                    if (!sessionByKey.TryGetValue(r.SessionKey, out var session))
                        continue;
                    string? meetingName = null;
                    if (session?.MeetingKey > 0)
                        meetingNameByKey.TryGetValue(session.MeetingKey, out meetingName);

                    raceResults.Add(new DriverRaceResult
                    {
                        SessionName = session?.SessionName ?? "",
                        CircuitName = session?.CircuitShortName ?? "",
                        MeetingName = meetingName,
                        SessionKey = r.SessionKey,
                        Position = r.Position,
                        EventDate = session?.DateStart ?? DateTime.UtcNow,
                    });
                }
            }

            return new DriverDetail
            {
                DriverNumber = driverNumber,
                BroadcastName = driver.BroadcastName ?? $"#{driverNumber}",
                FullName = driver.FullName ?? "",
                FirstName = driver.FirstName ?? "",
                LastName = driver.LastName ?? "",
                NameAcronym = driver.NameAcronym ?? "",
                TeamName = driver.TeamName ?? "",
                TeamColour = driver.TeamColour is not null ? $"#{driver.TeamColour}" : "#333333",
                HeadshotUrl = driver.HeadshotUrl ?? "",
                Points = currentEntry is not null ? (int)currentEntry.PointsCurrent : 0,
                Position = currentEntry?.PositionCurrent ?? 0,
                RaceResults = raceResults
                    .OrderByDescending(rr => rr.EventDate)
                    .ToList(),
            };
        }
        catch (Exception ex) { Debug.WriteLine($"[OpenF1] DriverDetail: {ex.Message}"); return null; }
    }

    private async Task<List<OpenF1Meeting>> GetMeetingsAsync(int year)
    {
        if (_meetingsCache is not null && _meetingsCacheYear == year)
            return _meetingsCache;

        _meetingsCache = await GetAsync<List<OpenF1Meeting>>(
            $"https://chicane-api.duckdns.org/api/proxy/meetings?year={year}");
        _meetingsCacheYear = year;
        return _meetingsCache ?? new();
    }

    public async Task<List<OpenF1Session>> GetSessionsAsync(int year)
    {
        if (_sessionsCache is not null && _sessionsCacheYear == year)
            return _sessionsCache;

        _sessionsCache = await GetAsync<List<OpenF1Session>>(
            $"https://chicane-api.duckdns.org/api/proxy/sessions?year={year}");
        _sessionsCacheYear = year;
        return _sessionsCache ?? new();
    }

    private async Task<string?> ResolveMeetingNameAsync(int meetingKey, int year, string fallback)
    {
        var meetings = await GetMeetingsAsync(year);
        return meetings.FirstOrDefault(m => m.MeetingKey == meetingKey)?.MeetingName?.ToUpper() ?? fallback;
    }

    private async Task<string?> ResolveCircuitImageAsync(int meetingKey, int year)
    {
        var meetings = await GetMeetingsAsync(year);
        return meetings.FirstOrDefault(m => m.MeetingKey == meetingKey)?.CircuitImage;
    }

    private static string FormatEventName(OpenF1Session session)
        => session.SessionName?.ToUpper() ?? "RACE";

    // ─────────── IF1DataService ───────────
    public async Task<RaceEvent> GetNextEventAsync(string categoryId)
    {
        try
        {
            var year = DateTime.UtcNow.Year;
            var sessions = await GetSessionsAsync(year);
            if (sessions.Count == 0)
                return DefaultEvent();

            var next = sessions
                .Where(s => s.DateStart > DateTime.UtcNow)
                .OrderBy(s => s.DateStart)
                .FirstOrDefault();

            Debug.WriteLine($"[OpenF1] NextEvent: {sessions.Count} sessions, next={next?.SessionKey} {next?.SessionName} {next?.DateStart:yyyy-MM-ddTHH:mm} {next?.CircuitShortName} {next?.CountryName}");

            if (next is null)
            {
                next = sessions.OrderByDescending(s => s.DateStart).First();
                var name = await ResolveMeetingNameAsync(next.MeetingKey, year, "NEXT RACE");
                return new RaceEvent
                {
                    EventName = $"{FormatEventName(next)} (COMPLETED)",
                    MeetingName = name,
                    CircuitImageUrl = await ResolveCircuitImageAsync(next.MeetingKey, year) ?? "",
                    ScheduledDateTime = next.DateStart.AddDays(365),
                    Circuit = new CircuitInfo { Name = next.CircuitShortName ?? "TBD", Country = next.CountryName ?? "", Location = next.Location ?? "" },
                    Weather = new WeatherInfo { Condition = "N/A", TemperatureC = 20, IconGlyph = "\u2600" },
                    SessionKey = next.SessionKey,
                };
            }

            return new RaceEvent
            {
                EventName = FormatEventName(next),
                MeetingName = await ResolveMeetingNameAsync(next.MeetingKey, year, "NEXT RACE") ?? "NEXT RACE",
                CircuitImageUrl = await ResolveCircuitImageAsync(next.MeetingKey, year) ?? "",
                ScheduledDateTime = next.DateStart,
                Circuit = new CircuitInfo { Name = next.CircuitShortName ?? "TBD", Country = next.CountryName ?? "", Location = next.Location ?? "" },
                Weather = new WeatherInfo { Condition = "N/A", TemperatureC = 20, IconGlyph = "\u2600" },
                SessionKey = next.SessionKey,
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
                $"https://chicane-api.duckdns.org/api/proxy/sessions?year={year}&session_type=Race");
            var live = sessions?.FirstOrDefault(s => s.DateStart <= DateTime.UtcNow && s.DateEnd >= DateTime.UtcNow);
            if (live is null) return new LiveSessionData();
            return await GetLiveDataAsync(live.SessionKey);
        }
        catch (Exception ex) { Debug.WriteLine($"[OpenF1] LiveSession: {ex.Message}"); return new LiveSessionData(); }
    }

    /// <summary>
    /// Fetches live timing data for a specific session: positions, driver info, and intervals.
    /// </summary>
    public async Task<LiveSessionData> GetLiveDataAsync(int sessionKey)
    {
        try
        {
            var sessions = await GetSessionsAsync(DateTime.UtcNow.Year);
            var session = sessions?.FirstOrDefault(s => s.SessionKey == sessionKey);

            await GetDriversAsync(sessionKey);

            var positions = await GetAsync<List<OpenF1Position>>(
                $"https://chicane-api.duckdns.org/api/proxy/position?session_key={sessionKey}");

            // Find latest lap from race_control or laps endpoint
            var laps = await GetAsync<List<OpenF1Lap>>(
                $"https://chicane-api.duckdns.org/api/proxy/laps?session_key={sessionKey}");

            var maxLap = laps?.Select(l => l.LapNumber).DefaultIfEmpty(0).Max() ?? 0;
            var totalLaps = 52; // Default for most races — can be refined per circuit

            var positionBoard = (positions ?? new())
                .OrderBy(p => p.Position)
                .Select(p => new DriverGap
                {
                    Position = p.Position,
                    DriverName = DriverName(p.DriverNumber),
                    TeamName = DriverTeam(p.DriverNumber),
                    HeadshotUrl = DriverHeadshot(p.DriverNumber),
                    GapDisplay = p.Points == 0 ? $"P{p.Position}" : $"+{p.Points * 0.001:F3}s",
                }).ToList();

            return new LiveSessionData
            {
                SessionName = session?.SessionName ?? "LIVE",
                MeetingName = session?.Location ?? "",
                CircuitName = session?.CircuitShortName ?? "",
                Status = "LIVE",
                CurrentLap = maxLap,
                TotalLaps = totalLaps,
                PositionBoard = positionBoard,
            };
        }
        catch (Exception ex) { Debug.WriteLine($"[OpenF1] LiveData: {ex.Message}"); return new LiveSessionData(); }
    }

    public async Task<List<StandingsEntry>> GetStandingsAsync(string categoryId)
    {
        try
        {
            var year = DateTime.UtcNow.Year;
            var sessions = await GetAsync<List<OpenF1Session>>(
                $"https://chicane-api.duckdns.org/api/proxy/sessions?year={year}&session_type=Race");
            if (sessions is null) return new();

            var latestRace = sessions
                .Where(s => s.DateEnd < DateTime.UtcNow && !s.IsCancelled && s.SessionName == "Race")
                .OrderByDescending(s => s.DateStart)
                .FirstOrDefault();

            if (latestRace is null) return new();

            var standings = await GetAsync<List<OpenF1ChampionshipDriver>>(
                $"https://chicane-api.duckdns.org/api/proxy/championship_drivers?session_key={latestRace.SessionKey}");
            if (standings is null || standings.Count == 0) return new();

            await GetDriversAsync(latestRace.SessionKey);

            return standings
                .OrderBy(s => s.PositionCurrent)
                .Select(s => new StandingsEntry
                {
                    Position = s.PositionCurrent,
                    DriverName = DriverName(s.DriverNumber),
                    TeamName = DriverTeam(s.DriverNumber),
                    Points = (int)s.PointsCurrent,
                    DriverNumber = s.DriverNumber,
                    PositionStart = s.PositionStart,
                    HeadshotUrl = DriverHeadshot(s.DriverNumber),
                })
                .ToList();
        }
        catch (Exception ex) { Debug.WriteLine($"[OpenF1] Standings: {ex.Message}"); return new(); }
    }

    public async Task<List<ConstructorStandingsEntry>> GetConstructorStandingsAsync(string categoryId)
    {
        try
        {
            var year = DateTime.UtcNow.Year;
            var sessions = await GetAsync<List<OpenF1Session>>(
                $"https://chicane-api.duckdns.org/api/proxy/sessions?year={year}&session_type=Race");
            if (sessions is null) return new();

            var latestRace = sessions
                .Where(s => s.DateEnd < DateTime.UtcNow && !s.IsCancelled && s.SessionName == "Race")
                .OrderByDescending(s => s.DateStart)
                .FirstOrDefault();

            if (latestRace is null) return new();

            var teams = await GetAsync<List<OpenF1ChampionshipTeam>>(
                $"https://chicane-api.duckdns.org/api/proxy/championship_teams?session_key={latestRace.SessionKey}");
            if (teams is null || teams.Count == 0) return new();

            return teams
                .OrderBy(t => t.PositionCurrent)
                .Select(t => new ConstructorStandingsEntry
                {
                    Position = t.PositionCurrent,
                    TeamName = t.TeamName ?? "",
                    Points = (int)t.PointsCurrent,
                    TeamLogo = TeamLogoFor(t.TeamName ?? ""),
                })
                .ToList();
        }
        catch (Exception ex) { Debug.WriteLine($"[OpenF1] ConstructorStandings: {ex.Message}"); return new(); }
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
                $"https://chicane-api.duckdns.org/api/proxy/sessions?year={year}");
            if (sessions is null) return new();

            var upcoming = sessions
                .Where(s => s.DateStart > DateTime.UtcNow && !s.IsCancelled)
                .OrderBy(s => s.DateStart)
                .ToList();

            var result = new List<RaceScheduleEntry>();
            foreach (var s in upcoming)
            {
                var name = await ResolveMeetingNameAsync(s.MeetingKey, year, "TBD");
                result.Add(new RaceScheduleEntry
                {
                    EventName = FormatEventName(s),
                    MeetingName = await ResolveMeetingNameAsync(s.MeetingKey, year, "TBD") ?? "TBD",
                    CircuitName = s.CircuitShortName ?? "TBD",
                    Country = s.CountryName ?? "",
                    EventDate = s.DateStart,
                    SessionKey = s.SessionKey,
                    MeetingKey = s.MeetingKey,
                    IsCompleted = false,
                });
            }
            return result;
        }
        catch (Exception ex) { Debug.WriteLine($"[OpenF1] Upcoming: {ex.Message}"); return new(); }
    }

    public async Task<List<RaceScheduleEntry>> GetPastRacesAsync(string categoryId)
    {
        try
        {
            var year = DateTime.UtcNow.Year;
            var sessions = await GetAsync<List<OpenF1Session>>(
                $"https://chicane-api.duckdns.org/api/proxy/sessions?year={year}&session_type=Race");
            if (sessions is null) return new();

            var past = sessions
                .Where(s => s.DateEnd < DateTime.UtcNow && !s.IsCancelled)
                .OrderByDescending(s => s.DateStart)
                .ToList();

            var result = new List<RaceScheduleEntry>();
            foreach (var s in past)
            {
                var name = await ResolveMeetingNameAsync(s.MeetingKey, year, "TBD");
                result.Add(new RaceScheduleEntry
                {
                    EventName = FormatEventName(s),
                    MeetingName = await ResolveMeetingNameAsync(s.MeetingKey, year, "TBD") ?? "TBD",
                    CircuitName = s.CircuitShortName ?? "TBD",
                    Country = s.CountryName ?? "",
                    EventDate = s.DateStart,
                    SessionKey = s.SessionKey,
                    IsCompleted = true,
                });
            }

            _ = Task.Run(async () =>
            {
                foreach (var entry in result)
                {
                    await Task.Delay(100);
                    var top3 = await GetAsync<List<OpenF1SessionResult>>(
                        $"https://chicane-api.duckdns.org/api/proxy/session_result?session_key={entry.SessionKey}&position<=3");
                    if (top3 is { Count: > 0 })
                    {
                        await GetDriversAsync(entry.SessionKey);
                        entry.Top3Result = string.Join(" ", top3.OrderBy(t => t.Position)
                            .Select(t => $"P{t.Position} {NameAcronym(t.DriverNumber)}"));
                    }
                }
            });

            return result;
        }
        catch (Exception ex) { Debug.WriteLine($"[OpenF1] Past: {ex.Message}"); return new(); }
    }

    public async Task<List<OpenF1Session>> GetSessionsByMeetingAsync(int meetingKey)
    {
        try
        {
            var sessions = await GetAsync<List<OpenF1Session>>(
                $"https://chicane-api.duckdns.org/api/proxy/sessions?meeting_key={meetingKey}");
            return sessions ?? new();
        }
        catch (Exception ex) { Debug.WriteLine($"[OpenF1] SessionsByMeeting: {ex.Message}"); return new(); }
    }

    public async Task<PastRaceDetail> GetPastRaceDetailAsync(int sessionKey, string categoryId)
    {
        try
        {
            var results = await GetAsync<List<OpenF1SessionResult>>(
                $"https://chicane-api.duckdns.org/api/proxy/session_result?session_key={sessionKey}");
            Debug.WriteLine($"[OpenF1] PastDetail session={sessionKey} results={results?.Count ?? 0}");
            if (results is null) return new PastRaceDetail { EventDate = DateTime.UtcNow };

            await GetDriversAsync(sessionKey);

            var driverStds = await GetAsync<List<OpenF1ChampionshipDriver>>(
                $"https://chicane-api.duckdns.org/api/proxy/championship_drivers?session_key={sessionKey}");
            var teamStds = await GetAsync<List<OpenF1ChampionshipTeam>>(
                $"https://chicane-api.duckdns.org/api/proxy/championship_teams?session_key={sessionKey}");

            var driverPoints = (driverStds ?? new())
                .ToDictionary(d => d.DriverNumber, d => (int)d.PointsCurrent);

            var sorted = results
                .OrderBy(r => r.Dnf || r.Dsq ? 1 : 0)
                .ThenBy(r => r.Dnf || r.Dsq ? (string?)DriverName(r.DriverNumber) : null)
                .ThenBy(r => r.Position)
                .Select(r =>
                {
                    var time = r.Dnf ? "DNF" : r.Dsq ? "DSQ" : r.Dns ? "DNS" :
                        r.Duration.HasValue ? FormatDuration(r.Duration.Value) : "--:--";
                    var gap = r.GapToLeader.HasValue
                        ? (r.GapToLeader.Value >= 60
                            ? $"+{(int)(r.GapToLeader.Value / 60)}:{r.GapToLeader.Value % 60:00.0}s"
                            : $"+{r.GapToLeader.Value:F1}s")
                        : null;
                    return new RaceResultEntry
                    {
                        Position = r.Position,
                        DriverName = DriverName(r.DriverNumber),
                        DriverAcronym = NameAcronym(r.DriverNumber),
                        TeamName = DriverTeam(r.DriverNumber),
                        TimeOrDnf = time,
                        Points = driverPoints.TryGetValue(r.DriverNumber, out var pts) ? pts : 0,
                        IsDnf = r.Dnf,
                        IsDns = r.Dns,
                        IsDsq = r.Dsq,
                        GapToLeader = gap,
                        NumberOfLaps = r.NumberOfLaps,
                        HeadshotUrl = DriverHeadshot(r.DriverNumber),
                    };
                })
                .ToList();

            return new PastRaceDetail
            {
                EventDate = DateTime.UtcNow,
                Results = sorted,
                DriverStandings = (driverStds ?? new())
                    .OrderBy(d => d.PositionCurrent)
                    .Select(d => new StandingsEntry
                    {
                        Position = d.PositionCurrent,
                        DriverName = DriverName(d.DriverNumber),
                        TeamName = DriverTeam(d.DriverNumber),
                        Points = (int)d.PointsCurrent,
                    }).ToList(),
                ConstructorStandings = (teamStds ?? new())
                    .OrderBy(t => t.PositionCurrent)
                    .Select(t => new ConstructorStandingsEntry
                    {
                        Position = t.PositionCurrent,
                        TeamName = t.TeamName ?? "",
                        Points = (int)t.PointsCurrent,
                    }).ToList(),
            };
        }
        catch (Exception ex) { Debug.WriteLine($"[OpenF1] PastDetail: {ex.Message}"); return new(); }
    }

    private static string TeamLogoFor(string teamName) => teamName switch
    {
        "Ferrari" => "team_ferrari_transparent.png",
        "Red Bull" or "Red Bull Racing" => "team_redbull_transparent.png",
        "Mercedes" => "team_mercedes_transparent.png",
        "McLaren" => "team_mclaren_transparent.png",
        "Aston Martin" => "team_astonmartin_transparent.png",
        "Alpine" => "team_alpine_transparent.png",
        "Racing Bulls" => "team_racingbulls_transparent.png",
        "Haas F1 Team" or "Haas" => "team_haas_transparent.png",
        "Williams" => "team_williams_transparent.png",
        "Audi" => "team_audi_transparent.png",
        "Cadillac" => "team_cadillac_transparent.png",
        _ => "",
    };

    public async Task<List<RaceWeekendCard>> GetUpcomingWeekendsAsync(int count = 3)
    {
        try
        {
            var year = DateTime.UtcNow.Year;
            var meetings = await GetMeetingsAsync(year);
            return meetings
                .Where(m => m.DateStart > DateTime.UtcNow && !string.IsNullOrWhiteSpace(m.MeetingName) && !m.MeetingName.StartsWith("Pre-Season"))
                .OrderBy(m => m.DateStart)
                .Take(count)
                .Select(m => new RaceWeekendCard
                {
                    MeetingName = m.MeetingName!.ToUpper(),
                    CircuitName = m.CircuitShortName ?? "",
                    Country = m.CountryName ?? "",
                    DateStart = m.DateStart,
                    DateEnd = m.DateEnd,
                    CircuitImageUrl = m.CircuitImage ?? "",
                    MeetingKey = m.MeetingKey,
                })
                .ToList();
        }
        catch (Exception ex) { Debug.WriteLine($"[OpenF1] UpcomingWeekends: {ex.Message}"); return new(); }
    }

    public async Task<List<MeetingScheduleCard>> GetMeetingScheduleAsync(bool isPast)
    {
        try
        {
            var year = DateTime.UtcNow.Year;
            var meetings = await GetMeetingsAsync(year);

            if (_sessionsCache is null || _sessionsCacheYear != year)
            {
                _sessionsCache = await GetAsync<List<OpenF1Session>>(
                    $"https://chicane-api.duckdns.org/api/proxy/sessions?year={year}");
                _sessionsCacheYear = year;
            }
            var sessions = _sessionsCache;

            var relevant = meetings.Where(m =>
                !string.IsNullOrWhiteSpace(m.MeetingName) &&
                !m.MeetingName.StartsWith("Pre-Season"));
            if (isPast)
                relevant = relevant.Where(m => m.DateEnd < DateTime.UtcNow).OrderByDescending(m => m.DateStart);
            else
                relevant = relevant.Where(m => m.DateStart > DateTime.UtcNow).OrderBy(m => m.DateStart);

            var result = new List<MeetingScheduleCard>();
            foreach (var meeting in relevant)
            {
                var meetingSessions = (sessions ?? new())
                    .Where(s => s.MeetingKey == meeting.MeetingKey && !s.IsCancelled)
                    .OrderBy(s => s.DateStart)
                    .ToList();

                var card = new MeetingScheduleCard
                {
                    MeetingName = meeting.MeetingName!,
                    CircuitName = meeting.CircuitShortName ?? "",
                    Country = meeting.CountryName ?? "",
                    DateStart = meeting.DateStart,
                    DateEnd = meeting.DateEnd,
                    CircuitImageUrl = meeting.CircuitImage ?? "",
                    MeetingKey = meeting.MeetingKey,
                    IsCompleted = isPast,
                    Sessions = meetingSessions.Select(s => new SessionTime
                    {
                        SessionName = s.SessionName ?? "",
                        DateTime = s.DateStart,
                        SessionKey = s.SessionKey,
                    }).ToList(),
                };

                if (isPast)
                {
                    var raceSession = meetingSessions.FirstOrDefault(s => s.SessionName == "Race");
                    if (raceSession is not null)
                    {
                        if (_driverCache is null or { Count: 0 })
                            await GetDriversAsync(raceSession.SessionKey);
                        var top3 = await GetAsync<List<OpenF1SessionResult>>(
                            $"https://chicane-api.duckdns.org/api/proxy/session_result?session_key={raceSession.SessionKey}&position<=3");
                        if (top3 is { Count: > 0 })
                        {
                            card.Top3Drivers = top3.OrderBy(t => t.Position).Select(t => new Top3Driver
                            {
                                Position = t.Position,
                                HeadshotUrl = _driverCache?.TryGetValue(t.DriverNumber, out var d) == true
                                    ? d.HeadshotUrl ?? "" : "",
                                Initials = _driverCache?.TryGetValue(t.DriverNumber, out var dr) == true
                                    ? dr.NameAcronym ?? dr.BroadcastName?[..3] ?? $"P{t.Position}" : $"P{t.Position}",
                                TimeOrDnf = t.Dnf ? "DNF" : t.Dsq ? "DSQ" :
                                    t.Duration.HasValue ? FormatDuration(t.Duration.Value) : "--:--",
                            }).ToList();
                        }
                    }
                }

                result.Add(card);
            }

            return result;
        }
        catch (Exception ex) { Debug.WriteLine($"[OpenF1] MeetingSchedule: {ex.Message}"); return new(); }
    }

    private string NameAcronym(int driverNumber)
    {
        if (_driverCache?.TryGetValue(driverNumber, out var d) == true)
            return d.NameAcronym ?? d.BroadcastName ?? $"D{driverNumber}";
        return $"D{driverNumber}";
    }

    // ── Live Data ──

    public async Task<string?> GetMeetingCircuitImageAsync(int meetingKey, int year)
    {
        try
        {
            var meetings = await GetAsync<List<OpenF1Meeting>>(
                $"https://chicane-api.duckdns.org/api/proxy/meetings?meeting_key={meetingKey}&year={year}");
            return meetings?.FirstOrDefault()?.CircuitImage;
        }
        catch (Exception ex) { Debug.WriteLine($"[OpenF1] CircuitImage: {ex.Message}"); return null; }
    }

    public async Task<List<OpenF1Lap>> GetLapsPerSessionAsync(int sessionKey)
    {
        try
        {
            var data = await GetAsync<List<OpenF1Lap>>(
                $"https://chicane-api.duckdns.org/api/proxy/laps?session_key={sessionKey}");
            return data ?? new();
        }
        catch (Exception ex) { Debug.WriteLine($"[OpenF1] Laps: {ex.Message}"); return new(); }
    }

    public async Task<OpenF1CarData?> GetCarDataAsync(int sessionKey, int driverNumber)
    {
        try
        {
            var data = await GetAsync<List<OpenF1CarData>>(
                $"https://chicane-api.duckdns.org/api/proxy/car_data?session_key={sessionKey}&driver_number={driverNumber}");
            return data?.MaxBy(d => d.Date);
        }
        catch (Exception ex) { Debug.WriteLine($"[OpenF1] CarData: {ex.Message}"); return null; }
    }

    public async Task<List<OpenF1Interval>> GetIntervalsAsync(int sessionKey)
    {
        try
        {
            var data = await GetAsync<List<OpenF1Interval>>(
                $"https://chicane-api.duckdns.org/api/proxy/intervals?session_key={sessionKey}");
            return (data ?? new())
                .GroupBy(i => i.DriverNumber)
                .Select(g => g.MaxBy(i => i.Date)!)
                .Where(i => i is not null)
                .ToList();
        }
        catch (Exception ex) { Debug.WriteLine($"[OpenF1] Intervals: {ex.Message}"); return new(); }
    }

    public async Task<List<OpenF1Location>> GetLocationsAsync(int sessionKey)
    {
        try
        {
            var data = await GetAsync<List<OpenF1Location>>(
                $"https://chicane-api.duckdns.org/api/proxy/location?session_key={sessionKey}");
            return (data ?? new())
                .GroupBy(l => l.DriverNumber)
                .Select(g => g.MaxBy(l => l.Date)!)
                .Where(l => l is not null)
                .ToList();
        }
        catch (Exception ex) { Debug.WriteLine($"[OpenF1] Locations: {ex.Message}"); return new(); }
    }

    public async Task<List<OpenF1RaceControl>> GetRaceControlAsync(int sessionKey)
    {
        try
        {
            var data = await GetAsync<List<OpenF1RaceControl>>(
                $"https://chicane-api.duckdns.org/api/proxy/race_control?session_key={sessionKey}");
            return (data ?? new()).OrderByDescending(r => r.Date).Take(30).ToList();
        }
        catch (Exception ex) { Debug.WriteLine($"[OpenF1] RaceControl: {ex.Message}"); return new(); }
    }

    public async Task<List<OpenF1Pit>> GetPitStopsAsync(int sessionKey)
    {
        try
        {
            var data = await GetAsync<List<OpenF1Pit>>(
                $"https://chicane-api.duckdns.org/api/proxy/pit?session_key={sessionKey}");
            return data ?? new();
        }
        catch (Exception ex) { Debug.WriteLine($"[OpenF1] PitStops: {ex.Message}"); return new(); }
    }

    public async Task<List<OpenF1Stint>> GetStintsAsync(int sessionKey)
    {
        try
        {
            var data = await GetAsync<List<OpenF1Stint>>(
                $"https://chicane-api.duckdns.org/api/proxy/stints?session_key={sessionKey}");
            return data ?? new();
        }
        catch (Exception ex) { Debug.WriteLine($"[OpenF1] Stints: {ex.Message}"); return new(); }
    }

    public async Task<List<OpenF1Overtake>> GetOvertakesAsync(int sessionKey)
    {
        try
        {
            var data = await GetAsync<List<OpenF1Overtake>>(
                $"https://chicane-api.duckdns.org/api/proxy/overtakes?session_key={sessionKey}");
            return (data ?? new()).OrderByDescending(o => o.Date).Take(30).ToList();
        }
        catch (Exception ex) { Debug.WriteLine($"[OpenF1] Overtakes: {ex.Message}"); return new(); }
    }

    private static string FormatDuration(double seconds)
    {
        var ts = TimeSpan.FromSeconds(seconds);
        return ts.TotalHours >= 1
            ? $"{ts.Hours}:{ts.Minutes:D2}:{ts.Seconds:D2}.{ts.Milliseconds:D3}"
            : $"{ts.Minutes:D2}:{ts.Seconds:D2}.{ts.Milliseconds:D3}";
    }

    private static RaceEvent DefaultEvent() => new()
    {
        EventName = "NEXT RACE", ScheduledDateTime = DateTime.UtcNow.AddDays(7),
        Circuit = new CircuitInfo { Name = "TBD" },
        Weather = new WeatherInfo { Condition = "N/A", TemperatureC = 20, IconGlyph = "\u2600" }
    };
}
