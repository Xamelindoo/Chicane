using Chicane.Mobile.Models;

namespace Chicane.Mobile.Services;

public class MockTeamsService : ITeamsService
{
    public Task<List<TeamEntry>> GetTeamsAsync()
    {
        var teams = new List<TeamEntry>
        {
            new() { Position = 1,  TeamName = "Red Bull Racing",  Points = 443, BorderColor = "#1A3A5C", Drivers = new() { new() { Name = "M. VERSTAPPEN", Points = 278 }, new() { Name = "S. PEREZ", Points = 165 } } },
            new() { Position = 2,  TeamName = "Mercedes",         Points = 383, BorderColor = "#00A19C", Drivers = new() { new() { Name = "L. HAMILTON", Points = 235 }, new() { Name = "G. RUSSELL", Points = 148 } } },
            new() { Position = 3,  TeamName = "Ferrari",          Points = 343, BorderColor = "#E10600", Drivers = new() { new() { Name = "C. LECLERC", Points = 218 }, new() { Name = "L. HAMILTON", Points = 235 } } },
            new() { Position = 4,  TeamName = "McLaren",          Points = 378, BorderColor = "#FF8700", Drivers = new() { new() { Name = "L. NORRIS", Points = 182 }, new() { Name = "O. PIASTRI", Points = 196 } } },
            new() { Position = 5,  TeamName = "Aston Martin",     Points = 216, BorderColor = "#006F62", Drivers = new() { new() { Name = "F. ALONSO", Points = 132 }, new() { Name = "L. STROLL", Points = 84 } } },
            new() { Position = 6,  TeamName = "Alpine",           Points = 110, BorderColor = "#005BAA", Drivers = new() { new() { Name = "P. GASLY", Points = 52 }, new() { Name = "E. OCON", Points = 58 } } },
            new() { Position = 7,  TeamName = "RB",               Points = 66,  BorderColor = "#6699FF", Drivers = new() { new() { Name = "Y. TSUNODA", Points = 38 }, new() { Name = "D. RICCIARDO", Points = 28 } } },
            new() { Position = 8,  TeamName = "Haas",             Points = 56,  BorderColor = "#B6B6B6", Drivers = new() { new() { Name = "N. HULKENBERG", Points = 32 }, new() { Name = "K. MAGNUSSEN", Points = 24 } } },
            new() { Position = 9,  TeamName = "Williams",         Points = 22,  BorderColor = "#005AFF", Drivers = new() { new() { Name = "A. ALBON", Points = 18 }, new() { Name = "L. SARGEANT", Points = 4 } } },
            new() { Position = 10, TeamName = "Sauber",           Points = 22,  BorderColor = "#00E600", Drivers = new() { new() { Name = "V. BOTTAS", Points = 14 }, new() { Name = "Z. GUANYU", Points = 8 } } }
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
}
