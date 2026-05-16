using Chicane.Mobile.Models;

namespace Chicane.Mobile.Services;

public class MockTeamsService
{
    public Task<List<TeamEntry>> GetTeamsAsync(string categoryId) =>
        Task.FromResult(new List<TeamEntry>());

    public Task<List<DriverStandingsEntry>> GetDriversAsync(string categoryId) =>
        Task.FromResult(new List<DriverStandingsEntry>());

    public Task<TeamDetail> GetTeamDetailAsync(string teamName, string categoryId) =>
        Task.FromResult(new TeamDetail { TeamName = teamName });
}
