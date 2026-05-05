using System.Net.Http.Json;
using System.Text.Json;
using Chicane.Mobile.Models;
using Chicane.Mobile.Models.Api;

namespace Chicane.Mobile.Services;

public class ErgastApiService : IF1DataService
{
    private readonly HttpClient _http;
    private const string BaseUrl = "https://ergast.com/api/f1/2025";

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
                $"{BaseUrl}.json",
                JsonOptions);
            if (response?.MrData.RaceTable?.Races is null) return DefaultRaceEvent();

            var now = DateTime.UtcNow;
            var nextRace = response.MrData.RaceTable.Races
                .Select(r => (Race: r, Date: ParseDate(r.Date)))
                .FirstOrDefault(x => x.Date > now);

            if (nextRace.Race is not null)
            {
                return new RaceEvent
                {
                    EventName = nextRace.Race.RaceName.ToUpper().Replace(" GRAND PRIX", " GP"),
                    ScheduledDateTime = nextRace.Date,
                    Circuit = new CircuitInfo
                    {
                        Name = nextRace.Race.Circuit.CircuitName,
                        Country = nextRace.Race.Circuit.Location.Country,
                    },
                    Weather = new WeatherInfo { Condition = "N/A", TemperatureC = 20, IconGlyph = "\u2600" }
                };
            }

            var lastRace = response.MrData.RaceTable.Races.Last();
            return new RaceEvent
            {
                EventName = $"{lastRace.RaceName.ToUpper().Replace(" GRAND PRIX", " GP")} (SEASON COMPLETE)",
                ScheduledDateTime = ParseDate(lastRace.Date).AddDays(365),
                Circuit = new CircuitInfo
                {
                    Name = lastRace.Circuit.CircuitName,
                    Country = lastRace.Circuit.Location.Country,
                },
                Weather = new WeatherInfo { Condition = "N/A", TemperatureC = 20, IconGlyph = "\u2600" }
            };
        }
        catch
        {
            return DefaultRaceEvent();
        }
    }

    private static DateTime ParseDate(string date) =>
        DateTime.TryParse(date, out var result) ? result : DateTime.UtcNow;

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
                $"{BaseUrl}/driverStandings.json",
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
                $"{BaseUrl}/constructorStandings.json",
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
