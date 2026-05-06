using Chicane.Mobile.Models;
using Chicane.Mobile.Models.Storage;
using Chicane.Mobile.Services.Repositories;

namespace Chicane.Mobile.Services;

public class CachedTeamsService : ITeamsService
{
    private readonly MockTeamsService _mock;
    private readonly TeamsRepository _teamsRepo;
    private readonly IF1DataService _f1Data;

    public CachedTeamsService(MockTeamsService mock, TeamsRepository teamsRepo, IF1DataService f1Data)
    {
        _mock = mock;
        _teamsRepo = teamsRepo;
        _f1Data = f1Data;
    }

    public async Task<List<TeamEntry>> GetTeamsAsync(string categoryId)
    {
        try
        {
            List<TeamEntry> teams;

            if (categoryId == "f1")
            {
                var constructorStandings = await _f1Data.GetConstructorStandingsAsync(categoryId);
                var driverStandings = await _f1Data.GetStandingsAsync(categoryId);

                teams = constructorStandings.Select(cs =>
                {
                    var teamDrivers = driverStandings
                        .Where(d => d.TeamName == cs.TeamName)
                        .Take(2)
                        .Select(d => new DriverEntry { Name = d.DriverName, Points = d.Points })
                        .ToList();

                    return new TeamEntry
                    {
                        Position = cs.Position,
                        TeamName = cs.TeamName,
                        Points = cs.Points,
                        BorderColor = GetTeamColor(cs.TeamName),
                        GradientColor = GetTeamColor(cs.TeamName),
                        Drivers = teamDrivers,
                    };
                }).ToList();
            }
            else
            {
                teams = await _mock.GetTeamsAsync(categoryId);
            }

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
    {
        if (categoryId == "f1")
        {
            var standings = await _f1Data.GetStandingsAsync(categoryId);
            return standings.Select(s => new DriverStandingsEntry
            {
                Position = s.Position,
                DriverName = s.DriverName,
                TeamName = s.TeamName,
                Points = s.Points,
                BorderColor = GetTeamColor(s.TeamName),
            }).ToList();
        }
        return await _mock.GetDriversAsync(categoryId);
    }

    public async Task<TeamDetail> GetTeamDetailAsync(string teamName, string categoryId)
        => await _mock.GetTeamDetailAsync(teamName, categoryId);

    private static string GetTeamColor(string teamName) => teamName switch
    {
        "Red Bull" or "Red Bull Racing" => "#1A3A5C",
        "Mercedes" => "#00A19C",
        "Ferrari" => "#E10600",
        "McLaren" => "#FF8700",
        "Aston Martin" => "#006F62",
        "Alpine" => "#005BAA",
        "RB" => "#6699FF",
        "Haas F1 Team" or "Haas" => "#B6B6B6",
        "Williams" => "#005AFF",
        "Sauber" or "Kick Sauber" => "#00E600",
        _ => "#333333",
    };
}
