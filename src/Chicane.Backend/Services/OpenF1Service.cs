using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;
using Chicane.Backend.Models.Api;

namespace Chicane.Backend.Services;

public class OpenF1Service
{
    private readonly HttpClient _http;
    private readonly string? _username;
    private readonly string? _password;
    private Dictionary<int, OpenF1Driver>? _driverCache;
    private int? _driverCacheSessionKey;
    private List<OpenF1Meeting>? _meetingsCache;
    private int? _meetingsCacheYear;
    private List<OpenF1Session>? _sessionsCache;
    private int? _sessionsCacheYear;

    public OpenF1Service(HttpClient http, IConfiguration config)
    {
        _http = http;
        _username = config["OpenF1:Username"];
        _password = config["OpenF1:Password"];
        _http.BaseAddress = new Uri("https://api.openf1.org/v1/");
        _http.DefaultRequestHeaders.Add("User-Agent", "Chicane-Backend/1.0");
    }

    private async Task AuthenticateAsync()
    {
        if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password)) return;
        if (_http.DefaultRequestHeaders.Authorization is not null) return;
        try
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", _username),
                new KeyValuePair<string, string>("password", _password),
            });
            var response = await _http.PostAsync("https://api.openf1.org/token", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            var token = doc.RootElement.GetProperty("access_token").GetString()!;
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            Debug.WriteLine("[OpenF1] Authenticated");
        }
        catch (Exception ex) { Debug.WriteLine($"[OpenF1] Auth failed: {ex.Message}"); }
    }

    private async Task<T?> GetAsync<T>(string path) where T : class
    {
        await AuthenticateAsync();
        try
        {
            var json = await _http.GetStringAsync(path);
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch { return null; }
    }

    // ─── Drivers cache ───

    public async Task LoadDriverCacheAsync(int sessionKey)
    {
        if (_driverCache is not null && _driverCacheSessionKey == sessionKey) return;
        var drivers = await GetAsync<List<OpenF1Driver>>($"drivers?session_key={sessionKey}");
        _driverCache = (drivers ?? new()).ToDictionary(d => d.DriverNumber);
        _driverCacheSessionKey = sessionKey;
    }

    public string DriverName(int driverNumber) =>
        _driverCache?.TryGetValue(driverNumber, out var d) == true
            ? d.BroadcastName ?? d.NameAcronym ?? $"#{driverNumber}" : $"#{driverNumber}";

    public string DriverFullName(int driverNumber) =>
        _driverCache?.TryGetValue(driverNumber, out var d) == true
            ? d.FullName ?? d.BroadcastName ?? $"#{driverNumber}" : $"#{driverNumber}";

    public string DriverCode(int driverNumber) =>
        _driverCache?.TryGetValue(driverNumber, out var d) == true
            ? d.NameAcronym ?? $"D{driverNumber}" : $"D{driverNumber}";

    public string DriverTeam(int driverNumber) =>
        _driverCache?.TryGetValue(driverNumber, out var d) == true
            ? d.TeamName ?? "" : "";

    public string DriverTeamColour(int driverNumber) =>
        _driverCache?.TryGetValue(driverNumber, out var d) == true
            ? d.TeamColour ?? "333333" : "333333";

    public string DriverHeadshot(int driverNumber) =>
        _driverCache?.TryGetValue(driverNumber, out var d) == true
            ? d.HeadshotUrl ?? "" : "";

    public List<object> GetCachedDrivers()
    {
        if (_driverCache is null) return new();
        return _driverCache.Values.Select(d => (object)new
        {
            driverNumber = d.DriverNumber,
            driverCode = d.NameAcronym ?? "",
            driverName = d.BroadcastName ?? d.NameAcronym ?? $"#{d.DriverNumber}",
            teamName = d.TeamName ?? "",
            teamColour = d.TeamColour ?? "333333",
            headshotUrl = d.HeadshotUrl ?? "",
            lastName = (d.LastName ?? d.FullName ?? "").ToUpperInvariant(),
        }).ToList();
    }

    // ─── Meetings ───

    public async Task<List<OpenF1Meeting>> GetMeetingsAsync(int year)
    {
        if (_meetingsCache is not null && _meetingsCacheYear == year) return _meetingsCache;
        _meetingsCache = await GetAsync<List<OpenF1Meeting>>($"meetings?year={year}");
        if (_meetingsCache is not null) _meetingsCacheYear = year;
        return _meetingsCache ?? new();
    }

    private async Task<string?> ResolveMeetingNameAsync(int meetingKey, int year)
    {
        var meetings = await GetMeetingsAsync(year);
        return meetings.FirstOrDefault(m => m.MeetingKey == meetingKey)?.MeetingName;
    }

    // ─── Sessions ───

    public async Task<List<OpenF1Session>> GetSessionsAsync(int year)
    {
        if (_sessionsCache is not null && _sessionsCacheYear == year) return _sessionsCache;
        _sessionsCache = await GetAsync<List<OpenF1Session>>($"sessions?year={year}");
        _sessionsCacheYear = year;
        return _sessionsCache ?? new();
    }

    // ─── Positions ───

    public async Task<List<OpenF1Position>> GetPositionsAsync(int sessionKey)
    {
        var data = await GetAsync<List<OpenF1Position>>($"position?session_key={sessionKey}");
        return data?.OrderBy(p => p.Position).ToList() ?? new();
    }

    // ─── Intervals ───

    public async Task<List<OpenF1Interval>> GetIntervalsAsync(int sessionKey)
    {
        var data = await GetAsync<List<OpenF1Interval>>($"intervals?session_key={sessionKey}");
        return (data ?? new()).GroupBy(i => i.DriverNumber).Select(g => g.MaxBy(i => i.Date)!).ToList();
    }

    // ─── Laps ───

    public async Task<List<OpenF1Lap>> GetLapsAsync(int sessionKey)
    {
        return await GetAsync<List<OpenF1Lap>>($"laps?session_key={sessionKey}") ?? new();
    }

    // ─── Car Data ───

    public async Task<OpenF1CarData?> GetCarDataAsync(int sessionKey, int driverNumber)
    {
        var data = await GetAsync<List<OpenF1CarData>>($"car_data?session_key={sessionKey}&driver_number={driverNumber}");
        return data?.MaxBy(d => d.Date);
    }

    // ─── Locations ───

    public async Task<List<OpenF1Location>> GetLocationsAsync(int sessionKey)
    {
        var data = await GetAsync<List<OpenF1Location>>($"location?session_key={sessionKey}");
        return (data ?? new()).GroupBy(l => l.DriverNumber).Select(g => g.MaxBy(l => l.Date)!).ToList();
    }

    // ─── Race Control ───

    public async Task<List<OpenF1RaceControl>> GetRaceControlAsync(int sessionKey)
    {
        var data = await GetAsync<List<OpenF1RaceControl>>($"race_control?session_key={sessionKey}");
        return (data ?? new()).OrderByDescending(r => r.Date).Take(30).ToList();
    }

    // ─── Pit / Stints ───

    public async Task<List<OpenF1Pit>> GetPitsAsync(int sessionKey)
    {
        return await GetAsync<List<OpenF1Pit>>($"pit?session_key={sessionKey}") ?? new();
    }

    public async Task<List<OpenF1Stint>> GetStintsAsync(int sessionKey)
    {
        return await GetAsync<List<OpenF1Stint>>($"stints?session_key={sessionKey}") ?? new();
    }

    // ─── Session Results / Past Races ───

    public async Task<List<OpenF1SessionResult>> GetSessionResultsAsync(int sessionKey)
    {
        return await GetAsync<List<OpenF1SessionResult>>($"session_result?session_key={sessionKey}") ?? new();
    }

    public async Task<List<OpenF1Session>> GetPastRacesAsync(int year)
    {
        var sessions = await GetAsync<List<OpenF1Session>>($"sessions?year={year}&session_type=Race");
        return (sessions ?? new())
            .Where(s => s.DateEnd < DateTime.UtcNow && !s.IsCancelled)
            .OrderByDescending(s => s.DateStart)
            .ToList();
    }

    // ─── Standings ───

    public async Task<List<OpenF1ChampionshipDriver>> GetDriverStandingsAsync(int year)
    {
        var sessions = await GetPastRacesAsync(year);
        var latest = sessions.FirstOrDefault();
        if (latest is null) return new();
        return await GetAsync<List<OpenF1ChampionshipDriver>>($"championship_drivers?session_key={latest.SessionKey}") ?? new();
    }

    public async Task<List<OpenF1ChampionshipTeam>> GetConstructorStandingsAsync(int year)
    {
        var sessions = await GetPastRacesAsync(year);
        var latest = sessions.FirstOrDefault();
        if (latest is null) return new();
        return await GetAsync<List<OpenF1ChampionshipTeam>>($"championship_teams?session_key={latest.SessionKey}") ?? new();
    }

    public async Task<Dictionary<string, string>> GetTeamColorsAsync()
    {
        var year = DateTime.UtcNow.Year;
        var sessions = await GetPastRacesAsync(year);
        var latest = sessions.FirstOrDefault();
        if (latest is null) return new();
        await LoadDriverCacheAsync(latest.SessionKey);
        return _driverCache?
            .Where(kv => kv.Value.TeamName is not null && kv.Value.TeamColour is not null)
            .GroupBy(kv => kv.Value.TeamName!)
            .ToDictionary(g => g.Key, g => $"#{g.First().Value.TeamColour}") ?? new();
    }

    // ─── Meetings / Next Event ───

    public async Task<Dictionary<string, object>> GetNextEventAsync()
    {
        var year = DateTime.UtcNow.Year;
        var sessions = await GetSessionsAsync(year);
        var next = sessions?
            .Where(s => s.DateStart > DateTime.UtcNow && !s.IsCancelled)
            .OrderBy(s => s.DateStart)
            .FirstOrDefault();

        if (next is null) return new() { ["eventName"] = "NEXT RACE" };

        return new()
        {
            ["sessionKey"] = next.SessionKey,
            ["sessionName"] = next.SessionName ?? "",
            ["meetingKey"] = next.MeetingKey,
            ["meetingName"] = await ResolveMeetingNameAsync(next.MeetingKey, year) ?? next.Location ?? "",
            ["circuitName"] = next.CircuitShortName ?? "",
            ["countryName"] = next.CountryName ?? "",
            ["location"] = next.Location ?? "",
            ["dateStart"] = next.DateStart,
            ["dateEnd"] = next.DateEnd,
        };
    }

    // ─── Meeting circuit image ───

    public async Task<string?> GetMeetingCircuitImageAsync(int meetingKey, int year)
    {
        var meetings = await GetMeetingsAsync(year);
        return meetings.FirstOrDefault(m => m.MeetingKey == meetingKey)?.CircuitImage;
    }

    public async Task<string?> GetRawStringAsync(string path)
    {
        await AuthenticateAsync();
        try
        {
            return await _http.GetStringAsync(path);
        }
        catch { return null; }
    }
}
