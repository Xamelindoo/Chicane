using Chicane.Mobile.Models;

namespace Chicane.Mobile.Services;

public interface ITeamsService
{
    Task<List<TeamEntry>> GetTeamsAsync(string categoryId);
    Task<List<DriverStandingsEntry>> GetDriversAsync(string categoryId);
    Task<TeamDetail> GetTeamDetailAsync(string teamName, string categoryId);
}
