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
}
