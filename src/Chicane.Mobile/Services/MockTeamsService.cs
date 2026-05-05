using Chicane.Mobile.Models;

namespace Chicane.Mobile.Services;

public class MockTeamsService : ITeamsService
{
    public Task<List<TeamEntry>> GetTeamsAsync(string categoryId) => Task.FromResult(categoryId switch
    {
        "f1" => F1Teams(),
        "wec" => WecTeams(),
        "wrc" => WrcTeams(),
        "motogp" => MotoGpTeams(),
        _ => new List<TeamEntry>()
    });

    public Task<List<DriverStandingsEntry>> GetDriversAsync(string categoryId) => Task.FromResult(categoryId switch
    {
        "f1" => F1Drivers(),
        "wec" => WecDrivers(),
        "wrc" => WrcDrivers(),
        "motogp" => MotoGpDrivers(),
        _ => new List<DriverStandingsEntry>()
    });

    public Task<TeamDetail> GetTeamDetailAsync(string teamName, string categoryId)
    {
        return Task.FromResult(categoryId switch
        {
            "f1" => GetF1TeamDetail(teamName),
            "wec" => BuildGenericTeamDetail(teamName),
            "wrc" => BuildGenericTeamDetail(teamName),
            "motogp" => BuildGenericTeamDetail(teamName),
            _ => BuildGenericTeamDetail(teamName)
        });
    }

    private static TeamDetail GetF1TeamDetail(string teamName) => teamName switch
    {
        "Ferrari" => BuildFerrariDetail(),
        "Red Bull Racing" => BuildRedBullDetail(),
        "Mercedes" => BuildMercedesDetail(),
        _ => BuildGenericTeamDetail(teamName)
    };

    // ────────────────── F1 DATA ──────────────────
    private static List<TeamEntry> F1Teams() => new()
    {
        new() { Position = 1, TeamName = "Red Bull Racing",  Points = 443, BorderColor = "#1A3A5C", GradientColor = "#1A3A5C", Drivers = new() { new() { Name = "M. VERSTAPPEN", Points = 278 }, new() { Name = "S. PEREZ", Points = 165 } } },
        new() { Position = 2, TeamName = "Mercedes",         Points = 383, BorderColor = "#00A19C", GradientColor = "#00A19C", Drivers = new() { new() { Name = "L. HAMILTON", Points = 235 }, new() { Name = "G. RUSSELL", Points = 148 } } },
        new() { Position = 3, TeamName = "Ferrari",          Points = 343, BorderColor = "#E10600", GradientColor = "#E10600", Drivers = new() { new() { Name = "C. LECLERC", Points = 218 }, new() { Name = "C. SAINZ", Points = 125 } } },
        new() { Position = 4, TeamName = "McLaren",          Points = 378, BorderColor = "#FF8700", GradientColor = "#FF8700", Drivers = new() { new() { Name = "L. NORRIS", Points = 182 }, new() { Name = "O. PIASTRI", Points = 196 } } },
        new() { Position = 5, TeamName = "Aston Martin",     Points = 216, BorderColor = "#006F62", GradientColor = "#006F62", Drivers = new() { new() { Name = "F. ALONSO", Points = 132 }, new() { Name = "L. STROLL", Points = 84 } } },
        new() { Position = 6, TeamName = "Alpine",           Points = 110, BorderColor = "#005BAA", GradientColor = "#005BAA", Drivers = new() { new() { Name = "P. GASLY", Points = 52 }, new() { Name = "E. OCON", Points = 58 } } },
        new() { Position = 7, TeamName = "RB",               Points = 66,  BorderColor = "#6699FF", GradientColor = "#6699FF", Drivers = new() { new() { Name = "Y. TSUNODA", Points = 38 }, new() { Name = "D. RICCIARDO", Points = 28 } } },
        new() { Position = 8, TeamName = "Haas",             Points = 56,  BorderColor = "#B6B6B6", GradientColor = "#B6B6B6", Drivers = new() { new() { Name = "N. HULKENBERG", Points = 32 }, new() { Name = "K. MAGNUSSEN", Points = 24 } } },
        new() { Position = 9, TeamName = "Williams",         Points = 22,  BorderColor = "#005AFF", GradientColor = "#005AFF", Drivers = new() { new() { Name = "A. ALBON", Points = 18 }, new() { Name = "L. SARGEANT", Points = 4 } } },
        new() { Position = 10, TeamName = "Sauber",          Points = 22,  BorderColor = "#00E600", GradientColor = "#00E600", Drivers = new() { new() { Name = "V. BOTTAS", Points = 14 }, new() { Name = "Z. GUANYU", Points = 8 } } },
    };

    private static List<DriverStandingsEntry> F1Drivers() => new()
    {
        new() { Position = 1,  DriverName = "M. VERSTAPPEN",  TeamName = "Red Bull Racing", Points = 278, BorderColor = "#1A3A5C" },
        new() { Position = 2,  DriverName = "L. HAMILTON",    TeamName = "Mercedes",        Points = 235, BorderColor = "#00A19C" },
        new() { Position = 3,  DriverName = "C. LECLERC",     TeamName = "Ferrari",         Points = 218, BorderColor = "#E10600" },
        new() { Position = 4,  DriverName = "O. PIASTRI",     TeamName = "McLaren",         Points = 196, BorderColor = "#FF8700" },
        new() { Position = 5,  DriverName = "L. NORRIS",      TeamName = "McLaren",         Points = 182, BorderColor = "#FF8700" },
    };

    // ────────────────── WEC DATA ──────────────────
    private static List<TeamEntry> WecTeams() => new()
    {
        new() { Position = 1, TeamName = "Toyota Gazoo Racing", Points = 145, BorderColor = "#E60000", GradientColor = "#E60000", Drivers = new() { new() { Name = "K. KOBAYASHI", Points = 75 }, new() { Name = "M. CONWAY", Points = 70 } } },
        new() { Position = 2, TeamName = "Ferrari AF Corse",    Points = 132, BorderColor = "#E10600", GradientColor = "#E10600", Drivers = new() { new() { Name = "A. FUOCO", Points = 68 }, new() { Name = "M. MOLINA", Points = 64 } } },
        new() { Position = 3, TeamName = "Porsche Penske",      Points = 118, BorderColor = "#D5001C", GradientColor = "#D5001C", Drivers = new() { new() { Name = "K. ESTRE", Points = 62 }, new() { Name = "A. LOTTERER", Points = 56 } } },
        new() { Position = 4, TeamName = "Cadillac Racing",     Points = 102, BorderColor = "#003DA5", GradientColor = "#003DA5", Drivers = new() { new() { Name = "E. BAMBER", Points = 54 }, new() { Name = "A. LYNN", Points = 48 } } },
        new() { Position = 5, TeamName = "Peugeot TotalEnergies", Points = 88, BorderColor = "#00A650", GradientColor = "#00A650", Drivers = new() { new() { Name = "J. VERGNE", Points = 46 }, new() { Name = "P. DI RESTA", Points = 42 } } },
    };

    private static List<DriverStandingsEntry> WecDrivers() => new()
    {
        new() { Position = 1, DriverName = "K. KOBAYASHI",    TeamName = "Toyota Gazoo Racing", Points = 75, BorderColor = "#E60000" },
        new() { Position = 2, DriverName = "M. CONWAY",       TeamName = "Toyota Gazoo Racing", Points = 70, BorderColor = "#E60000" },
        new() { Position = 3, DriverName = "A. FUOCO",        TeamName = "Ferrari AF Corse",    Points = 68, BorderColor = "#E10600" },
        new() { Position = 4, DriverName = "M. MOLINA",       TeamName = "Ferrari AF Corse",    Points = 64, BorderColor = "#E10600" },
        new() { Position = 5, DriverName = "K. ESTRE",        TeamName = "Porsche Penske",      Points = 62, BorderColor = "#D5001C" },
    };

    // ────────────────── WRC DATA ──────────────────
    private static List<TeamEntry> WrcTeams() => new()
    {
        new() { Position = 1, TeamName = "Toyota GR Yaris",   Points = 378, BorderColor = "#E60000", GradientColor = "#E60000", Drivers = new() { new() { Name = "K. ROVANPERA", Points = 210 }, new() { Name = "E. EVANS", Points = 168 } } },
        new() { Position = 2, TeamName = "Hyundai i20 N",     Points = 344, BorderColor = "#00B2A9", GradientColor = "#00B2A9", Drivers = new() { new() { Name = "T. NEUVILLE", Points = 182 }, new() { Name = "O. TANAK", Points = 162 } } },
        new() { Position = 3, TeamName = "M-Sport Ford Puma",  Points = 290, BorderColor = "#003DA5", GradientColor = "#003DA5", Drivers = new() { new() { Name = "A. FOURMAUX", Points = 148 }, new() { Name = "G. MUNSTER", Points = 142 } } },
    };

    private static List<DriverStandingsEntry> WrcDrivers() => new()
    {
        new() { Position = 1, DriverName = "K. ROVANPERA",    TeamName = "Toyota GR Yaris",    Points = 210, BorderColor = "#E60000" },
        new() { Position = 2, DriverName = "T. NEUVILLE",     TeamName = "Hyundai i20 N",      Points = 182, BorderColor = "#00B2A9" },
        new() { Position = 3, DriverName = "E. EVANS",        TeamName = "Toyota GR Yaris",    Points = 168, BorderColor = "#E60000" },
        new() { Position = 4, DriverName = "O. TANAK",        TeamName = "Hyundai i20 N",      Points = 162, BorderColor = "#00B2A9" },
        new() { Position = 5, DriverName = "A. FOURMAUX",     TeamName = "M-Sport Ford Puma",  Points = 148, BorderColor = "#003DA5" },
    };

    // ────────────────── MOTOGP DATA ──────────────────
    private static List<TeamEntry> MotoGpTeams() => new()
    {
        new() { Position = 1, TeamName = "Ducati Lenovo",       Points = 420, BorderColor = "#E10600", GradientColor = "#E10600", Drivers = new() { new() { Name = "F. BAGNAIA", Points = 285 }, new() { Name = "E. BASTIANINI", Points = 135 } } },
        new() { Position = 2, TeamName = "Prima Pramac Racing", Points = 385, BorderColor = "#7B2D8E", GradientColor = "#7B2D8E", Drivers = new() { new() { Name = "J. MARTIN", Points = 262 }, new() { Name = "F. MORBIDELLI", Points = 123 } } },
        new() { Position = 3, TeamName = "Aprilia Racing",      Points = 310, BorderColor = "#CC0000", GradientColor = "#CC0000", Drivers = new() { new() { Name = "M. VIÑALES", Points = 190 }, new() { Name = "A. ESPARGARO", Points = 120 } } },
        new() { Position = 4, TeamName = "Red Bull KTM",        Points = 285, BorderColor = "#FF6600", GradientColor = "#FF6600", Drivers = new() { new() { Name = "B. BINDER", Points = 185 }, new() { Name = "J. MILLER", Points = 100 } } },
        new() { Position = 5, TeamName = "Gresini Ducati",      Points = 260, BorderColor = "#00A0E4", GradientColor = "#00A0E4", Drivers = new() { new() { Name = "M. MARQUEZ", Points = 248 }, new() { Name = "A. MARQUEZ", Points = 12 } } },
    };

    private static List<DriverStandingsEntry> MotoGpDrivers() => new()
    {
        new() { Position = 1, DriverName = "F. BAGNAIA",      TeamName = "Ducati Lenovo",       Points = 285, BorderColor = "#E10600" },
        new() { Position = 2, DriverName = "J. MARTIN",       TeamName = "Prima Pramac Racing", Points = 262, BorderColor = "#7B2D8E" },
        new() { Position = 3, DriverName = "M. MARQUEZ",      TeamName = "Gresini Ducati",      Points = 248, BorderColor = "#00A0E4" },
        new() { Position = 4, DriverName = "M. VIÑALES",      TeamName = "Aprilia Racing",      Points = 190, BorderColor = "#CC0000" },
        new() { Position = 5, DriverName = "B. BINDER",       TeamName = "Red Bull KTM",        Points = 185, BorderColor = "#FF6600" },
    };
    
    // ────────────────── DETAIL BUILDERS ──────────────────
    private static TeamDetail BuildFerrariDetail() => new()
    {
        TeamName = "Ferrari", BorderColor = "#E10600", Emoji = "🏎️", GradientColor = "#E10600", TotalPoints = 343,
        Drivers = new() { new() { Name = "C. LECLERC", Points = 218 }, new() { Name = "C. SAINZ", Points = 125 } },
        RaceResults = new()
        {
            new() { EventName = "RED BULL RING GP", EventDate = DateTime.UtcNow.AddDays(-10), Driver1Name = "C. LECLERC", Driver1Points = 25, Driver1Position = 1, Driver1BestLap = "1:24.332", Driver2Name = "C. SAINZ", Driver2Points = 18, Driver2Position = 3, TotalPoints = 43 },
            new() { EventName = "MONTREAL GP", EventDate = DateTime.UtcNow.AddDays(-24), Driver1Name = "C. LECLERC", Driver1Points = 8, Driver1Position = 6, Driver2Name = "C. SAINZ", Driver2Points = 0, Driver2Position = 14, TotalPoints = 8 },
            new() { EventName = "IMOLA GP", EventDate = DateTime.UtcNow.AddDays(-52), Driver1Name = "C. LECLERC", Driver1Points = 15, Driver1Position = 3, Driver1BestLap = "1:26.210", Driver2Name = "C. SAINZ", Driver2Points = 10, Driver2Position = 8, TotalPoints = 25 },
        },
        SeasonStats = new() { Championships = 16, RaceWins = 242, Podiums = 804, PolePositions = 249, FastestLaps = 259, FirstSeason = "1950" },
        Profile = new() { FullName = "Scuderia Ferrari HP", Chassis = "SF-24", Base = "Maranello, Italy", PowerUnit = "Ferrari", TeamPrincipal = "Frédéric Vasseur", TechnicalDirector = "Loïc Serra", ReserveDriver = "O. Bearman", History = "The oldest and most successful team in F1 history.", PreviousSeasonSummary = "2024: 2nd in Constructors with 652 points." },
    };

    private static TeamDetail BuildRedBullDetail() => new()
    {
        TeamName = "Red Bull Racing", BorderColor = "#1A3A5C", Emoji = "🐂", GradientColor = "#1A3A5C", TotalPoints = 443,
        Drivers = new() { new() { Name = "M. VERSTAPPEN", Points = 278 }, new() { Name = "S. PEREZ", Points = 165 } },
        RaceResults = new()
        {
            new() { EventName = "RED BULL RING GP", EventDate = DateTime.UtcNow.AddDays(-10), Driver1Name = "M. VERSTAPPEN", Driver1Points = 25, Driver1Position = 1, Driver1BestLap = "1:24.110", Driver2Name = "S. PEREZ", Driver2Points = 10, Driver2Position = 6, TotalPoints = 35 },
            new() { EventName = "BARCELONA GP", EventDate = DateTime.UtcNow.AddDays(-38), Driver1Name = "M. VERSTAPPEN", Driver1Points = 25, Driver1Position = 1, Driver2Name = "S. PEREZ", Driver2Points = 15, Driver2Position = 3, TotalPoints = 40 },
            new() { EventName = "AUSTRALIAN GP", EventDate = DateTime.UtcNow.AddDays(-95), Driver1Name = "M. VERSTAPPEN", Driver1Points = 25, Driver1Position = 1, Driver2Name = "S. PEREZ", Driver2Points = 8, Driver2Position = 6, TotalPoints = 33 },
        },
        SeasonStats = new() { Championships = 7, RaceWins = 118, Podiums = 275, PolePositions = 103, FastestLaps = 98, FirstSeason = "2005" },
        Profile = new() { FullName = "Oracle Red Bull Racing", Chassis = "RB20", Base = "Milton Keynes, UK", PowerUnit = "Honda RBPT", TeamPrincipal = "Christian Horner", TechnicalDirector = "Pierre Waché", ReserveDriver = "L. Lawson", History = "Founded in 2005, dominating since 2010 with Vettel and Verstappen.", PreviousSeasonSummary = "2024: 1st in Constructors with 860 points." },
    };

    private static TeamDetail BuildMercedesDetail() => new()
    {
        TeamName = "Mercedes", BorderColor = "#00A19C", Emoji = "⚡", GradientColor = "#00A19C", TotalPoints = 383,
        Drivers = new() { new() { Name = "L. HAMILTON", Points = 235 }, new() { Name = "G. RUSSELL", Points = 148 } },
        RaceResults = new()
        {
            new() { EventName = "MONTREAL GP", EventDate = DateTime.UtcNow.AddDays(-24), Driver1Name = "L. HAMILTON", Driver1Points = 25, Driver1Position = 1, Driver1BestLap = "1:26.002", Driver2Name = "G. RUSSELL", Driver2Points = 10, Driver2Position = 5, TotalPoints = 35 },
            new() { EventName = "MIAMI GP", EventDate = DateTime.UtcNow.AddDays(-66), Driver1Name = "L. HAMILTON", Driver1Points = 25, Driver1Position = 1, Driver1BestLap = "1:28.910", Driver2Name = "G. RUSSELL", Driver2Points = 6, Driver2Position = 7, TotalPoints = 31 },
            new() { EventName = "AUSTRALIAN GP", EventDate = DateTime.UtcNow.AddDays(-95), Driver1Name = "L. HAMILTON", Driver1Points = 18, Driver1Position = 2, Driver1BestLap = "1:24.546", Driver2Name = "G. RUSSELL", Driver2Points = 8, Driver2Position = 7, TotalPoints = 26 },
        },
        SeasonStats = new() { Championships = 8, RaceWins = 125, Podiums = 281, PolePositions = 136, FastestLaps = 104, FirstSeason = "1954" },
        Profile = new() { FullName = "Mercedes-AMG Petronas F1 Team", Chassis = "W15", Base = "Brackley, UK", PowerUnit = "Mercedes", TeamPrincipal = "Toto Wolff", TechnicalDirector = "James Allison", ReserveDriver = "M. Schumacher", History = "Returned in 2010, won 8 consecutive Constructors' titles.", PreviousSeasonSummary = "2024: 3rd in Constructors with 409 points." },
    };

    private static TeamDetail BuildGenericTeamDetail(string teamName)
    {
        var colors = GetTeamColors(teamName);
        return new TeamDetail
        {
            TeamName = teamName, BorderColor = colors.Border, GradientColor = colors.Gradient, Emoji = colors.Emoji,
            Drivers = new() { new() { Name = "Driver 1", Points = 0 }, new() { Name = "Driver 2", Points = 0 } },
            SeasonStats = new(), Profile = new() { FullName = teamName, History = "Team details coming soon." },
        };
    }

    private static (string Border, string Gradient, string Emoji) GetTeamColors(string teamName) => teamName switch
    {
        "Ferrari" => ("#E10600", "#E10600", "🏎️"), "Red Bull Racing" => ("#1A3A5C", "#1A3A5C", "🐂"), "Mercedes" => ("#00A19C", "#00A19C", "⚡"),
        "McLaren" => ("#FF8700", "#FF8700", "🔶"), "Aston Martin" => ("#006F62", "#006F62", "💚"), "Alpine" => ("#005BAA", "#005BAA", "🔷"),
        "RB" => ("#6699FF", "#6699FF", "🔹"), "Haas" => ("#B6B6B6", "#B6B6B6", "🔘"), "Williams" => ("#005AFF", "#005AFF", "🔵"), "Sauber" => ("#00E600", "#00E600", "🟢"),
        _ => ("#333333", "#333333", "🏎️")
    };
}
