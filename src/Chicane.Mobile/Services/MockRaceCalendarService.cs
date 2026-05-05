using Chicane.Mobile.Models;

namespace Chicane.Mobile.Services;

public class MockRaceCalendarService : IRaceCalendarService
{
    public Task<List<RaceScheduleEntry>> GetUpcomingRacesAsync()
    {
        var races = new List<RaceScheduleEntry>
        {
            new() { EventName = "MONZA GP", CircuitName = "Autodromo Nazionale di Monza", Country = "Italy", EventDate = DateTime.UtcNow.AddDays(3), WeatherCondition = "Rain Expected", WeatherIcon = "\u26C8", TemperatureC = 18, IsCompleted = false },
            new() { EventName = "MONACO GP", CircuitName = "Circuit de Monaco", Country = "Monaco", EventDate = DateTime.UtcNow.AddDays(17), WeatherCondition = "Sunny", WeatherIcon = "\u2600", TemperatureC = 24, IsCompleted = false },
            new() { EventName = "SILVERSTONE GP", CircuitName = "Silverstone Circuit", Country = "Great Britain", EventDate = DateTime.UtcNow.AddDays(31), WeatherCondition = "Cloudy", WeatherIcon = "\u2601", TemperatureC = 16, IsCompleted = false },
            new() { EventName = "SPA GP", CircuitName = "Circuit de Spa-Francorchamps", Country = "Belgium", EventDate = DateTime.UtcNow.AddDays(45), WeatherCondition = "Rain Expected", WeatherIcon = "\u26C8", TemperatureC = 14, IsCompleted = false },
            new() { EventName = "HUNGARORING GP", CircuitName = "Hungaroring", Country = "Hungary", EventDate = DateTime.UtcNow.AddDays(59), WeatherCondition = "Sunny", WeatherIcon = "\u2600", TemperatureC = 30, IsCompleted = false },
            new() { EventName = "ZANDVOORT GP", CircuitName = "Circuit Zandvoort", Country = "Netherlands", EventDate = DateTime.UtcNow.AddDays(73), WeatherCondition = "Cloudy", WeatherIcon = "\u2601", TemperatureC = 19, IsCompleted = false },
            new() { EventName = "SUZUKA GP", CircuitName = "Suzuka International", Country = "Japan", EventDate = DateTime.UtcNow.AddDays(87), WeatherCondition = "Sunny", WeatherIcon = "\u2600", TemperatureC = 22, IsCompleted = false },
            new() { EventName = "SINGAPORE GP", CircuitName = "Marina Bay Street Circuit", Country = "Singapore", EventDate = DateTime.UtcNow.AddDays(101), WeatherCondition = "Thunderstorm", WeatherIcon = "\u26C8", TemperatureC = 31, IsCompleted = false },
            new() { EventName = "ABU DHABI GP", CircuitName = "Yas Marina Circuit", Country = "UAE", EventDate = DateTime.UtcNow.AddDays(115), WeatherCondition = "Clear", WeatherIcon = "\u2600", TemperatureC = 28, IsCompleted = false },
            new() { EventName = "BAHRAIN GP", CircuitName = "Bahrain International Circuit", Country = "Bahrain", EventDate = DateTime.UtcNow.AddDays(129), WeatherCondition = "Clear", WeatherIcon = "\u2600", TemperatureC = 34, IsCompleted = false }
        };
        return Task.FromResult(races);
    }

    public async Task<List<RaceScheduleEntry>> GetPastRacesAsync()
    {
        var races = new List<RaceScheduleEntry>
        {
            new() { EventName = "AUSTRALIAN GP", CircuitName = "Albert Park Circuit", Country = "Australia", EventDate = DateTime.UtcNow.AddDays(-95), WeatherCondition = "Sunny", WeatherIcon = "\u2600", TemperatureC = 26, IsCompleted = true },
            new() { EventName = "SAUDI ARABIAN GP", CircuitName = "Jeddah Corniche Circuit", Country = "Saudi Arabia", EventDate = DateTime.UtcNow.AddDays(-80), WeatherCondition = "Clear", WeatherIcon = "\u2600", TemperatureC = 29, IsCompleted = true },
            new() { EventName = "MIAMI GP", CircuitName = "Miami International Autodrome", Country = "USA", EventDate = DateTime.UtcNow.AddDays(-66), WeatherCondition = "Sunny", WeatherIcon = "\u2600", TemperatureC = 31, IsCompleted = true },
            new() { EventName = "IMOLA GP", CircuitName = "Autodromo Enzo e Dino Ferrari", Country = "Italy", EventDate = DateTime.UtcNow.AddDays(-52), WeatherCondition = "Cloudy", WeatherIcon = "\u2601", TemperatureC = 17, IsCompleted = true },
            new() { EventName = "BARCELONA GP", CircuitName = "Circuit de Barcelona-Catalunya", Country = "Spain", EventDate = DateTime.UtcNow.AddDays(-38), WeatherCondition = "Sunny", WeatherIcon = "\u2600", TemperatureC = 25, IsCompleted = true },
            new() { EventName = "MONTREAL GP", CircuitName = "Circuit Gilles Villeneuve", Country = "Canada", EventDate = DateTime.UtcNow.AddDays(-24), WeatherCondition = "Rain", WeatherIcon = "\u26C8", TemperatureC = 15, IsCompleted = true },
            new() { EventName = "RED BULL RING GP", CircuitName = "Red Bull Ring", Country = "Austria", EventDate = DateTime.UtcNow.AddDays(-10), WeatherCondition = "Sunny", WeatherIcon = "\u2600", TemperatureC = 27, IsCompleted = true }
        };

        foreach (var race in races)
        {
            var detail = await GetPastRaceDetailAsync(race.EventName);
            var top3 = detail.Results
                .Where(r => !r.IsDnf)
                .Take(3)
                .Select(r => $"{r.Position}. {GetLastName(r.DriverName)}")
                .ToList();
            race.Result = string.Join(" ", top3);
        }

        return races;
    }

    private static string GetLastName(string fullName)
    {
        var parts = fullName.Split(' ');
        var lastName = parts.Length > 1 ? parts[^1] : fullName;
        return lastName.Length > 3 ? lastName[..3] : lastName.ToUpper();
    }

    public Task<PastRaceDetail> GetPastRaceDetailAsync(string eventName)
    {
        return Task.FromResult(eventName switch
        {
            "AUSTRALIAN GP" => BuildAustralianGp(),
            "SAUDI ARABIAN GP" => BuildSaudiGp(),
            "MIAMI GP" => BuildMiamiGp(),
            _ => BuildGenericPastRaceDetail(eventName)
        });
    }

    private static PastRaceDetail BuildAustralianGp()
    {
        return new PastRaceDetail
        {
            EventName = "AUSTRALIAN GP",
            YellowFlagCount = 2,
            Results = new List<RaceResultEntry>
            {
                new() { Position = 1, DriverName = "M. VERSTAPPEN", TeamName = "Red Bull Racing", TimeOrDnf = "1:24.332", IsFastestLap = false },
                new() { Position = 2, DriverName = "L. HAMILTON", TeamName = "Mercedes", TimeOrDnf = "1:24.546", IsFastestLap = false },
                new() { Position = 3, DriverName = "C. LECLERC", TeamName = "Ferrari", TimeOrDnf = "1:24.810", IsFastestLap = true },
                new() { Position = 4, DriverName = "O. PIASTRI", TeamName = "McLaren", TimeOrDnf = "1:25.102", IsFastestLap = false },
                new() { Position = 5, DriverName = "L. NORRIS", TeamName = "McLaren", TimeOrDnf = "1:25.338", IsFastestLap = false },
                new() { Position = 6, DriverName = "S. PEREZ", TeamName = "Red Bull Racing", TimeOrDnf = "1:25.550", IsFastestLap = false },
                new() { Position = 7, DriverName = "G. RUSSELL", TeamName = "Mercedes", TimeOrDnf = "1:25.721", IsFastestLap = false },
                new() { Position = 8, DriverName = "F. ALONSO", TeamName = "Aston Martin", TimeOrDnf = "1:26.003", IsFastestLap = false },
                new() { Position = 9, DriverName = "C. SAINZ", TeamName = "Ferrari", TimeOrDnf = "1:26.210", IsFastestLap = false },
                new() { Position = 10, DriverName = "L. STROLL", TeamName = "Aston Martin", TimeOrDnf = "1:26.445", IsFastestLap = false },
                new() { Position = 11, DriverName = "E. OCON", TeamName = "Alpine", TimeOrDnf = "1:26.680", IsFastestLap = false },
                new() { Position = 12, DriverName = "P. GASLY", TeamName = "Alpine", TimeOrDnf = "1:26.810", IsFastestLap = false },
                new() { Position = 13, DriverName = "Y. TSUNODA", TeamName = "RB", TimeOrDnf = "1:27.120", IsFastestLap = false },
                new() { Position = 14, DriverName = "N. HULKENBERG", TeamName = "Haas", TimeOrDnf = "1:27.450", IsFastestLap = false },
                new() { Position = 15, DriverName = "D. RICCIARDO", TeamName = "RB", TimeOrDnf = "1:27.890", IsFastestLap = false },
                new() { Position = 16, DriverName = "A. ALBON", TeamName = "Williams", TimeOrDnf = "1:28.220", IsFastestLap = false },
                new() { Position = 17, DriverName = "V. BOTTAS", TeamName = "Sauber", TimeOrDnf = "1:28.560", IsFastestLap = false },
                new() { Position = 18, DriverName = "Z. GUANYU", TeamName = "Sauber", TimeOrDnf = "1:29.110", IsFastestLap = false },
                new() { Position = 19, DriverName = "L. SARGEANT", TeamName = "Williams", TimeOrDnf = "1:29.450", IsFastestLap = false },
                new() { Position = 20, DriverName = "K. MAGNUSSEN", TeamName = "Haas", TimeOrDnf = "DNF", IsDnf = true, IsFastestLap = false }
            },
            DriverStandings = BuildStandingsAfterAuGp(),
            ConstructorStandings = BuildConstructorAfterAuGp()
        };
    }

    private static PastRaceDetail BuildSaudiGp()
    {
        return new PastRaceDetail
        {
            EventName = "SAUDI ARABIAN GP",
            YellowFlagCount = 0,
            Results = new List<RaceResultEntry>
            {
                new() { Position = 1, DriverName = "M. VERSTAPPEN", TeamName = "Red Bull Racing", TimeOrDnf = "1:30.112", IsFastestLap = true },
                new() { Position = 2, DriverName = "C. LECLERC", TeamName = "Ferrari", TimeOrDnf = "1:30.315", IsFastestLap = false },
                new() { Position = 3, DriverName = "S. PEREZ", TeamName = "Red Bull Racing", TimeOrDnf = "1:30.610", IsFastestLap = false },
                new() { Position = 4, DriverName = "L. HAMILTON", TeamName = "Mercedes", TimeOrDnf = "1:30.820", IsFastestLap = false },
                new() { Position = 5, DriverName = "L. NORRIS", TeamName = "McLaren", TimeOrDnf = "1:31.040", IsFastestLap = false },
                new() { Position = 6, DriverName = "O. PIASTRI", TeamName = "McLaren", TimeOrDnf = "1:31.310", IsFastestLap = false },
                new() { Position = 7, DriverName = "G. RUSSELL", TeamName = "Mercedes", TimeOrDnf = "1:31.550", IsFastestLap = false },
                new() { Position = 8, DriverName = "F. ALONSO", TeamName = "Aston Martin", TimeOrDnf = "1:31.890", IsFastestLap = false },
                new() { Position = 9, DriverName = "C. SAINZ", TeamName = "Ferrari", TimeOrDnf = "1:32.140", IsFastestLap = false },
                new() { Position = 10, DriverName = "L. STROLL", TeamName = "Aston Martin", TimeOrDnf = "1:32.450", IsFastestLap = false },
                new() { Position = 11, DriverName = "E. OCON", TeamName = "Alpine", TimeOrDnf = "1:32.780", IsFastestLap = false },
                new() { Position = 12, DriverName = "P. GASLY", TeamName = "Alpine", TimeOrDnf = "1:33.010", IsFastestLap = false },
                new() { Position = 13, DriverName = "Y. TSUNODA", TeamName = "RB", TimeOrDnf = "1:33.350", IsFastestLap = false },
                new() { Position = 14, DriverName = "N. HULKENBERG", TeamName = "Haas", TimeOrDnf = "1:33.690", IsFastestLap = false },
                new() { Position = 15, DriverName = "D. RICCIARDO", TeamName = "RB", TimeOrDnf = "1:34.020", IsFastestLap = false },
                new() { Position = 16, DriverName = "Z. GUANYU", TeamName = "Sauber", TimeOrDnf = "1:34.450", IsFastestLap = false },
                new() { Position = 17, DriverName = "K. MAGNUSSEN", TeamName = "Haas", TimeOrDnf = "1:34.780", IsFastestLap = false },
                new() { Position = 18, DriverName = "V. BOTTAS", TeamName = "Sauber", TimeOrDnf = "1:35.110", IsFastestLap = false },
                new() { Position = 19, DriverName = "L. SARGEANT", TeamName = "Williams", TimeOrDnf = "1:36.020", IsFastestLap = false },
                new() { Position = 20, DriverName = "A. ALBON", TeamName = "Williams", TimeOrDnf = "DNF", IsDnf = true, IsFastestLap = false }
            },
            DriverStandings = BuildStandingsAfterAuGp(),
            ConstructorStandings = BuildConstructorAfterAuGp()
        };
    }

    private static PastRaceDetail BuildMiamiGp()
    {
        return new PastRaceDetail
        {
            EventName = "MIAMI GP",
            YellowFlagCount = 1,
            Results = new List<RaceResultEntry>
            {
                new() { Position = 1, DriverName = "L. HAMILTON", TeamName = "Mercedes", TimeOrDnf = "1:28.910", IsFastestLap = true },
                new() { Position = 2, DriverName = "M. VERSTAPPEN", TeamName = "Red Bull Racing", TimeOrDnf = "1:29.120", IsFastestLap = false },
                new() { Position = 3, DriverName = "L. NORRIS", TeamName = "McLaren", TimeOrDnf = "1:29.440", IsFastestLap = false },
                new() { Position = 4, DriverName = "C. LECLERC", TeamName = "Ferrari", TimeOrDnf = "1:29.710", IsFastestLap = false },
                new() { Position = 5, DriverName = "O. PIASTRI", TeamName = "McLaren", TimeOrDnf = "1:29.980", IsFastestLap = false },
                new() { Position = 6, DriverName = "S. PEREZ", TeamName = "Red Bull Racing", TimeOrDnf = "1:30.210", IsFastestLap = false },
                new() { Position = 7, DriverName = "G. RUSSELL", TeamName = "Mercedes", TimeOrDnf = "1:30.550", IsFastestLap = false },
                new() { Position = 8, DriverName = "F. ALONSO", TeamName = "Aston Martin", TimeOrDnf = "1:30.880", IsFastestLap = false },
                new() { Position = 9, DriverName = "C. SAINZ", TeamName = "Ferrari", TimeOrDnf = "1:31.120", IsFastestLap = false },
                new() { Position = 10, DriverName = "L. STROLL", TeamName = "Aston Martin", TimeOrDnf = "1:31.450", IsFastestLap = false },
                new() { Position = 11, DriverName = "E. OCON", TeamName = "Alpine", TimeOrDnf = "1:31.780", IsFastestLap = false },
                new() { Position = 12, DriverName = "P. GASLY", TeamName = "Alpine", TimeOrDnf = "1:32.010", IsFastestLap = false },
                new() { Position = 13, DriverName = "Y. TSUNODA", TeamName = "RB", TimeOrDnf = "1:32.340", IsFastestLap = false },
                new() { Position = 14, DriverName = "N. HULKENBERG", TeamName = "Haas", TimeOrDnf = "1:32.680", IsFastestLap = false },
                new() { Position = 15, DriverName = "D. RICCIARDO", TeamName = "RB", TimeOrDnf = "1:33.010", IsFastestLap = false },
                new() { Position = 16, DriverName = "V. BOTTAS", TeamName = "Sauber", TimeOrDnf = "1:33.450", IsFastestLap = false },
                new() { Position = 17, DriverName = "A. ALBON", TeamName = "Williams", TimeOrDnf = "1:33.890", IsFastestLap = false },
                new() { Position = 18, DriverName = "K. MAGNUSSEN", TeamName = "Haas", TimeOrDnf = "1:34.220", IsFastestLap = false },
                new() { Position = 19, DriverName = "Z. GUANYU", TeamName = "Sauber", TimeOrDnf = "1:34.660", IsFastestLap = false },
                new() { Position = 20, DriverName = "L. SARGEANT", TeamName = "Williams", TimeOrDnf = "DNF", IsDnf = true }
            },
            DriverStandings = BuildStandingsAfterAuGp(),
            ConstructorStandings = BuildConstructorAfterAuGp()
        };
    }

    private static PastRaceDetail BuildGenericPastRaceDetail(string eventName)
    {
        return BuildAustralianGp() with { EventName = eventName };
    }

    private static List<StandingsEntry> BuildStandingsAfterAuGp()
    {
        return new List<StandingsEntry>
        {
            new() { Position = 1,  DriverName = "M. VERSTAPPEN",  TeamName = "Red Bull Racing", Points = 278 },
            new() { Position = 2,  DriverName = "L. HAMILTON",    TeamName = "Mercedes",        Points = 235 },
            new() { Position = 3,  DriverName = "C. LECLERC",     TeamName = "Ferrari",         Points = 218 },
            new() { Position = 4,  DriverName = "O. PIASTRI",     TeamName = "McLaren",         Points = 196 },
            new() { Position = 5,  DriverName = "L. NORRIS",      TeamName = "McLaren",         Points = 182 },
            new() { Position = 6,  DriverName = "S. PEREZ",       TeamName = "Red Bull Racing", Points = 165 },
            new() { Position = 7,  DriverName = "G. RUSSELL",     TeamName = "Mercedes",        Points = 148 },
            new() { Position = 8,  DriverName = "F. ALONSO",      TeamName = "Aston Martin",    Points = 132 },
            new() { Position = 9,  DriverName = "C. SAINZ",       TeamName = "Ferrari",         Points = 125 },
            new() { Position = 10, DriverName = "L. STROLL",      TeamName = "Aston Martin",    Points = 84 },
            new() { Position = 11, DriverName = "E. OCON",        TeamName = "Alpine",          Points = 58 },
            new() { Position = 12, DriverName = "P. GASLY",       TeamName = "Alpine",          Points = 52 },
            new() { Position = 13, DriverName = "Y. TSUNODA",     TeamName = "RB",              Points = 38 },
            new() { Position = 14, DriverName = "N. HULKENBERG",  TeamName = "Haas",            Points = 32 },
            new() { Position = 15, DriverName = "D. RICCIARDO",   TeamName = "RB",              Points = 28 },
            new() { Position = 16, DriverName = "K. MAGNUSSEN",   TeamName = "Haas",            Points = 24 },
            new() { Position = 17, DriverName = "A. ALBON",       TeamName = "Williams",        Points = 18 },
            new() { Position = 18, DriverName = "V. BOTTAS",      TeamName = "Sauber",          Points = 14 },
            new() { Position = 19, DriverName = "Z. GUANYU",      TeamName = "Sauber",          Points = 8 },
            new() { Position = 20, DriverName = "L. SARGEANT",    TeamName = "Williams",        Points = 4 }
        };
    }

    private static List<ConstructorStandingsEntry> BuildConstructorAfterAuGp()
    {
        return new List<ConstructorStandingsEntry>
        {
            new() { Position = 1,  TeamName = "Red Bull Racing",  Points = 443 },
            new() { Position = 2,  TeamName = "Mercedes",         Points = 383 },
            new() { Position = 3,  TeamName = "Ferrari",          Points = 343 },
            new() { Position = 4,  TeamName = "McLaren",          Points = 378 },
            new() { Position = 5,  TeamName = "Aston Martin",     Points = 216 },
            new() { Position = 6,  TeamName = "Alpine",           Points = 110 },
            new() { Position = 7,  TeamName = "RB",               Points = 66  },
            new() { Position = 8,  TeamName = "Haas",             Points = 56  },
            new() { Position = 9,  TeamName = "Williams",         Points = 22  },
            new() { Position = 10, TeamName = "Sauber",           Points = 22  }
        };
    }
}
