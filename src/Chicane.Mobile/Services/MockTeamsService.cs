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
}
