using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;
using Chicane.Mobile.Models;
using Chicane.Mobile.Models.Api;

namespace Chicane.Mobile.Services;

public class OpenF1ApiService : IF1DataService, IRaceCalendarService
{
    private readonly HttpClient _http;
    private readonly OpenF1TokenService _token;
    private List<OpenF1Meeting>? _meetingsCache;
    private List<OpenF1Session>? _sessionsCache;
    private int? _sessionsCacheYear;
    private Dictionary<int, OpenF1Driver>? _driverCache;
    private int? _driverCacheSessionKey;
    private bool? _tokenWorks;
    private int _delayMs = 350;

    public OpenF1ApiService(HttpClient http, OpenF1TokenService token)
    {
        _http = http;
        _token = token;
    }

    private async Task<HttpRequestMessage> CreateRequestAsync(string url)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Accept.ParseAdd("application/json");

        if (_tokenWorks ?? true)
        {
            try
            {
                var accessToken = await _token.GetTokenAsync();
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[OpenF1] Token error: {ex.Message}");
            }
        }

        return request;
    }

    private async Task<T?> GetAsync<T>(string url) where T : class
    {
        var request = await CreateRequestAsync(url);
        var response = await _http.SendAsync(request);
        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Debug.WriteLine($"[OpenF1] {url} → {(int)response.StatusCode} {response.StatusCode}");
            if (json.Length > 0)
                Debug.WriteLine($"[OpenF1] Body: {json[..Math.Min(json.Length, 500)]}");

            if (_tokenWorks is null && response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _tokenWorks = false;
                _delayMs = 350;
                Debug.WriteLine("[OpenF1] Token test failed — fallback free tier (350ms delay)");
                return await GetWithoutTokenAsync<T>(url);
            }

            return null;
        }

        if (_tokenWorks is null)
        {
            _tokenWorks = true;
            _delayMs = 180;
            Debug.WriteLine("[OpenF1] Token test OK — paid tier (180ms delay)");
        }

        var preview = json.Length <= 300 ? json : json[..300] + "...";
        Debug.WriteLine($"[OpenF1-OK] {url[..Math.Min(url.Length, 100)]} → {json.Length} bytes: {preview}");

        return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    private async Task<T?> GetWithoutTokenAsync<T>(string url) where T : class
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Accept.ParseAdd("application/json");
        var response = await _http.SendAsync(request);
        var json = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            Debug.WriteLine($"[OpenF1] (no-token) {url} → {(int)response.StatusCode} {response.StatusCode}");
            if (json.Length > 0)
                Debug.WriteLine($"[OpenF1] Body: {json[..Math.Min(json.Length, 500)]}");
            return null;
        }
        return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    private async Task<Dictionary<int, OpenF1Driver>> GetDriversAsync(int sessionKey)
    {
        if (_driverCache is not null && _driverCacheSessionKey == sessionKey)
            return _driverCache;

        var drivers = await GetAsync<List<OpenF1Driver>>(
            $"https://api.openf1.org/v1/drivers?session_key={sessionKey}");
        _driverCache = (drivers ?? new()).ToDictionary(d => d.DriverNumber);
        _driverCacheSessionKey = sessionKey;
        return _driverCache;
    }

    private string DriverName(int driverNumber)
    {
        if (_driverCache?.TryGetValue(driverNumber, out var d) == true)
            return d.BroadcastName ?? d.NameAcronym ?? $"#{driverNumber}";
        return $"#{driverNumber}";
    }

    private string DriverTeam(int driverNumber)
    {
        if (_driverCache?.TryGetValue(driverNumber, out var d) == true)
            return d.TeamName ?? "";
        return "";
    }

    private string DriverHeadshot(int driverNumber)
    {
        if (_driverCache?.TryGetValue(driverNumber, out var d) == true)
            return d.HeadshotUrl ?? "";
        return "";
    }

    public async Task<Dictionary<string, string>> GetTeamColorsAsync()
    {
        if (_driverCache is null or { Count: 0 })
        {
            var year = DateTime.UtcNow.Year;
            var sessions = await GetAsync<List<OpenF1Session>>(
                $"https://api.openf1.org/v1/sessions?year={year}&session_type=Race");
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
                $"https://api.openf1.org/v1/championship_drivers?driver_number={driverNumber}");
            var currentEntry = standings?.OrderByDescending(s => s.SessionKey).FirstOrDefault();

            var results = await GetAsync<List<OpenF1SessionResult>>(
                $"https://api.openf1.org/v1/session_result?driver_number={driverNumber}");

            // Build meeting lookup dict for O(1) meeting name resolution
            var meetings = await GetMeetingsAsync(year);
            var meetingNameByKey = (meetings ?? new()).ToDictionary(m => m.MeetingKey, m => m.MeetingName);

            var raceResults = new List<DriverRaceResult>();
            if (results is not null)
            {
                foreach (var r in results)
                {
                    sessionByKey.TryGetValue(r.SessionKey, out var session);
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
        if (_meetingsCache is not null)
            return _meetingsCache;

        _meetingsCache = await GetAsync<List<OpenF1Meeting>>(
            $"https://api.openf1.org/v1/meetings?year={year}");
        return _meetingsCache ?? new();
    }

    private async Task<List<OpenF1Session>> GetSessionsAsync(int year)
    {
        if (_sessionsCache is not null && _sessionsCacheYear == year)
            return _sessionsCache;

        _sessionsCache = await GetAsync<List<OpenF1Session>>(
            $"https://api.openf1.org/v1/sessions?year={year}");
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
            var sessions = await GetAsync<List<OpenF1Session>>(
                $"https://api.openf1.org/v1/sessions?year={year}");
            if (sessions is null || sessions.Count == 0)
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
                $"https://api.openf1.org/v1/sessions?year={year}&session_type=Race");
            var live = sessions?.FirstOrDefault(s => s.DateStart <= DateTime.UtcNow && s.DateEnd >= DateTime.UtcNow);
            if (live is null) return new LiveSessionData();

            await GetDriversAsync(live.SessionKey);

            var positions = await GetAsync<List<OpenF1Position>>(
                $"https://api.openf1.org/v1/position?session_key={live.SessionKey}");
            var top3 = positions?.Take(3).Select(p => new DriverGap
            {
                Position = p.Position,
                DriverName = DriverName(p.DriverNumber),
                TeamName = DriverTeam(p.DriverNumber),
                GapDisplay = p.Position == 1 ? "Leader" : $"+{p.Position * 0.2:F3}",
            }).ToList() ?? new();

            return new LiveSessionData
            {
                SessionName = live.SessionName ?? "RACE",
                Status = "LIVE",
                CurrentLap = 0,
                TotalLaps = 52,
                PositionBoard = top3,
            };
        }
        catch (Exception ex) { Debug.WriteLine($"[OpenF1] LiveSession: {ex.Message}"); return new LiveSessionData(); }
    }

    public async Task<List<StandingsEntry>> GetStandingsAsync(string categoryId)
    {
        try
        {
            var year = DateTime.UtcNow.Year;
            var sessions = await GetAsync<List<OpenF1Session>>(
                $"https://api.openf1.org/v1/sessions?year={year}&session_type=Race");
            if (sessions is null) return new();

            var latestRace = sessions
                .Where(s => s.DateEnd < DateTime.UtcNow && !s.IsCancelled && s.SessionName == "Race")
                .OrderByDescending(s => s.DateStart)
                .FirstOrDefault();

            if (latestRace is null) return new();

            var standings = await GetAsync<List<OpenF1ChampionshipDriver>>(
                $"https://api.openf1.org/v1/championship_drivers?session_key={latestRace.SessionKey}");
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
                $"https://api.openf1.org/v1/sessions?year={year}&session_type=Race");
            if (sessions is null) return new();

            var latestRace = sessions
                .Where(s => s.DateEnd < DateTime.UtcNow && !s.IsCancelled && s.SessionName == "Race")
                .OrderByDescending(s => s.DateStart)
                .FirstOrDefault();

            if (latestRace is null) return new();

            var teams = await GetAsync<List<OpenF1ChampionshipTeam>>(
                $"https://api.openf1.org/v1/championship_teams?session_key={latestRace.SessionKey}");
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
                $"https://api.openf1.org/v1/sessions?year={year}");
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
                $"https://api.openf1.org/v1/sessions?year={year}");
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
                    await Task.Delay(_delayMs);
                    var top3 = await GetAsync<List<OpenF1SessionResult>>(
                        $"https://api.openf1.org/v1/session_result?session_key={entry.SessionKey}&position<=3");
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

    public async Task<PastRaceDetail> GetPastRaceDetailAsync(int sessionKey, string categoryId)
    {
        try
        {
            var results = await GetAsync<List<OpenF1SessionResult>>(
                $"https://api.openf1.org/v1/session_result?session_key={sessionKey}");
            Debug.WriteLine($"[OpenF1] PastDetail session={sessionKey} results={results?.Count ?? 0}");
            if (results is null) return new PastRaceDetail { EventDate = DateTime.UtcNow };

            await GetDriversAsync(sessionKey);

            var driverStds = await GetAsync<List<OpenF1ChampionshipDriver>>(
                $"https://api.openf1.org/v1/championship_drivers?session_key={sessionKey}");
            var teamStds = await GetAsync<List<OpenF1ChampionshipTeam>>(
                $"https://api.openf1.org/v1/championship_teams?session_key={sessionKey}");

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
                        TeamName = DriverTeam(r.DriverNumber),
                        TimeOrDnf = time,
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
                    $"https://api.openf1.org/v1/sessions?year={year}");
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
                            $"https://api.openf1.org/v1/session_result?session_key={raceSession.SessionKey}&position<=3");
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
