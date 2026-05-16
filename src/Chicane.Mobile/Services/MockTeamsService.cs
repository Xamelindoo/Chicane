using Chicane.Mobile.Models;

namespace Chicane.Mobile.Services;

public class MockTeamsService : ITeamsService
{
    public Task<List<TeamEntry>> GetTeamsAsync(string categoryId) =>
        Task.FromResult(new List<TeamEntry>());

    public Task<List<DriverStandingsEntry>> GetDriversAsync(string categoryId) =>
        Task.FromResult(new List<DriverStandingsEntry>());

    public Task<TeamDetail> GetTeamDetailAsync(string teamName, string categoryId) =>
        Task.FromResult(BuildGenericTeamDetail(teamName));

    private static TeamDetail BuildGenericTeamDetail(string teamName) => new()
    {
        TeamName = teamName,
        BorderColor = "#333333",
        GradientColor = "#333333",
        Emoji = "\U0001f3ce",
        Drivers = new() { new() { Name = "Driver 1", Points = 0 }, new() { Name = "Driver 2", Points = 0 } },
    };
}
