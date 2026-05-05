using Chicane.Mobile.Models;

namespace Chicane.Mobile.Services;

public interface ITeamsService
{
    Task<List<TeamEntry>> GetTeamsAsync();
    Task<List<DriverStandingsEntry>> GetDriversAsync();
    Task<TeamDetail> GetTeamDetailAsync(string teamName);
}
