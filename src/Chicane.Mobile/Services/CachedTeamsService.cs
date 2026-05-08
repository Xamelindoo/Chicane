using Chicane.Mobile.Models;
using Chicane.Mobile.Models.Storage;
using Chicane.Mobile.Services.Repositories;

namespace Chicane.Mobile.Services;

public class CachedTeamsService : ITeamsService
{
    private readonly MockTeamsService _mock;
    private readonly TeamsRepository _teamsRepo;
    private readonly IF1DataService _f1Data;
    private readonly OpenF1ApiService _openF1;

    public CachedTeamsService(MockTeamsService mock, TeamsRepository teamsRepo, IF1DataService f1Data, OpenF1ApiService openF1)
    {
        _mock = mock;
        _teamsRepo = teamsRepo;
        _f1Data = f1Data;
        _openF1 = openF1;
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
                var realColors = await _openF1.GetTeamColorsAsync();

                teams = constructorStandings.Select(cs =>
                {
                    var teamDrivers = driverStandings
                        .Where(d => d.TeamName == cs.TeamName)
                        .Take(2)
                        .Select(d => new DriverEntry { Name = d.DriverName, Points = d.Points })
                        .ToList();

                    var color = ResolveTeamColor(cs.TeamName, realColors);

                    return new TeamEntry
                    {
                        Position = cs.Position,
                        TeamName = cs.TeamName,
                        Points = cs.Points,
                        BorderColor = color,
                        GradientColor = color,
                        Drivers = teamDrivers,
                        TeamLogo = GetTeamLogo(cs.TeamName),
                    };
                }).ToList();

                System.Diagnostics.Debug.WriteLine($"[Teams] F1 teams: {string.Join(", ", teams.Select(t => t.TeamName))}");
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
            var realColors = await _openF1.GetTeamColorsAsync();

            return standings.Select(s => new DriverStandingsEntry
            {
                Position = s.Position,
                DriverName = s.DriverName,
                TeamName = s.TeamName,
                Points = s.Points,
                BorderColor = ResolveTeamColor(s.TeamName, realColors),
                DriverNumber = s.DriverNumber,
                PositionStart = s.PositionStart,
                TeamLogo = GetTeamLogo(s.TeamName),
                HeadshotUrl = s.HeadshotUrl,
            }).ToList();
        }
        return await _mock.GetDriversAsync(categoryId);
    }

    public async Task<TeamDetail> GetTeamDetailAsync(string teamName, string categoryId)
    {
        if (categoryId != "f1")
            return await _mock.GetTeamDetailAsync(teamName, categoryId);

        try
        {
            var constructorStandings = await _f1Data.GetConstructorStandingsAsync(categoryId);
            var driverStandings = await _f1Data.GetStandingsAsync(categoryId);
            var realColors = await _openF1.GetTeamColorsAsync();

            var team = constructorStandings.FirstOrDefault(cs => cs.TeamName == teamName);
            var teamDrivers = driverStandings.Where(d => d.TeamName == teamName).ToList();
            var color = ResolveTeamColor(teamName, realColors);

            return new TeamDetail
            {
                TeamName = team?.TeamName ?? teamName,
                TeamLogo = GetTeamLogo(teamName),
                BorderColor = color,
                GradientColor = color,
                TotalPoints = team?.Points ?? 0,
                Emoji = GetTeamEmoji(teamName),
                Drivers = teamDrivers.Select(d => new DriverEntry
                {
                    Name = d.DriverName,
                    Points = d.Points,
                }).ToList(),
                RaceResults = new List<TeamRaceResult>(),
            };
        }
        catch
        {
            return await _mock.GetTeamDetailAsync(teamName, categoryId);
        }
    }

    private static string ResolveTeamColor(string teamName, Dictionary<string, string>? realColors)
    {
        if (realColors?.TryGetValue(teamName, out var realColor) == true)
            return realColor;
        return GetHardcodedTeamColor(teamName);
    }

    private static string GetHardcodedTeamColor(string teamName) => teamName switch
    {
        "Red Bull" or "Red Bull Racing" => "#1A3A5C",
        "Mercedes" => "#00A19C",
        "Ferrari" => "#E10600",
        "McLaren" => "#FF8700",
        "Aston Martin" => "#006F62",
        "Alpine" => "#005BAA",
        "Racing Bulls" => "#6699FF",
        "Haas F1 Team" or "Haas" => "#B6B6B6",
        "Williams" => "#005AFF",
        "Audi" => "#00E600",
        "Cadillac" => "#333333",
        _ => "#333333",
    };

    private static string GetTeamEmoji(string teamName) => teamName switch
    {
        "Ferrari" => "\U0001f3ce",
        "Red Bull Racing" => "\U0001f402",
        "Mercedes" => "\u26A1",
        "McLaren" => "\U0001f536",
        "Aston Martin" => "\U0001f49a",
        "Alpine" => "\U0001f537",
        "Racing Bulls" => "\U0001f539",
        "Haas F1 Team" or "Haas" => "\U0001f518",
        "Williams" => "\U0001f535",
        "Audi" => "\U0001f7e2",
        "Cadillac" => "\U0001f4cd",
        _ => "\U0001f3ce",
    };

    private static string GetTeamLogo(string teamName) => teamName switch
    {
        "Ferrari" => "team_ferrari.png",
        "Red Bull" or "Red Bull Racing" => "team_redbull.png",
        "Mercedes" => "team_mercedes.png",
        "McLaren" => "team_mclaren.png",
        "Aston Martin" => "team_astonmartin.png",
        "Alpine" => "team_alpine.png",
        "Racing Bulls" => "team_racingbulls.png",
        "Haas F1 Team" or "Haas" => "team_haas.png",
        "Williams" => "team_williams.png",
        "Audi" => "team_audi.png",
        "Cadillac" => "team_cadillac.png",
        _ => "",
    };
}
