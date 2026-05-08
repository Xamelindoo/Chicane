using Chicane.Mobile.Models;

namespace Chicane.Mobile.Services;

public class MockRaceCalendarService : IRaceCalendarService
{
    public Task<List<RaceScheduleEntry>> GetUpcomingRacesAsync(string categoryId) =>
        Task.FromResult(new List<RaceScheduleEntry>());

    public Task<List<RaceScheduleEntry>> GetPastRacesAsync(string categoryId) =>
        Task.FromResult(new List<RaceScheduleEntry>());

    public Task<PastRaceDetail> GetPastRaceDetailAsync(int sessionKey, string categoryId) =>
        Task.FromResult(new PastRaceDetail
        {
            EventDate = DateTime.UtcNow.AddDays(-30),
            Results = new() { new() { Position = 1, DriverName = "P1 DRIVER", TeamName = "P1 Team", TimeOrDnf = "--:--" } },
            DriverStandings = new() { new() { Position = 1, DriverName = "P1 DRIVER", TeamName = "P1 Team", Points = 100 } },
            ConstructorStandings = new() { new() { Position = 1, TeamName = "P1 Team", Points = 100 } },
        });
}
