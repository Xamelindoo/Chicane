using Chicane.Mobile.Models;

namespace Chicane.Mobile.Services;

public class MockF1DataService : IF1DataService
{
    public Task<RaceEvent> GetNextEventAsync()
    {
        var raceEvent = new RaceEvent
        {
            EventName = "MONZA GP",
            ScheduledDateTime = DateTime.UtcNow.AddDays(3).AddHours(14).AddMinutes(0),
            Circuit = new CircuitInfo
            {
                Name = "Autodromo Nazionale di Monza",
                Country = "Italy",
                ImageUrl = "circuit_placeholder.png"
            },
            Weather = new WeatherInfo
            {
                Condition = "Rain Expected",
                TemperatureC = 18,
                IconGlyph = "\u26C8"
            }
        };

        return Task.FromResult(raceEvent);
    }

    public Task<LiveSessionData> GetLiveSessionDataAsync()
    {
        var liveData = new LiveSessionData
        {
            SessionName = "RACE",
            Status = "LIVE",
            CurrentLap = 32,
            TotalLaps = 53,
            PositionBoard = new List<DriverGap>
            {
                new() { Position = 1, DriverName = "M. VERSTAPPEN", TeamName = "Red Bull Racing", GapDisplay = "Leader" },
                new() { Position = 2, DriverName = "L. HAMILTON", TeamName = "Mercedes", GapDisplay = "+0.214" },
                new() { Position = 3, DriverName = "C. LECLERC", TeamName = "Ferrari", GapDisplay = "+0.892" }
            }
        };

        return Task.FromResult(liveData);
    }

    public Task<List<StandingsEntry>> GetStandingsAsync()
    {
        var standings = new List<StandingsEntry>
        {
            new() { Position = 1,  DriverName = "M. VERSTAPPEN",  TeamName = "Red Bull Racing", Points = 278 },
            new() { Position = 2,  DriverName = "L. HAMILTON",    TeamName = "Mercedes",        Points = 235 },
            new() { Position = 3,  DriverName = "C. LECLERC",     TeamName = "Ferrari",         Points = 218 },
            new() { Position = 4,  DriverName = "O. PIASTRI",     TeamName = "McLaren",         Points = 196 },
            new() { Position = 5,  DriverName = "L. NORRIS",      TeamName = "McLaren",         Points = 182 },
            new() { Position = 6,  DriverName = "S. PEREZ",       TeamName = "Red Bull Racing", Points = 165 },
            new() { Position = 7,  DriverName = "G. RUSSELL",     TeamName = "Mercedes",        Points = 148 },
            new() { Position = 8,  DriverName = "F. ALONSO",      TeamName = "Aston Martin",    Points = 132 },
            new() { Position = 9,  DriverName = "C. SAINZ",       TeamName = "Ferrari",         Points = 125 },
            new() { Position = 10, DriverName = "L. STROLL",      TeamName = "Aston Martin",    Points = 84 },
            new() { Position = 11, DriverName = "E. OCON",        TeamName = "Alpine",          Points = 58 },
            new() { Position = 12, DriverName = "P. GASLY",       TeamName = "Alpine",          Points = 52 },
            new() { Position = 13, DriverName = "Y. TSUNODA",     TeamName = "RB",              Points = 38 },
            new() { Position = 14, DriverName = "N. HULKENBERG",  TeamName = "Haas",            Points = 32 },
            new() { Position = 15, DriverName = "D. RICCIARDO",   TeamName = "RB",              Points = 28 },
            new() { Position = 16, DriverName = "K. MAGNUSSEN",   TeamName = "Haas",            Points = 24 },
            new() { Position = 17, DriverName = "A. ALBON",       TeamName = "Williams",        Points = 18 },
            new() { Position = 18, DriverName = "V. BOTTAS",      TeamName = "Sauber",          Points = 14 },
            new() { Position = 19, DriverName = "Z. GUANYU",      TeamName = "Sauber",          Points = 8 },
            new() { Position = 20, DriverName = "L. SARGEANT",    TeamName = "Williams",        Points = 4 }
        };

        return Task.FromResult(standings);
    }

    public Task<List<TrackPosition>> GetTrackPositionsAsync()
    {
        var positions = new List<TrackPosition>
        {
            new() { Position = 1, DriverName = "M. VERSTAPPEN", Sector = 3, LastTime = "1:24.332" },
            new() { Position = 2, DriverName = "L. HAMILTON",   Sector = 3, LastTime = "1:24.546" },
            new() { Position = 3, DriverName = "C. LECLERC",    Sector = 2, LastTime = "1:24.810" }
        };

        return Task.FromResult(positions);
    }

    public Task<List<ConstructorStandingsEntry>> GetConstructorStandingsAsync()
    {
        var standings = new List<ConstructorStandingsEntry>
        {
            new() { Position = 1,  TeamName = "Red Bull Racing",  Points = 443 },
            new() { Position = 2,  TeamName = "Mercedes",         Points = 383 },
            new() { Position = 3,  TeamName = "Ferrari",          Points = 343 },
            new() { Position = 4,  TeamName = "McLaren",          Points = 378 },
            new() { Position = 5,  TeamName = "Aston Martin",     Points = 216 },
            new() { Position = 6,  TeamName = "Alpine",           Points = 110 },
            new() { Position = 7,  TeamName = "RB",               Points = 66  },
            new() { Position = 8,  TeamName = "Haas",             Points = 56  },
            new() { Position = 9,  TeamName = "Williams",         Points = 22  },
            new() { Position = 10, TeamName = "Sauber",           Points = 22  }
        };

        return Task.FromResult(standings);
    }
}
