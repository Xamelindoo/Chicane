using Chicane.Mobile.Models;

namespace Chicane.Mobile.Services;

public class MockTeamsService : ITeamsService
{
    public Task<List<TeamEntry>> GetTeamsAsync()
    {
        var teams = new List<TeamEntry>
        {
            new() { Position = 1,  TeamName = "Red Bull Racing",  Points = 443, BorderColor = "#1A3A5C", Drivers = new() { "M. VERSTAPPEN", "S. PEREZ" } },
            new() { Position = 2,  TeamName = "Mercedes",         Points = 383, BorderColor = "#00A19C", Drivers = new() { "L. HAMILTON", "G. RUSSELL" } },
            new() { Position = 3,  TeamName = "Ferrari",          Points = 343, BorderColor = "#E10600", Drivers = new() { "C. LECLERC", "L. HAMILTON" } },
            new() { Position = 4,  TeamName = "McLaren",          Points = 378, BorderColor = "#FF8700", Drivers = new() { "L. NORRIS", "O. PIASTRI" } },
            new() { Position = 5,  TeamName = "Aston Martin",     Points = 216, BorderColor = "#006F62", Drivers = new() { "F. ALONSO", "L. STROLL" } },
            new() { Position = 6,  TeamName = "Alpine",           Points = 110, BorderColor = "#005BAA", Drivers = new() { "P. GASLY", "E. OCON" } },
            new() { Position = 7,  TeamName = "RB",               Points = 66,  BorderColor = "#6699FF", Drivers = new() { "Y. TSUNODA", "D. RICCIARDO" } },
            new() { Position = 8,  TeamName = "Haas",             Points = 56,  BorderColor = "#B6B6B6", Drivers = new() { "N. HULKENBERG", "K. MAGNUSSEN" } },
            new() { Position = 9,  TeamName = "Williams",         Points = 22,  BorderColor = "#005AFF", Drivers = new() { "A. ALBON", "L. SARGEANT" } },
            new() { Position = 10, TeamName = "Sauber",           Points = 22,  BorderColor = "#00E600", Drivers = new() { "V. BOTTAS", "Z. GUANYU" } }
        };

        return Task.FromResult(teams);
    }
}
