using Chicane.Mobile.Models;
using Chicane.Mobile.Models.Storage;
using Chicane.Mobile.Services.Repositories;

namespace Chicane.Mobile.Services;

/// <summary>
/// Team and driver data service with SQLite caching.
/// Provides teams list, driver standings (for leaderboards), and team details.
/// All F1 reads are cache-first; cache misses trigger API calls via CachedF1DataService
/// (which itself is cache-first, creating a layered cache architecture).
/// Non-F1 categories fall back to MockTeamsService.
/// Contains static helpers for team colors, emojis, and logo filenames.
/// </summary>
public class CachedTeamsService : ITeamsService
{
    private readonly MockTeamsService _mock;
    private readonly TeamsRepository _teamsRepo;
    private readonly IF1DataService _f1Data;
    private readonly OpenF1ApiService _openF1;
    private readonly IRaceCalendarService _scheduleService;

    public CachedTeamsService(MockTeamsService mock, TeamsRepository teamsRepo, IF1DataService f1Data, OpenF1ApiService openF1, IRaceCalendarService scheduleService)
    {
        _mock = mock;
        _teamsRepo = teamsRepo;
        _f1Data = f1Data;
        _openF1 = openF1;
        _scheduleService = scheduleService;
    }

    // -------- Teams List (cache-first) --------

    /// <summary>
    /// Returns the full list of F1 teams with positions, points, colors, logos, and drivers.
    /// Cache-first: reads from SQLite; on miss, fetches from downstream services and saves to cache.
    /// </summary>
    public async Task<List<TeamEntry>> GetTeamsAsync(string categoryId)
    {
        try
        {
            // Check SQLite cache first
            var cached = await _teamsRepo.GetByCategoryAsync(categoryId);
            if (cached.Count > 0)
                return cached.Select(StorageMapper.ToDomain).ToList();

            List<TeamEntry> teams;

            if (categoryId == "f1")
            {
                // Fetch standings from cached services (layered cache: SQLite → API)
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
            }
            else
            {
                teams = await _mock.GetTeamsAsync(categoryId);
            }

            // Write to cache
            var records = teams.Select(t => StorageMapper.ToRecord(t, categoryId)).ToList();
            await _teamsRepo.UpsertAsync(categoryId, records);
            return teams;
        }
        catch
        {
            var records = await _teamsRepo.GetByCategoryAsync(categoryId);
            if (records.Count > 0)
                return records.Select(StorageMapper.ToDomain).ToList();
            return await _mock.GetTeamsAsync(categoryId);
        }
    }

    // -------- Driver Standings (for Leaderboards, cache-first) --------

    /// <summary>
    /// Returns driver standings with position arrows, headshots, and team colors.
    /// Uses CachedF1DataService.GetStandingsAsync (cache-first) underneath.
    /// </summary>
    public async Task<List<DriverStandingsEntry>> GetDriversAsync(string categoryId)
    {
        if (categoryId != "f1")
            return await _mock.GetDriversAsync(categoryId);

        try
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
        catch
        {
            return await _mock.GetDriversAsync(categoryId);
        }
    }

    // -------- Team Detail (API-first, with fallback) --------

            /// <summary>
            /// Returns full detail for a single team: colors, logo, drivers, and race results.
            /// Race results are populated from cached session results (cache-first, 0 API if loaded).
            /// </summary>
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

                    // Populate race results from cached session results
                    var raceResults = new List<TeamRaceResult>();
                    var pastRaces = (await _scheduleService.GetPastRacesAsync(categoryId))
                        .Where(r => r.IsCompleted)
                        .Take(10);
                    foreach (var race in pastRaces)
                    {
                        if (race.SessionKey <= 0) continue;
                        var detail = await _scheduleService.GetPastRaceDetailAsync(race.SessionKey, categoryId);
                        var teamResults = detail.Results
                            .Where(r => r.TeamName == teamName)
                            .OrderBy(r => r.Position)
                            .Take(2)
                            .ToList();

                        if (teamResults.Count > 0)
                        {
                            raceResults.Add(new TeamRaceResult
                            {
                                EventName = race.MeetingName ?? race.EventName,
                                SessionName = race.EventName,
                                EventDate = race.EventDate,
                                Driver1Name = teamResults.Count > 0 ? teamResults[0].DriverName : "",
                                Driver1Position = teamResults.Count > 0 ? teamResults[0].Position : 0,
                                Driver1Points = teamResults.Count > 0 ? F1Points(teamResults[0].Position) : 0,
                                Driver1BestLap = teamResults.Count > 0 && teamResults[0].IsFastestLap ? teamResults[0].TimeOrDnf : null,
                                Driver2Name = teamResults.Count > 1 ? teamResults[1].DriverName : "",
                                Driver2Position = teamResults.Count > 1 ? teamResults[1].Position : 0,
                                Driver2Points = teamResults.Count > 1 ? F1Points(teamResults[1].Position) : 0,
                                Driver2BestLap = teamResults.Count > 1 && teamResults[1].IsFastestLap ? teamResults[1].TimeOrDnf : null,
                                TotalPoints = teamResults.Sum(r => F1Points(r.Position)),
                            });
                        }
                    }

                    return new TeamDetail
                    {
                        TeamName = team?.TeamName ?? teamName,
                        TeamLogo = GetTeamLogo(teamName),
                        TeamLogoTransparent = GetTeamLogoTransparent(teamName),
                        BorderColor = color,
                        GradientColor = color,
                        TotalPoints = team?.Points ?? 0,
                        Emoji = GetTeamEmoji(teamName),
                        Drivers = teamDrivers.Select(d => new DriverEntry
                        {
                            Name = d.DriverName,
                            Points = d.Points,
                        }).ToList(),
                        RaceResults = raceResults.OrderByDescending(r => r.EventDate).ToList(),
                    };
        }
        catch
        {
            return await _mock.GetTeamDetailAsync(teamName, categoryId);
        }
    }

    // -------- Static helpers: colors, emojis, logos --------

    /// <summary> Resolves team color: API-provided first, hardcoded fallback. </summary>
    private static string ResolveTeamColor(string teamName, Dictionary<string, string>? realColors)
    {
        if (realColors?.TryGetValue(teamName, out var realColor) == true)
            return realColor;
        return GetHardcodedTeamColor(teamName);
    }

    /// <summary> Hardcoded F1 team colors used when API colors are unavailable. </summary>
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

    /// <summary> Unicode emoji for each F1 team. </summary>
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

    /// <summary> Maps team name to transparent PNG logo filename. </summary>
    private static string GetTeamLogo(string teamName) => teamName switch
    {
        "Ferrari" => "team_ferrari_transparent.png",
        "Red Bull" or "Red Bull Racing" => "team_redbull_transparent.png",
        "Mercedes" => "team_mercedes_transparent.png",
        "McLaren" => "team_mclaren_transparent.png",
        "Aston Martin" => "team_astonmartin_transparent.png",
        "Alpine" => "team_alpine_transparent.png",
        "Racing Bulls" => "team_racingbulls_transparent.png",
        "Haas F1 Team" or "Haas" => "team_haas_transparent.png",
        "Williams" => "team_williams_transparent.png",
        "Audi" => "team_audi_transparent.png",
        "Cadillac" => "team_cadillac_transparent.png",
        _ => "",
    };

    /// <summary> Same as GetTeamLogo — kept as separate method for future differentiation. </summary>
    private static string GetTeamLogoTransparent(string teamName) => teamName switch
    {
        "Ferrari" => "team_ferrari_transparent.png",
        "Red Bull" or "Red Bull Racing" => "team_redbull_transparent.png",
        "Mercedes" => "team_mercedes_transparent.png",
        "McLaren" => "team_mclaren_transparent.png",
        "Aston Martin" => "team_astonmartin_transparent.png",
        "Alpine" => "team_alpine_transparent.png",
        "Racing Bulls" => "team_racingbulls_transparent.png",
        "Haas F1 Team" or "Haas" => "team_haas_transparent.png",
        "Williams" => "team_williams_transparent.png",
        "Audi" => "team_audi_transparent.png",
        "Cadillac" => "team_cadillac_transparent.png",
        _ => "",
    };

    /// <summary>F1 2026 points system: 25-18-15-12-10-8-6-4-2-1 for positions 1-10.</summary>
    private static int F1Points(int position) => position switch
    {
        1 => 25, 2 => 18, 3 => 15, 4 => 12, 5 => 10,
        6 => 8, 7 => 6, 8 => 4, 9 => 2, 10 => 1,
        _ => 0
    };
}
