using Chicane.Mobile.Models;

namespace Chicane.Mobile.Services;

public class MockRaceCalendarService : IRaceCalendarService
{
    public Task<List<RaceScheduleEntry>> GetUpcomingRacesAsync(string categoryId) =>
        Task.FromResult(categoryId switch
        {
            "f1" => F1Upcoming(),
            "wec" => WecUpcoming(),
            "wrc" => WrcUpcoming(),
            "motogp" => MotoGpUpcoming(),
            _ => new List<RaceScheduleEntry>()
        });

    public async Task<List<RaceScheduleEntry>> GetPastRacesAsync(string categoryId)
    {
        var races = categoryId switch
        {
            "f1" => F1Past(),
            "wec" => WecPast(),
            "wrc" => WrcPast(),
            "motogp" => MotoGpPast(),
            _ => new List<RaceScheduleEntry>()
        };

        foreach (var race in races)
        {
            var detail = await GetPastRaceDetailAsync(race.EventName, categoryId);
            var top3 = detail.Results
                .Where(r => !r.IsDnf).Take(3)
                .Select(r => $"{r.Position}. {GetLastName(r.DriverName)}").ToList();
            race.Result = string.Join(" ", top3);
        }
        return races;
    }

    public Task<PastRaceDetail> GetPastRaceDetailAsync(string eventName, string categoryId) =>
        Task.FromResult(categoryId switch
        {
            "f1" => BuildF1Detail(eventName),
            _ => BuildGenericDetail(eventName)
        });

    private static string GetLastName(string fullName)
    {
        var parts = fullName.Split(' ');
        var lastName = parts.Length > 1 ? parts[^1] : fullName;
        return lastName.Length > 3 ? lastName[..3] : lastName.ToUpper();
    }

    // ═══════════ F1 ═══════════
    private static List<RaceScheduleEntry> F1Upcoming() => new()
    {
        new() { EventName = "MONZA GP", CircuitName = "Autodromo Nazionale di Monza", Country = "Italy", EventDate = DateTime.UtcNow.AddDays(3), WeatherCondition = "Rain Expected", WeatherIcon = "\u26C8", TemperatureC = 18, IsCompleted = false },
        new() { EventName = "MONACO GP", CircuitName = "Circuit de Monaco", Country = "Monaco", EventDate = DateTime.UtcNow.AddDays(17), WeatherCondition = "Sunny", WeatherIcon = "\u2600", TemperatureC = 24, IsCompleted = false },
        new() { EventName = "SILVERSTONE GP", CircuitName = "Silverstone Circuit", Country = "Great Britain", EventDate = DateTime.UtcNow.AddDays(31), WeatherCondition = "Cloudy", WeatherIcon = "\u2601", TemperatureC = 16, IsCompleted = false },
        new() { EventName = "SPA GP", CircuitName = "Circuit de Spa-Francorchamps", Country = "Belgium", EventDate = DateTime.UtcNow.AddDays(45), WeatherCondition = "Rain Expected", WeatherIcon = "\u26C8", TemperatureC = 14, IsCompleted = false },
        new() { EventName = "HUNGARORING GP", CircuitName = "Hungaroring", Country = "Hungary", EventDate = DateTime.UtcNow.AddDays(59), WeatherCondition = "Sunny", WeatherIcon = "\u2600", TemperatureC = 30, IsCompleted = false },
    };

    private static List<RaceScheduleEntry> F1Past() => new()
    {
        new() { EventName = "AUSTRALIAN GP", CircuitName = "Albert Park Circuit", Country = "Australia", EventDate = DateTime.UtcNow.AddDays(-95), WeatherCondition = "Sunny", WeatherIcon = "\u2600", TemperatureC = 26, IsCompleted = true },
        new() { EventName = "SAUDI ARABIAN GP", CircuitName = "Jeddah Corniche Circuit", Country = "Saudi Arabia", EventDate = DateTime.UtcNow.AddDays(-80), WeatherCondition = "Clear", WeatherIcon = "\u2600", TemperatureC = 29, IsCompleted = true },
        new() { EventName = "MIAMI GP", CircuitName = "Miami International Autodrome", Country = "USA", EventDate = DateTime.UtcNow.AddDays(-66), WeatherCondition = "Sunny", WeatherIcon = "\u2600", TemperatureC = 31, IsCompleted = true },
    };

    private static PastRaceDetail BuildF1Detail(string eventName) => eventName switch
    {
        "AUSTRALIAN GP" => BuildAusGp(),
        "SAUDI ARABIAN GP" => BuildSauGp(),
        "MIAMI GP" => BuildMiaGp(),
        _ => BuildGenericDetail(eventName)
    };

    // ═══════════ WEC ═══════════
    private static List<RaceScheduleEntry> WecUpcoming() => new()
    {
        new() { EventName = "6H SPA", CircuitName = "Circuit de Spa-Francorchamps", Country = "Belgium", EventDate = DateTime.UtcNow.AddDays(8), WeatherCondition = "Cloudy", WeatherIcon = "\u2601", TemperatureC = 14, IsCompleted = false },
        new() { EventName = "24H LE MANS", CircuitName = "Circuit de la Sarthe", Country = "France", EventDate = DateTime.UtcNow.AddDays(28), WeatherCondition = "Clear", WeatherIcon = "\u2600", TemperatureC = 22, IsCompleted = false },
        new() { EventName = "6H SAO PAULO", CircuitName = "Interlagos", Country = "Brazil", EventDate = DateTime.UtcNow.AddDays(42), WeatherCondition = "Sunny", WeatherIcon = "\u2600", TemperatureC = 26, IsCompleted = false },
        new() { EventName = "LONE STAR LE MANS", CircuitName = "Circuit of the Americas", Country = "USA", EventDate = DateTime.UtcNow.AddDays(56), WeatherCondition = "Sunny", WeatherIcon = "\u2600", TemperatureC = 32, IsCompleted = false },
        new() { EventName = "6H FUJI", CircuitName = "Fuji Speedway", Country = "Japan", EventDate = DateTime.UtcNow.AddDays(70), WeatherCondition = "Rain", WeatherIcon = "\u26C8", TemperatureC = 18, IsCompleted = false },
    };

    private static List<RaceScheduleEntry> WecPast() => new()
    {
        new() { EventName = "QATAR 1812KM", CircuitName = "Losail International Circuit", Country = "Qatar", EventDate = DateTime.UtcNow.AddDays(-90), WeatherCondition = "Clear", WeatherIcon = "\u2600", TemperatureC = 30, IsCompleted = true },
        new() { EventName = "IMOLA 6H", CircuitName = "Autodromo Enzo e Dino Ferrari", Country = "Italy", EventDate = DateTime.UtcNow.AddDays(-60), WeatherCondition = "Cloudy", WeatherIcon = "\u2601", TemperatureC = 17, IsCompleted = true },
    };

    // ═══════════ WRC ═══════════
    private static List<RaceScheduleEntry> WrcUpcoming() => new()
    {
        new() { EventName = "RALLY PORTUGAL", CircuitName = "Matosinhos", Country = "Portugal", EventDate = DateTime.UtcNow.AddDays(14), WeatherCondition = "Sunny", WeatherIcon = "\u2600", TemperatureC = 24, IsCompleted = false },
        new() { EventName = "RALLY ITALIA", CircuitName = "Olbia", Country = "Italy", EventDate = DateTime.UtcNow.AddDays(35), WeatherCondition = "Sunny", WeatherIcon = "\u2600", TemperatureC = 26, IsCompleted = false },
        new() { EventName = "RALLY POLAND", CircuitName = "Mikolajki", Country = "Poland", EventDate = DateTime.UtcNow.AddDays(49), WeatherCondition = "Cloudy", WeatherIcon = "\u2601", TemperatureC = 18, IsCompleted = false },
        new() { EventName = "RALLY LATVIA", CircuitName = "Liepaja", Country = "Latvia", EventDate = DateTime.UtcNow.AddDays(63), WeatherCondition = "Rain", WeatherIcon = "\u26C8", TemperatureC = 14, IsCompleted = false },
        new() { EventName = "RALLY FINLAND", CircuitName = "Jyvaskyla", Country = "Finland", EventDate = DateTime.UtcNow.AddDays(77), WeatherCondition = "Sunny", WeatherIcon = "\u2600", TemperatureC = 16, IsCompleted = false },
    };

    private static List<RaceScheduleEntry> WrcPast() => new()
    {
        new() { EventName = "RALLYE MONTE-CARLO", CircuitName = "Gap", Country = "Monaco/France", EventDate = DateTime.UtcNow.AddDays(-100), WeatherCondition = "Snow", WeatherIcon = "\u2744", TemperatureC = -2, IsCompleted = true },
        new() { EventName = "RALLY SWEDEN", CircuitName = "Umea", Country = "Sweden", EventDate = DateTime.UtcNow.AddDays(-85), WeatherCondition = "Snow", WeatherIcon = "\u2744", TemperatureC = -8, IsCompleted = true },
        new() { EventName = "RALLY KENYA", CircuitName = "Naivasha", Country = "Kenya", EventDate = DateTime.UtcNow.AddDays(-70), WeatherCondition = "Sunny", WeatherIcon = "\u2600", TemperatureC = 28, IsCompleted = true },
    };

    // ═══════════ MOTOGP ═══════════
    private static List<RaceScheduleEntry> MotoGpUpcoming() => new()
    {
        new() { EventName = "FRENCH GP", CircuitName = "Le Mans Bugatti", Country = "France", EventDate = DateTime.UtcNow.AddDays(10), WeatherCondition = "Cloudy", WeatherIcon = "\u2601", TemperatureC = 18, IsCompleted = false },
        new() { EventName = "CATALAN GP", CircuitName = "Circuit de Barcelona-Catalunya", Country = "Spain", EventDate = DateTime.UtcNow.AddDays(24), WeatherCondition = "Sunny", WeatherIcon = "\u2600", TemperatureC = 26, IsCompleted = false },
        new() { EventName = "ITALIAN GP", CircuitName = "Mugello Circuit", Country = "Italy", EventDate = DateTime.UtcNow.AddDays(38), WeatherCondition = "Sunny", WeatherIcon = "\u2600", TemperatureC = 28, IsCompleted = false },
        new() { EventName = "DUTCH TT", CircuitName = "TT Circuit Assen", Country = "Netherlands", EventDate = DateTime.UtcNow.AddDays(52), WeatherCondition = "Rain", WeatherIcon = "\u26C8", TemperatureC = 16, IsCompleted = false },
        new() { EventName = "GERMAN GP", CircuitName = "Sachsenring", Country = "Germany", EventDate = DateTime.UtcNow.AddDays(66), WeatherCondition = "Cloudy", WeatherIcon = "\u2601", TemperatureC = 20, IsCompleted = false },
    };

    private static List<RaceScheduleEntry> MotoGpPast() => new()
    {
        new() { EventName = "THAILAND GP", CircuitName = "Chang International Circuit", Country = "Thailand", EventDate = DateTime.UtcNow.AddDays(-95), WeatherCondition = "Sunny", WeatherIcon = "\u2600", TemperatureC = 35, IsCompleted = true },
        new() { EventName = "ARGENTINA GP", CircuitName = "Termas de Rio Hondo", Country = "Argentina", EventDate = DateTime.UtcNow.AddDays(-80), WeatherCondition = "Sunny", WeatherIcon = "\u2600", TemperatureC = 28, IsCompleted = true },
        new() { EventName = "AMERICAS GP", CircuitName = "Circuit of the Americas", Country = "USA", EventDate = DateTime.UtcNow.AddDays(-65), WeatherCondition = "Cloudy", WeatherIcon = "\u2601", TemperatureC = 22, IsCompleted = true },
    };

    // ═══════════ DETAIL BUILDERS ═══════════
    private static PastRaceDetail BuildAusGp() => new()
    {
        EventName = "AUSTRALIAN GP", EventDate = DateTime.UtcNow.AddDays(-95), YellowFlagCount = 2,
        Results = new List<RaceResultEntry>
        {
            new() { Position = 1,  DriverName = "M. VERSTAPPEN",  TeamName = "Red Bull Racing", TimeOrDnf = "1:24.332" },
            new() { Position = 2,  DriverName = "L. HAMILTON",    TeamName = "Mercedes",        TimeOrDnf = "1:24.546" },
            new() { Position = 3,  DriverName = "C. LECLERC",     TeamName = "Ferrari",         TimeOrDnf = "1:24.810", IsFastestLap = true },
            new() { Position = 4,  DriverName = "O. PIASTRI",     TeamName = "McLaren",         TimeOrDnf = "1:25.102" },
            new() { Position = 5,  DriverName = "L. NORRIS",      TeamName = "McLaren",         TimeOrDnf = "1:25.338" },
            new() { Position = 6,  DriverName = "S. PEREZ",       TeamName = "Red Bull Racing", TimeOrDnf = "1:25.550" },
            new() { Position = 7,  DriverName = "G. RUSSELL",     TeamName = "Mercedes",        TimeOrDnf = "1:25.721" },
            new() { Position = 8,  DriverName = "F. ALONSO",      TeamName = "Aston Martin",    TimeOrDnf = "1:26.003" },
            new() { Position = 9,  DriverName = "C. SAINZ",       TeamName = "Ferrari",         TimeOrDnf = "1:26.210" },
            new() { Position = 10, DriverName = "L. STROLL",      TeamName = "Aston Martin",    TimeOrDnf = "1:26.445" },
            new() { Position = 11, DriverName = "E. OCON",        TeamName = "Alpine",          TimeOrDnf = "1:26.680" },
            new() { Position = 12, DriverName = "P. GASLY",       TeamName = "Alpine",          TimeOrDnf = "1:26.810" },
            new() { Position = 13, DriverName = "Y. TSUNODA",     TeamName = "RB",              TimeOrDnf = "1:27.120" },
            new() { Position = 14, DriverName = "N. HULKENBERG",  TeamName = "Haas",            TimeOrDnf = "1:27.450" },
            new() { Position = 15, DriverName = "D. RICCIARDO",   TeamName = "RB",              TimeOrDnf = "1:27.890" },
            new() { Position = 16, DriverName = "V. BOTTAS",      TeamName = "Sauber",          TimeOrDnf = "1:28.220" },
            new() { Position = 17, DriverName = "Z. GUANYU",      TeamName = "Sauber",          TimeOrDnf = "1:28.560" },
            new() { Position = 18, DriverName = "A. ALBON",       TeamName = "Williams",        TimeOrDnf = "DNF", IsDnf = true },
            new() { Position = 19, DriverName = "K. MAGNUSSEN",   TeamName = "Haas",            TimeOrDnf = "DNF", IsDnf = true },
        },
        DriverStandings = new()
        {
            new() { Position = 1, DriverName = "M. VERSTAPPEN", TeamName = "Red Bull Racing", Points = 278 },
            new() { Position = 2, DriverName = "L. HAMILTON",   TeamName = "Mercedes",        Points = 235 },
            new() { Position = 3, DriverName = "C. LECLERC",    TeamName = "Ferrari",         Points = 218 },
            new() { Position = 4, DriverName = "O. PIASTRI",    TeamName = "McLaren",         Points = 196 },
            new() { Position = 5, DriverName = "L. NORRIS",     TeamName = "McLaren",         Points = 182 },
        },
        ConstructorStandings = new()
        {
            new() { Position = 1, TeamName = "Red Bull Racing", Points = 443 },
            new() { Position = 2, TeamName = "Mercedes",        Points = 383 },
            new() { Position = 3, TeamName = "Ferrari",         Points = 343 },
            new() { Position = 4, TeamName = "McLaren",         Points = 378 },
            new() { Position = 5, TeamName = "Aston Martin",    Points = 216 },
        },
    };

    private static PastRaceDetail BuildSauGp() => new()
    {
        EventName = "SAUDI ARABIAN GP", EventDate = DateTime.UtcNow.AddDays(-80), YellowFlagCount = 0,
        Results = new List<RaceResultEntry>
        {
            new() { Position = 1,  DriverName = "M. VERSTAPPEN",  TeamName = "Red Bull Racing", TimeOrDnf = "1:30.112", IsFastestLap = true },
            new() { Position = 2,  DriverName = "C. LECLERC",     TeamName = "Ferrari",         TimeOrDnf = "1:30.315" },
            new() { Position = 3,  DriverName = "S. PEREZ",       TeamName = "Red Bull Racing", TimeOrDnf = "1:30.610" },
            new() { Position = 4,  DriverName = "G. RUSSELL",     TeamName = "Mercedes",        TimeOrDnf = "1:30.820" },
            new() { Position = 5,  DriverName = "L. HAMILTON",    TeamName = "Mercedes",        TimeOrDnf = "1:31.040" },
            new() { Position = 6,  DriverName = "F. ALONSO",      TeamName = "Aston Martin",    TimeOrDnf = "1:31.310" },
            new() { Position = 7,  DriverName = "O. PIASTRI",     TeamName = "McLaren",         TimeOrDnf = "1:31.550" },
            new() { Position = 8,  DriverName = "L. NORRIS",      TeamName = "McLaren",         TimeOrDnf = "1:31.780" },
            new() { Position = 9,  DriverName = "C. SAINZ",       TeamName = "Ferrari",         TimeOrDnf = "1:32.110" },
            new() { Position = 10, DriverName = "L. STROLL",      TeamName = "Aston Martin",    TimeOrDnf = "1:32.450" },
            new() { Position = 11, DriverName = "E. OCON",        TeamName = "Alpine",          TimeOrDnf = "1:32.890" },
            new() { Position = 12, DriverName = "P. GASLY",       TeamName = "Alpine",          TimeOrDnf = "1:33.220" },
            new() { Position = 13, DriverName = "Y. TSUNODA",     TeamName = "RB",              TimeOrDnf = "1:33.550" },
            new() { Position = 14, DriverName = "D. RICCIARDO",   TeamName = "RB",              TimeOrDnf = "1:33.990" },
            new() { Position = 15, DriverName = "N. HULKENBERG",  TeamName = "Haas",            TimeOrDnf = "1:34.330" },
            new() { Position = 16, DriverName = "V. BOTTAS",      TeamName = "Sauber",          TimeOrDnf = "1:34.770" },
            new() { Position = 17, DriverName = "Z. GUANYU",      TeamName = "Sauber",          TimeOrDnf = "1:35.110" },
            new() { Position = 18, DriverName = "K. MAGNUSSEN",   TeamName = "Haas",            TimeOrDnf = "1:35.550" },
            new() { Position = 19, DriverName = "A. ALBON",       TeamName = "Williams",        TimeOrDnf = "DNF", IsDnf = true },
        },
        DriverStandings = new()
        {
            new() { Position = 1, DriverName = "M. VERSTAPPEN", TeamName = "Red Bull Racing", Points = 303 },
            new() { Position = 2, DriverName = "L. HAMILTON",   TeamName = "Mercedes",        Points = 260 },
            new() { Position = 3, DriverName = "C. LECLERC",    TeamName = "Ferrari",         Points = 250 },
            new() { Position = 4, DriverName = "S. PEREZ",      TeamName = "Red Bull Racing", Points = 198 },
            new() { Position = 5, DriverName = "G. RUSSELL",    TeamName = "Mercedes",        Points = 185 },
        },
        ConstructorStandings = new()
        {
            new() { Position = 1, TeamName = "Red Bull Racing", Points = 468 },
            new() { Position = 2, TeamName = "Ferrari",         Points = 405 },
            new() { Position = 3, TeamName = "Mercedes",        Points = 395 },
            new() { Position = 4, TeamName = "McLaren",         Points = 370 },
            new() { Position = 5, TeamName = "Aston Martin",    Points = 228 },
        },
    };

    private static PastRaceDetail BuildMiaGp() => new()
    {
        EventName = "MIAMI GP", EventDate = DateTime.UtcNow.AddDays(-66), YellowFlagCount = 1,
        Results = new List<RaceResultEntry>
        {
            new() { Position = 1,  DriverName = "L. HAMILTON",    TeamName = "Mercedes",        TimeOrDnf = "1:28.910", IsFastestLap = true },
            new() { Position = 2,  DriverName = "M. VERSTAPPEN",  TeamName = "Red Bull Racing", TimeOrDnf = "1:29.120" },
            new() { Position = 3,  DriverName = "L. NORRIS",      TeamName = "McLaren",         TimeOrDnf = "1:29.440" },
            new() { Position = 4,  DriverName = "C. LECLERC",     TeamName = "Ferrari",         TimeOrDnf = "1:29.710" },
            new() { Position = 5,  DriverName = "O. PIASTRI",     TeamName = "McLaren",         TimeOrDnf = "1:29.980" },
            new() { Position = 6,  DriverName = "C. SAINZ",       TeamName = "Ferrari",         TimeOrDnf = "1:30.210" },
            new() { Position = 7,  DriverName = "S. PEREZ",       TeamName = "Red Bull Racing", TimeOrDnf = "1:30.550" },
            new() { Position = 8,  DriverName = "G. RUSSELL",     TeamName = "Mercedes",        TimeOrDnf = "1:30.880" },
            new() { Position = 9,  DriverName = "F. ALONSO",      TeamName = "Aston Martin",    TimeOrDnf = "1:31.220" },
            new() { Position = 10, DriverName = "L. STROLL",      TeamName = "Aston Martin",    TimeOrDnf = "1:31.550" },
            new() { Position = 11, DriverName = "E. OCON",        TeamName = "Alpine",          TimeOrDnf = "1:31.990" },
            new() { Position = 12, DriverName = "P. GASLY",       TeamName = "Alpine",          TimeOrDnf = "1:32.340" },
            new() { Position = 13, DriverName = "Y. TSUNODA",     TeamName = "RB",              TimeOrDnf = "1:32.780" },
            new() { Position = 14, DriverName = "D. RICCIARDO",   TeamName = "RB",              TimeOrDnf = "1:33.110" },
            new() { Position = 15, DriverName = "N. HULKENBERG",  TeamName = "Haas",            TimeOrDnf = "1:33.550" },
            new() { Position = 16, DriverName = "K. MAGNUSSEN",   TeamName = "Haas",            TimeOrDnf = "1:34.120" },
            new() { Position = 17, DriverName = "V. BOTTAS",      TeamName = "Sauber",          TimeOrDnf = "1:34.660" },
            new() { Position = 18, DriverName = "Z. GUANYU",      TeamName = "Sauber",          TimeOrDnf = "1:35.110" },
            new() { Position = 19, DriverName = "A. ALBON",       TeamName = "Williams",        TimeOrDnf = "DNF", IsDnf = true },
            new() { Position = 20, DriverName = "L. SARGEANT",    TeamName = "Williams",        TimeOrDnf = "DNF", IsDnf = true },
        },
        DriverStandings = new()
        {
            new() { Position = 1, DriverName = "M. VERSTAPPEN", TeamName = "Red Bull Racing", Points = 318 },
            new() { Position = 2, DriverName = "L. HAMILTON",   TeamName = "Mercedes",        Points = 290 },
            new() { Position = 3, DriverName = "C. LECLERC",    TeamName = "Ferrari",         Points = 268 },
            new() { Position = 4, DriverName = "L. NORRIS",     TeamName = "McLaren",         Points = 215 },
            new() { Position = 5, DriverName = "O. PIASTRI",    TeamName = "McLaren",         Points = 204 },
        },
        ConstructorStandings = new()
        {
            new() { Position = 1, TeamName = "Red Bull Racing", Points = 483 },
            new() { Position = 2, TeamName = "Mercedes",        Points = 428 },
            new() { Position = 3, TeamName = "Ferrari",         Points = 415 },
            new() { Position = 4, TeamName = "McLaren",         Points = 395 },
            new() { Position = 5, TeamName = "Aston Martin",    Points = 248 },
        },
    };

    private static PastRaceDetail BuildGenericDetail(string eventName) => new()
    {
        EventName = eventName, EventDate = DateTime.UtcNow.AddDays(-30), YellowFlagCount = 0,
        Results = new() { new() { Position = 1, DriverName = "P1 DRIVER", TeamName = "P1 Team", TimeOrDnf = "--:--" } },
        DriverStandings = new() { new() { Position = 1, DriverName = "P1 DRIVER", TeamName = "P1 Team", Points = 100 } },
        ConstructorStandings = new() { new() { Position = 1, TeamName = "P1 Team", Points = 100 } },
    };
}
