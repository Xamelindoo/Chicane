using Chicane.Mobile.Models;

namespace Chicane.Mobile.Services;

public class MockF1DataService : IF1DataService
{
    public Task<RaceEvent> GetNextEventAsync(string categoryId)
    {
        return Task.FromResult(categoryId switch
        {
            "f1" => new RaceEvent
            {
                EventName = "MONZA GP",
                ScheduledDateTime = DateTime.UtcNow.AddDays(3).AddHours(14),
                Circuit = new CircuitInfo { Name = "Autodromo Nazionale di Monza", Country = "Italy", ImageUrl = "circuit_placeholder.png" },
                Weather = new WeatherInfo { Condition = "Rain Expected", TemperatureC = 18, IconGlyph = "\u26C8" }
            },
            "wec" => new RaceEvent
            {
                EventName = "24H LE MANS",
                ScheduledDateTime = DateTime.UtcNow.AddDays(12).AddHours(15),
                Circuit = new CircuitInfo { Name = "Circuit de la Sarthe", Country = "France", ImageUrl = "circuit_placeholder.png" },
                Weather = new WeatherInfo { Condition = "Clear", TemperatureC = 22, IconGlyph = "\u2600" }
            },
            "wrc" => new RaceEvent
            {
                EventName = "RALLY FINLAND",
                ScheduledDateTime = DateTime.UtcNow.AddDays(21).AddHours(8),
                Circuit = new CircuitInfo { Name = "Jyvaskyla", Country = "Finland", ImageUrl = "circuit_placeholder.png" },
                Weather = new WeatherInfo { Condition = "Cloudy", TemperatureC = 12, IconGlyph = "\u2601" }
            },
            "motogp" => new RaceEvent
            {
                EventName = "MUGELLO GP",
                ScheduledDateTime = DateTime.UtcNow.AddDays(9).AddHours(13),
                Circuit = new CircuitInfo { Name = "Autodromo del Mugello", Country = "Italy", ImageUrl = "circuit_placeholder.png" },
                Weather = new WeatherInfo { Condition = "Sunny", TemperatureC = 28, IconGlyph = "\u2600" }
            },
            _ => new RaceEvent
            {
                EventName = "NEXT RACE",
                ScheduledDateTime = DateTime.UtcNow.AddDays(7),
                Circuit = new CircuitInfo { Name = "TBD", Country = "", ImageUrl = "circuit_placeholder.png" },
                Weather = new WeatherInfo { Condition = "N/A", TemperatureC = 20, IconGlyph = "\u2600" }
            }
        });
    }

    public Task<LiveSessionData> GetLiveSessionDataAsync(string categoryId)
    {
        var session = categoryId switch
        {
            "f1" => new LiveSessionData
            {
                SessionName = "RACE", Status = "LIVE", CurrentLap = 32, TotalLaps = 53,
                PositionBoard = new List<DriverGap>
                {
                    new() { Position = 1, DriverName = "M. VERSTAPPEN", TeamName = "Red Bull Racing", GapDisplay = "Leader" },
                    new() { Position = 2, DriverName = "L. HAMILTON", TeamName = "Mercedes", GapDisplay = "+0.214" },
                    new() { Position = 3, DriverName = "C. LECLERC", TeamName = "Ferrari", GapDisplay = "+0.892" }
                }
            },
            "wec" => new LiveSessionData
            {
                SessionName = "HOUR 8", Status = "LIVE", CurrentLap = 112, TotalLaps = 380,
                PositionBoard = new List<DriverGap>
                {
                    new() { Position = 1, DriverName = "TOYOTA #7", TeamName = "Toyota Gazoo Racing", GapDisplay = "Leader" },
                    new() { Position = 2, DriverName = "FERRARI #50", TeamName = "Ferrari AF Corse", GapDisplay = "+12.340" },
                    new() { Position = 3, DriverName = "PORSCHE #6", TeamName = "Porsche Penske", GapDisplay = "+45.112" }
                }
            },
            "wrc" => new LiveSessionData
            {
                SessionName = "SS12", Status = "LIVE", CurrentLap = 12, TotalLaps = 22,
                PositionBoard = new List<DriverGap>
                {
                    new() { Position = 1, DriverName = "K. ROVANPERA", TeamName = "Toyota GR Yaris", GapDisplay = "Leader" },
                    new() { Position = 2, DriverName = "T. NEUVILLE", TeamName = "Hyundai i20 N", GapDisplay = "+3.2" },
                    new() { Position = 3, DriverName = "O. TANAK", TeamName = "M-Sport Ford", GapDisplay = "+8.7" }
                }
            },
            "motogp" => new LiveSessionData
            {
                SessionName = "RACE", Status = "LIVE", CurrentLap = 14, TotalLaps = 23,
                PositionBoard = new List<DriverGap>
                {
                    new() { Position = 1, DriverName = "F. BAGNAIA", TeamName = "Ducati Lenovo", GapDisplay = "Leader" },
                    new() { Position = 2, DriverName = "M. MARQUEZ", TeamName = "Gresini Ducati", GapDisplay = "+0.342" },
                    new() { Position = 3, DriverName = "J. MARTIN", TeamName = "Prima Pramac", GapDisplay = "+0.891" }
                }
            },
            _ => new LiveSessionData()
        };

        return Task.FromResult(session);
    }

    public Task<List<StandingsEntry>> GetStandingsAsync(string categoryId)
    {
        var standings = categoryId switch
        {
            "f1" => F1Standings(),
            "wec" => WecStandings(),
            "wrc" => WrcStandings(),
            "motogp" => MotoGpStandings(),
            _ => new List<StandingsEntry>()
        };
        return Task.FromResult(standings);
    }

    public Task<List<ConstructorStandingsEntry>> GetConstructorStandingsAsync(string categoryId)
    {
        var standings = categoryId switch
        {
            "f1" => F1ConstructorStandings(),
            "wec" => WecConstructorStandings(),
            "wrc" => WrcConstructorStandings(),
            "motogp" => MotoGpConstructorStandings(),
            _ => new List<ConstructorStandingsEntry>()
        };
        return Task.FromResult(standings);
    }

    public Task<List<TrackPosition>> GetTrackPositionsAsync(string categoryId)
    {
        var positions = categoryId switch
        {
            "f1" => F1Track(),
            "wec" => WecTrack(),
            "wrc" => WrcTrack(),
            "motogp" => MotoGpTrack(),
            _ => new List<TrackPosition>()
        };
        return Task.FromResult(positions);
    }

    private static List<StandingsEntry> F1Standings() => new()
    {
        new() { Position = 1, DriverName = "M. VERSTAPPEN", TeamName = "Red Bull Racing", Points = 278 },
        new() { Position = 2, DriverName = "L. HAMILTON", TeamName = "Mercedes", Points = 235 },
        new() { Position = 3, DriverName = "C. LECLERC", TeamName = "Ferrari", Points = 218 },
    };

    private static List<StandingsEntry> WecStandings() => new()
    {
        new() { Position = 1, DriverName = "TOYOTA #7", TeamName = "Toyota Gazoo Racing", Points = 145 },
        new() { Position = 2, DriverName = "FERRARI #50", TeamName = "Ferrari AF Corse", Points = 132 },
        new() { Position = 3, DriverName = "PORSCHE #6", TeamName = "Porsche Penske", Points = 118 },
    };

    private static List<StandingsEntry> WrcStandings() => new()
    {
        new() { Position = 1, DriverName = "K. ROVANPERA", TeamName = "Toyota GR Yaris", Points = 210 },
        new() { Position = 2, DriverName = "T. NEUVILLE", TeamName = "Hyundai i20 N", Points = 182 },
        new() { Position = 3, DriverName = "O. TANAK", TeamName = "M-Sport Ford", Points = 165 },
    };

    private static List<StandingsEntry> MotoGpStandings() => new()
    {
        new() { Position = 1, DriverName = "F. BAGNAIA", TeamName = "Ducati Lenovo", Points = 285 },
        new() { Position = 2, DriverName = "J. MARTIN", TeamName = "Prima Pramac", Points = 262 },
        new() { Position = 3, DriverName = "M. MARQUEZ", TeamName = "Gresini Ducati", Points = 248 },
    };

    private static List<ConstructorStandingsEntry> F1ConstructorStandings() => new()
    {
        new() { Position = 1, TeamName = "Red Bull Racing", Points = 443 },
        new() { Position = 2, TeamName = "Mercedes", Points = 383 },
        new() { Position = 3, TeamName = "Ferrari", Points = 343 },
    };

    private static List<ConstructorStandingsEntry> WecConstructorStandings() => new()
    {
        new() { Position = 1, TeamName = "Toyota Gazoo Racing", Points = 145 },
        new() { Position = 2, TeamName = "Ferrari AF Corse", Points = 132 },
        new() { Position = 3, TeamName = "Porsche Penske", Points = 118 },
    };

    private static List<ConstructorStandingsEntry> WrcConstructorStandings() => new()
    {
        new() { Position = 1, TeamName = "Toyota GR Yaris", Points = 378 },
        new() { Position = 2, TeamName = "Hyundai i20 N", Points = 344 },
        new() { Position = 3, TeamName = "M-Sport Ford", Points = 290 },
    };

    private static List<ConstructorStandingsEntry> MotoGpConstructorStandings() => new()
    {
        new() { Position = 1, TeamName = "Ducati Lenovo", Points = 420 },
        new() { Position = 2, TeamName = "Prima Pramac", Points = 385 },
        new() { Position = 3, TeamName = "Aprilia Racing", Points = 310 },
    };

    private static List<TrackPosition> F1Track() => new()
    {
        new() { Position = 1, DriverName = "M. VERSTAPPEN", Sector = 3, LastTime = "1:24.332" },
        new() { Position = 2, DriverName = "L. HAMILTON", Sector = 3, LastTime = "1:24.546" },
        new() { Position = 3, DriverName = "C. LECLERC", Sector = 2, LastTime = "1:24.810" },
    };

    private static List<TrackPosition> WecTrack() => new()
    {
        new() { Position = 1, DriverName = "TOYOTA #7", Sector = 1, LastTime = "3:28.112" },
        new() { Position = 2, DriverName = "FERRARI #50", Sector = 1, LastTime = "3:28.450" },
        new() { Position = 3, DriverName = "PORSCHE #6", Sector = 2, LastTime = "3:29.001" },
    };

    private static List<TrackPosition> WrcTrack() => new()
    {
        new() { Position = 1, DriverName = "K. ROVANPERA", Sector = 2, LastTime = "4:12.8" },
        new() { Position = 2, DriverName = "T. NEUVILLE", Sector = 1, LastTime = "4:15.2" },
        new() { Position = 3, DriverName = "O. TANAK", Sector = 3, LastTime = "4:18.1" },
    };

    private static List<TrackPosition> MotoGpTrack() => new()
    {
        new() { Position = 1, DriverName = "F. BAGNAIA", Sector = 4, LastTime = "1:45.332" },
        new() { Position = 2, DriverName = "M. MARQUEZ", Sector = 3, LastTime = "1:45.510" },
        new() { Position = 3, DriverName = "J. MARTIN", Sector = 4, LastTime = "1:45.720" },
    };
}
