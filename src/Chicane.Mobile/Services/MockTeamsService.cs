using Chicane.Mobile.Models;

namespace Chicane.Mobile.Services;

public class MockTeamsService : ITeamsService
{
    public Task<List<TeamEntry>> GetTeamsAsync()
    {
        var teams = new List<TeamEntry>
        {
            new() { Position = 1,  TeamName = "Red Bull Racing",  Points = 443, BorderColor = "#1A3A5C", GradientColor = "#1A3A5C", Drivers = new() { new() { Name = "M. VERSTAPPEN", Points = 278 }, new() { Name = "S. PEREZ", Points = 165 } } },
            new() { Position = 2,  TeamName = "Mercedes",         Points = 383, BorderColor = "#00A19C", GradientColor = "#00A19C", Drivers = new() { new() { Name = "L. HAMILTON", Points = 235 }, new() { Name = "G. RUSSELL", Points = 148 } } },
            new() { Position = 3,  TeamName = "Ferrari",          Points = 343, BorderColor = "#E10600", GradientColor = "#E10600", Drivers = new() { new() { Name = "C. LECLERC", Points = 218 }, new() { Name = "C. SAINZ", Points = 125 } } },
            new() { Position = 4,  TeamName = "McLaren",          Points = 378, BorderColor = "#FF8700", GradientColor = "#FF8700", Drivers = new() { new() { Name = "L. NORRIS", Points = 182 }, new() { Name = "O. PIASTRI", Points = 196 } } },
            new() { Position = 5,  TeamName = "Aston Martin",     Points = 216, BorderColor = "#006F62", GradientColor = "#006F62", Drivers = new() { new() { Name = "F. ALONSO", Points = 132 }, new() { Name = "L. STROLL", Points = 84 } } },
            new() { Position = 6,  TeamName = "Alpine",           Points = 110, BorderColor = "#005BAA", GradientColor = "#005BAA", Drivers = new() { new() { Name = "P. GASLY", Points = 52 }, new() { Name = "E. OCON", Points = 58 } } },
            new() { Position = 7,  TeamName = "RB",               Points = 66,  BorderColor = "#6699FF", GradientColor = "#6699FF", Drivers = new() { new() { Name = "Y. TSUNODA", Points = 38 }, new() { Name = "D. RICCIARDO", Points = 28 } } },
            new() { Position = 8,  TeamName = "Haas",             Points = 56,  BorderColor = "#B6B6B6", GradientColor = "#B6B6B6", Drivers = new() { new() { Name = "N. HULKENBERG", Points = 32 }, new() { Name = "K. MAGNUSSEN", Points = 24 } } },
            new() { Position = 9,  TeamName = "Williams",         Points = 22,  BorderColor = "#005AFF", GradientColor = "#005AFF", Drivers = new() { new() { Name = "A. ALBON", Points = 18 }, new() { Name = "L. SARGEANT", Points = 4 } } },
            new() { Position = 10, TeamName = "Sauber",           Points = 22,  BorderColor = "#00E600", GradientColor = "#00E600", Drivers = new() { new() { Name = "V. BOTTAS", Points = 14 }, new() { Name = "Z. GUANYU", Points = 8 } } }
        };

        return Task.FromResult(teams);
    }

    public Task<List<DriverStandingsEntry>> GetDriversAsync()
    {
        var drivers = new List<DriverStandingsEntry>
        {
            new() { Position = 1,  DriverName = "M. VERSTAPPEN",  TeamName = "Red Bull Racing", Points = 278, BorderColor = "#1A3A5C" },
            new() { Position = 2,  DriverName = "L. HAMILTON",    TeamName = "Mercedes",        Points = 235, BorderColor = "#00A19C" },
            new() { Position = 3,  DriverName = "C. LECLERC",     TeamName = "Ferrari",         Points = 218, BorderColor = "#E10600" },
            new() { Position = 4,  DriverName = "O. PIASTRI",     TeamName = "McLaren",         Points = 196, BorderColor = "#FF8700" },
            new() { Position = 5,  DriverName = "L. NORRIS",      TeamName = "McLaren",         Points = 182, BorderColor = "#FF8700" },
            new() { Position = 6,  DriverName = "S. PEREZ",       TeamName = "Red Bull Racing", Points = 165, BorderColor = "#1A3A5C" },
            new() { Position = 7,  DriverName = "G. RUSSELL",     TeamName = "Mercedes",        Points = 148, BorderColor = "#00A19C" },
            new() { Position = 8,  DriverName = "F. ALONSO",      TeamName = "Aston Martin",    Points = 132, BorderColor = "#006F62" },
            new() { Position = 9,  DriverName = "C. SAINZ",       TeamName = "Ferrari",         Points = 125, BorderColor = "#E10600" },
            new() { Position = 10, DriverName = "L. STROLL",      TeamName = "Aston Martin",    Points = 84,  BorderColor = "#006F62" },
            new() { Position = 11, DriverName = "E. OCON",        TeamName = "Alpine",          Points = 58,  BorderColor = "#005BAA" },
            new() { Position = 12, DriverName = "P. GASLY",       TeamName = "Alpine",          Points = 52,  BorderColor = "#005BAA" },
            new() { Position = 13, DriverName = "Y. TSUNODA",     TeamName = "RB",              Points = 38,  BorderColor = "#6699FF" },
            new() { Position = 14, DriverName = "N. HULKENBERG",  TeamName = "Haas",            Points = 32,  BorderColor = "#B6B6B6" },
            new() { Position = 15, DriverName = "D. RICCIARDO",   TeamName = "RB",              Points = 28,  BorderColor = "#6699FF" },
            new() { Position = 16, DriverName = "K. MAGNUSSEN",   TeamName = "Haas",            Points = 24,  BorderColor = "#B6B6B6" },
            new() { Position = 17, DriverName = "A. ALBON",       TeamName = "Williams",        Points = 18,  BorderColor = "#005AFF" },
            new() { Position = 18, DriverName = "V. BOTTAS",      TeamName = "Sauber",          Points = 14,  BorderColor = "#00E600" },
            new() { Position = 19, DriverName = "Z. GUANYU",      TeamName = "Sauber",          Points = 8,   BorderColor = "#00E600" },
            new() { Position = 20, DriverName = "L. SARGEANT",    TeamName = "Williams",        Points = 4,   BorderColor = "#005AFF" }
        };

        return Task.FromResult(drivers);
    }

    public Task<TeamDetail> GetTeamDetailAsync(string teamName)
    {
        return Task.FromResult(teamName switch
        {
            "Ferrari" => BuildFerrariDetail(),
            "Red Bull Racing" => BuildRedBullDetail(),
            "Mercedes" => BuildMercedesDetail(),
            _ => BuildGenericTeamDetail(teamName)
        });
    }

    private static TeamDetail BuildFerrariDetail()
    {
        return new TeamDetail
        {
            TeamName = "Ferrari",
            BorderColor = "#E10600",
            Emoji = "🏎️",
            GradientColor = "#E10600",
            TotalPoints = 343,
            Drivers = new List<DriverEntry>
            {
                new() { Name = "C. LECLERC", Points = 218 },
                new() { Name = "C. SAINZ", Points = 125 }
            },
            RaceResults = new List<TeamRaceResult>
            {
                new() { EventName = "RED BULL RING GP", EventDate = DateTime.UtcNow.AddDays(-10), Driver1Name = "C. LECLERC", Driver1Points = 25, Driver1Position = 1, Driver1BestLap = "1:24.332", Driver2Name = "C. SAINZ", Driver2Points = 18, Driver2Position = 3, Driver2BestLap = null, TotalPoints = 43 },
                new() { EventName = "MONTREAL GP", EventDate = DateTime.UtcNow.AddDays(-24), Driver1Name = "C. LECLERC", Driver1Points = 8, Driver1Position = 6, Driver1BestLap = null, Driver2Name = "C. SAINZ", Driver2Points = 0, Driver2Position = 14, Driver2BestLap = null, TotalPoints = 8 },
                new() { EventName = "BARCELONA GP", EventDate = DateTime.UtcNow.AddDays(-38), Driver1Name = "C. LECLERC", Driver1Points = 12, Driver1Position = 4, Driver1BestLap = null, Driver2Name = "C. SAINZ", Driver2Points = 12, Driver2Position = 5, Driver2BestLap = null, TotalPoints = 24 },
                new() { EventName = "IMOLA GP", EventDate = DateTime.UtcNow.AddDays(-52), Driver1Name = "C. LECLERC", Driver1Points = 15, Driver1Position = 3, Driver1BestLap = "1:26.210", Driver2Name = "C. SAINZ", Driver2Points = 10, Driver2Position = 8, Driver2BestLap = null, TotalPoints = 25 },
                new() { EventName = "MIAMI GP", EventDate = DateTime.UtcNow.AddDays(-66), Driver1Name = "C. LECLERC", Driver1Points = 10, Driver1Position = 4, Driver1BestLap = null, Driver2Name = "C. SAINZ", Driver2Points = 0, Driver2Position = 9, Driver2BestLap = null, TotalPoints = 10 },
                new() { EventName = "SAUDI ARABIAN GP", EventDate = DateTime.UtcNow.AddDays(-80), Driver1Name = "C. LECLERC", Driver1Points = 18, Driver1Position = 2, Driver1BestLap = null, Driver2Name = "C. SAINZ", Driver2Points = 0, Driver2Position = 11, Driver2BestLap = null, TotalPoints = 18 },
                new() { EventName = "AUSTRALIAN GP", EventDate = DateTime.UtcNow.AddDays(-95), Driver1Name = "C. LECLERC", Driver1Points = 15, Driver1Position = 1, Driver1BestLap = null, Driver2Name = "C. SAINZ", Driver2Points = 8, Driver2Position = 6, Driver2BestLap = null, TotalPoints = 23 },
            },
            SeasonStats = new TeamSeasonStats
            {
                Championships = 16,
                RaceWins = 242,
                Podiums = 804,
                PolePositions = 249,
                FastestLaps = 259,
                FirstSeason = "1950",
                HomeRace = "Italian GP"
            },
            Profile = new TeamProfile
            {
                FullName = "Scuderia Ferrari HP",
                Chassis = "SF-24",
                Base = "Maranello, Italy",
                PowerUnit = "Ferrari",
                TeamPrincipal = "Frédéric Vasseur",
                TechnicalDirector = "Loïc Serra",
                ReserveDriver = "O. Bearman / A. Giovinazzi",
                History = "Founded in 1929 by Enzo Ferrari, the Scuderia Ferrari is the oldest and most successful team in Formula One history. The Prancing Horse has competed in every F1 World Championship since 1950, collecting 16 Constructors' titles and 15 Drivers' titles with legends like Schumacher, Lauda, and Ascari.",
                PreviousSeasonSummary = "2024: 2nd in Constructors' Championship with 652 points. Charles Leclerc finished 3rd in drivers' standings with 356 points, while Carlos Sainz secured 5th place with 290 points. The team scored 5 wins across the season."
            }
        };
    }

    private static TeamDetail BuildRedBullDetail()
    {
        return new TeamDetail
        {
            TeamName = "Red Bull Racing",
            BorderColor = "#1A3A5C",
            Emoji = "🐂",
            GradientColor = "#1A3A5C",
            TotalPoints = 443,
            Drivers = new List<DriverEntry>
            {
                new() { Name = "M. VERSTAPPEN", Points = 278 },
                new() { Name = "S. PEREZ", Points = 165 }
            },
            RaceResults = new List<TeamRaceResult>
            {
                new() { EventName = "RED BULL RING GP", EventDate = DateTime.UtcNow.AddDays(-10), Driver1Name = "M. VERSTAPPEN", Driver1Points = 25, Driver1Position = 1, Driver1BestLap = "1:24.110", Driver2Name = "S. PEREZ", Driver2Points = 10, Driver2Position = 6, Driver2BestLap = null, TotalPoints = 35 },
                new() { EventName = "MONTREAL GP", EventDate = DateTime.UtcNow.AddDays(-24), Driver1Name = "M. VERSTAPPEN", Driver1Points = 18, Driver1Position = 2, Driver1BestLap = "1:25.880", Driver2Name = "S. PEREZ", Driver2Points = 0, Driver2Position = 16, Driver2BestLap = null, TotalPoints = 18 },
                new() { EventName = "BARCELONA GP", EventDate = DateTime.UtcNow.AddDays(-38), Driver1Name = "M. VERSTAPPEN", Driver1Points = 25, Driver1Position = 1, Driver1BestLap = null, Driver2Name = "S. PEREZ", Driver2Points = 15, Driver2Position = 3, Driver2BestLap = null, TotalPoints = 40 },
                new() { EventName = "IMOLA GP", EventDate = DateTime.UtcNow.AddDays(-52), Driver1Name = "M. VERSTAPPEN", Driver1Points = 25, Driver1Position = 1, Driver1BestLap = "1:25.110", Driver2Name = "S. PEREZ", Driver2Points = 12, Driver2Position = 4, Driver2BestLap = null, TotalPoints = 37 },
                new() { EventName = "MIAMI GP", EventDate = DateTime.UtcNow.AddDays(-66), Driver1Name = "M. VERSTAPPEN", Driver1Points = 18, Driver1Position = 2, Driver1BestLap = null, Driver2Name = "S. PEREZ", Driver2Points = 8, Driver2Position = 6, Driver2BestLap = null, TotalPoints = 26 },
                new() { EventName = "SAUDI ARABIAN GP", EventDate = DateTime.UtcNow.AddDays(-80), Driver1Name = "M. VERSTAPPEN", Driver1Points = 25, Driver1Position = 1, Driver1BestLap = "1:30.112", Driver2Name = "S. PEREZ", Driver2Points = 15, Driver2Position = 3, Driver2BestLap = null, TotalPoints = 40 },
                new() { EventName = "AUSTRALIAN GP", EventDate = DateTime.UtcNow.AddDays(-95), Driver1Name = "M. VERSTAPPEN", Driver1Points = 25, Driver1Position = 1, Driver1BestLap = null, Driver2Name = "S. PEREZ", Driver2Points = 8, Driver2Position = 6, Driver2BestLap = null, TotalPoints = 33 },
            },
            SeasonStats = new TeamSeasonStats
            {
                Championships = 7,
                RaceWins = 118,
                Podiums = 275,
                PolePositions = 103,
                FastestLaps = 98,
                FirstSeason = "2005",
                HomeRace = "Austrian GP"
            },
            Profile = new TeamProfile
            {
                FullName = "Oracle Red Bull Racing",
                Chassis = "RB20",
                Base = "Milton Keynes, United Kingdom",
                PowerUnit = "Honda RBPT",
                TeamPrincipal = "Christian Horner",
                TechnicalDirector = "Pierre Waché",
                ReserveDriver = "L. Lawson",
                History = "Founded in 2005 after Red Bull purchased the Jaguar Racing team. Under the leadership of Christian Horner and the design genius of Adrian Newey, the team rose to dominance, winning four consecutive Drivers' and Constructors' doubles from 2010 to 2013 with Sebastian Vettel, and later dominating the 2022-2024 seasons with Max Verstappen.",
                PreviousSeasonSummary = "2024: 1st in Constructors' Championship with 860 points. Max Verstappen won his fourth consecutive Drivers' title with 575 points (19 wins in 22 races). Sergio Perez finished 2nd with 285 points. The team achieved a record-breaking 21 wins in a single season."
            }
        };
    }

    private static TeamDetail BuildMercedesDetail()
    {
        return new TeamDetail
        {
            TeamName = "Mercedes",
            BorderColor = "#00A19C",
            Emoji = "⚡",
            GradientColor = "#00A19C",
            TotalPoints = 383,
            Drivers = new List<DriverEntry>
            {
                new() { Name = "L. HAMILTON", Points = 235 },
                new() { Name = "G. RUSSELL", Points = 148 }
            },
            RaceResults = new List<TeamRaceResult>
            {
                new() { EventName = "RED BULL RING GP", EventDate = DateTime.UtcNow.AddDays(-10), Driver1Name = "L. HAMILTON", Driver1Points = 12, Driver1Position = 4, Driver1BestLap = null, Driver2Name = "G. RUSSELL", Driver2Points = 8, Driver2Position = 7, Driver2BestLap = null, TotalPoints = 20 },
                new() { EventName = "MONTREAL GP", EventDate = DateTime.UtcNow.AddDays(-24), Driver1Name = "L. HAMILTON", Driver1Points = 25, Driver1Position = 1, Driver1BestLap = "1:26.002", Driver2Name = "G. RUSSELL", Driver2Points = 10, Driver2Position = 5, Driver2BestLap = null, TotalPoints = 35 },
                new() { EventName = "BARCELONA GP", EventDate = DateTime.UtcNow.AddDays(-38), Driver1Name = "L. HAMILTON", Driver1Points = 18, Driver1Position = 2, Driver1BestLap = null, Driver2Name = "G. RUSSELL", Driver2Points = 15, Driver2Position = 3, Driver2BestLap = "1:25.440", TotalPoints = 33 },
                new() { EventName = "IMOLA GP", EventDate = DateTime.UtcNow.AddDays(-52), Driver1Name = "L. HAMILTON", Driver1Points = 10, Driver1Position = 5, Driver1BestLap = null, Driver2Name = "G. RUSSELL", Driver2Points = 0, Driver2Position = 12, Driver2BestLap = null, TotalPoints = 10 },
                new() { EventName = "MIAMI GP", EventDate = DateTime.UtcNow.AddDays(-66), Driver1Name = "L. HAMILTON", Driver1Points = 25, Driver1Position = 1, Driver1BestLap = "1:28.910", Driver2Name = "G. RUSSELL", Driver2Points = 6, Driver2Position = 7, Driver2BestLap = null, TotalPoints = 31 },
                new() { EventName = "SAUDI ARABIAN GP", EventDate = DateTime.UtcNow.AddDays(-80), Driver1Name = "L. HAMILTON", Driver1Points = 12, Driver1Position = 4, Driver1BestLap = null, Driver2Name = "G. RUSSELL", Driver2Points = 0, Driver2Position = 10, Driver2BestLap = null, TotalPoints = 12 },
                new() { EventName = "AUSTRALIAN GP", EventDate = DateTime.UtcNow.AddDays(-95), Driver1Name = "L. HAMILTON", Driver1Points = 18, Driver1Position = 2, Driver1BestLap = "1:24.546", Driver2Name = "G. RUSSELL", Driver2Points = 8, Driver2Position = 7, Driver2BestLap = null, TotalPoints = 26 },
            },
            SeasonStats = new TeamSeasonStats
            {
                Championships = 8,
                RaceWins = 125,
                Podiums = 281,
                PolePositions = 136,
                FastestLaps = 104,
                FirstSeason = "1954",
                HomeRace = "British GP"
            },
            Profile = new TeamProfile
            {
                FullName = "Mercedes-AMG Petronas F1 Team",
                Chassis = "W15",
                Base = "Brackley, United Kingdom",
                PowerUnit = "Mercedes",
                TeamPrincipal = "Toto Wolff",
                TechnicalDirector = "James Allison",
                ReserveDriver = "M. Schumacher / F. Vesti",
                History = "The Mercedes F1 team returned to the sport in 2010 after purchasing the championship-winning Brawn GP outfit. With the introduction of the hybrid turbo era in 2014, Mercedes began an unprecedented run of dominance, winning eight consecutive Constructors' Championships from 2014 to 2021, powering Lewis Hamilton to six of his seven world titles.",
                PreviousSeasonSummary = "2024: 3rd in Constructors' Championship with 409 points. Lewis Hamilton finished 6th in the drivers' standings with 223 points, George Russell finished 7th with 186 points. The team secured 3 wins, struggling to match the dominant pace of Red Bull."
            }
        };
    }

    private static TeamDetail BuildGenericTeamDetail(string teamName)
    {
        var colors = GetTeamColors(teamName);
        return BuildFerrariDetail() with
        {
            TeamName = teamName,
            BorderColor = colors.Border,
            GradientColor = colors.Gradient,
            Emoji = colors.Emoji,
            TotalPoints = 0,
            SeasonStats = new TeamSeasonStats(),
            Profile = new TeamProfile
            {
                FullName = teamName,
                History = "Team details coming soon.",
                PreviousSeasonSummary = "Season data not available yet."
            }
        };
    }

    private static (string Border, string Gradient, string Emoji) GetTeamColors(string teamName)
    {
        return teamName switch
        {
            "Ferrari" => ("#E10600", "#E10600", "🏎️"),
            "Red Bull Racing" => ("#1A3A5C", "#1A3A5C", "🐂"),
            "Mercedes" => ("#00A19C", "#00A19C", "⚡"),
            "McLaren" => ("#FF8700", "#FF8700", "🔶"),
            "Aston Martin" => ("#006F62", "#006F62", "💚"),
            "Alpine" => ("#005BAA", "#005BAA", "🔷"),
            "RB" => ("#6699FF", "#6699FF", "🔹"),
            "Haas" => ("#B6B6B6", "#B6B6B6", "🔘"),
            "Williams" => ("#005AFF", "#005AFF", "🔵"),
            "Sauber" => ("#00E600", "#00E600", "🟢"),
            _ => ("#333333", "#333333", "🏎️")
        };
    }
}
