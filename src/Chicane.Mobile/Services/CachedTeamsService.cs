using Chicane.Mobile.Models;
using Chicane.Mobile.Models.Storage;
using Chicane.Mobile.Services.Repositories;

namespace Chicane.Mobile.Services;

public class CachedTeamsService : ITeamsService
{
    private readonly MockTeamsService _mock;
    private readonly TeamsRepository _teamsRepo;

    public CachedTeamsService(MockTeamsService mock, TeamsRepository teamsRepo)
    {
        _mock = mock;
        _teamsRepo = teamsRepo;
    }

    public async Task<List<TeamEntry>> GetTeamsAsync(string categoryId)
    {
        try
        {
            var teams = await _mock.GetTeamsAsync(categoryId);
            var records = teams.Select(t => StorageMapper.ToRecord(t, categoryId)).ToList();
            await _teamsRepo.UpsertAsync(categoryId, records);
            return teams;
        }
        catch
        {
            var records = await _teamsRepo.GetByCategoryAsync(categoryId);
            return records.Select(StorageMapper.ToDomain).ToList();
        }
    }

    public async Task<List<DriverStandingsEntry>> GetDriversAsync(string categoryId)
        => await _mock.GetDriversAsync(categoryId);

    public async Task<TeamDetail> GetTeamDetailAsync(string teamName, string categoryId)
        => await _mock.GetTeamDetailAsync(teamName, categoryId);
}
