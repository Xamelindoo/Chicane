using System.Net.Http.Json;
using System.Text.Json;
using Chicane.Mobile.Models;
using Chicane.Mobile.Models.Api;

namespace Chicane.Mobile.Services;

public class ErgastApiService : IF1DataService
{
    private readonly HttpClient _http;
    private const string BaseUrl = "https://ergast.com/api/f1";

    public ErgastApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<RaceEvent> GetNextEventAsync(string categoryId)
    {
        if (categoryId != "f1") return DefaultRaceEvent();

        try
        {
            var response = await _http.GetFromJsonAsync<ErgastResponse>(
                $"{BaseUrl}/current/next.json",
                JsonOptions);
            return response is not null ? ErgastMapper.ToRaceEvent(response) : DefaultRaceEvent();
        }
        catch
        {
            return DefaultRaceEvent();
        }
    }

    public async Task<LiveSessionData> GetLiveSessionDataAsync(string categoryId)
    {
        return new LiveSessionData(); // Ergast non fornisce dati live
    }

    public async Task<List<StandingsEntry>> GetStandingsAsync(string categoryId)
    {
        if (categoryId != "f1") return new();

        try
        {
            var response = await _http.GetFromJsonAsync<ErgastResponse>(
                $"{BaseUrl}/current/driverStandings.json",
                JsonOptions);
            return response is not null ? ErgastMapper.ToDriverStandings(response) : new();
        }
        catch
        {
            return new();
        }
    }

    public async Task<List<ConstructorStandingsEntry>> GetConstructorStandingsAsync(string categoryId)
    {
        if (categoryId != "f1") return new();

        try
        {
            var response = await _http.GetFromJsonAsync<ErgastResponse>(
                $"{BaseUrl}/current/constructorStandings.json",
                JsonOptions);
            return response is not null ? ErgastMapper.ToConstructorStandings(response) : new();
        }
        catch
        {
            return new();
        }
    }

    public async Task<List<TrackPosition>> GetTrackPositionsAsync(string categoryId)
    {
        return new(); // Ergast non fornisce track positions live
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static RaceEvent DefaultRaceEvent() => new()
    {
        EventName = "NEXT RACE",
        ScheduledDateTime = DateTime.UtcNow.AddDays(7),
        Circuit = new CircuitInfo { Name = "TBD" },
        Weather = new WeatherInfo { Condition = "N/A", TemperatureC = 20, IconGlyph = "\u2600" }
    };
}
