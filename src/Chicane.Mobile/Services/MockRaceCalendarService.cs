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

    public Task<List<RaceScheduleEntry>> GetPastRacesAsync()
    {
        var races = new List<RaceScheduleEntry>
        {
            new() { EventName = "AUSTRALIAN GP", CircuitName = "Albert Park Circuit", Country = "Australia", EventDate = DateTime.UtcNow.AddDays(-95), WeatherCondition = "Sunny", WeatherIcon = "\u2600", TemperatureC = 26, Result = "1. VER 2. HAM 3. LEC", IsCompleted = true },
            new() { EventName = "SAUDI ARABIAN GP", CircuitName = "Jeddah Corniche Circuit", Country = "Saudi Arabia", EventDate = DateTime.UtcNow.AddDays(-80), WeatherCondition = "Clear", WeatherIcon = "\u2600", TemperatureC = 29, Result = "1. VER 2. LEC 3. PER", IsCompleted = true },
            new() { EventName = "MIAMI GP", CircuitName = "Miami International Autodrome", Country = "USA", EventDate = DateTime.UtcNow.AddDays(-66), WeatherCondition = "Sunny", WeatherIcon = "\u2600", TemperatureC = 31, Result = "1. HAM 2. VER 3. NOR", IsCompleted = true },
            new() { EventName = "IMOLA GP", CircuitName = "Autodromo Enzo e Dino Ferrari", Country = "Italy", EventDate = DateTime.UtcNow.AddDays(-52), WeatherCondition = "Cloudy", WeatherIcon = "\u2601", TemperatureC = 17, Result = "1. VER 2. PIA 3. LEC", IsCompleted = true },
            new() { EventName = "BARCELONA GP", CircuitName = "Circuit de Barcelona-Catalunya", Country = "Spain", EventDate = DateTime.UtcNow.AddDays(-38), WeatherCondition = "Sunny", WeatherIcon = "\u2600", TemperatureC = 25, Result = "1. VER 2. HAM 3. RUS", IsCompleted = true },
            new() { EventName = "MONTREAL GP", CircuitName = "Circuit Gilles Villeneuve", Country = "Canada", EventDate = DateTime.UtcNow.AddDays(-24), WeatherCondition = "Rain", WeatherIcon = "\u26C8", TemperatureC = 15, Result = "1. HAM 2. VER 3. ALO", IsCompleted = true },
            new() { EventName = "RED BULL RING GP", CircuitName = "Red Bull Ring", Country = "Austria", EventDate = DateTime.UtcNow.AddDays(-10), WeatherCondition = "Sunny", WeatherIcon = "\u2600", TemperatureC = 27, Result = "1. VER 2. NOR 3. SAI", IsCompleted = true }
        };

        return Task.FromResult(races);
    }
}
