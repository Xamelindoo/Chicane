using Chicane.Mobile.Models;

namespace Chicane.Mobile.Models.Storage;

public static class StorageMapper
{
    // Standings
    public static StandingsRecord ToRecord(StandingsEntry entry, string categoryId) => new()
    {
        CategoryId = categoryId,
        Position = entry.Position,
        DriverName = entry.DriverName,
        TeamName = entry.TeamName,
        Points = entry.Points,
    };

    public static StandingsEntry ToDomain(StandingsRecord record) => new()
    {
        Position = record.Position,
        DriverName = record.DriverName,
        TeamName = record.TeamName,
        Points = record.Points,
    };

    // Constructor standings
    public static ConstructorStandingsRecord ToRecord(ConstructorStandingsEntry entry, string categoryId) => new()
    {
        CategoryId = categoryId,
        Position = entry.Position,
        TeamName = entry.TeamName,
        Points = entry.Points,
    };

    public static ConstructorStandingsEntry ToDomain(ConstructorStandingsRecord record) => new()
    {
        Position = record.Position,
        TeamName = record.TeamName,
        Points = record.Points,
    };

    // Schedule
    public static RaceScheduleRecord ToRecord(RaceScheduleEntry entry, string categoryId) => new()
    {
        EventName = entry.EventName,
        CategoryId = categoryId,
        CircuitName = entry.CircuitName,
        Country = entry.Country,
        EventDate = entry.EventDate,
        WeatherCondition = entry.WeatherCondition,
        WeatherIcon = entry.WeatherIcon,
        TemperatureC = entry.TemperatureC,
        IsCompleted = entry.IsCompleted,
        Result = entry.Result,
    };

    public static RaceScheduleEntry ToDomain(RaceScheduleRecord record) => new()
    {
        EventName = record.EventName,
        CircuitName = record.CircuitName,
        Country = record.Country,
        EventDate = record.EventDate,
        WeatherCondition = record.WeatherCondition,
        WeatherIcon = record.WeatherIcon,
        TemperatureC = record.TemperatureC,
        IsCompleted = record.IsCompleted,
        Result = record.Result,
    };

    // Teams
    public static TeamRecord ToRecord(TeamEntry entry, string categoryId) => new()
    {
        TeamName = entry.TeamName,
        CategoryId = categoryId,
        Position = entry.Position,
        Points = entry.Points,
        BorderColor = entry.BorderColor,
        GradientColor = entry.GradientColor,
        Driver1Name = entry.Drivers.Count > 0 ? entry.Drivers[0].Name : "",
        Driver1Points = entry.Drivers.Count > 0 ? entry.Drivers[0].Points : 0,
        Driver2Name = entry.Drivers.Count > 1 ? entry.Drivers[1].Name : "",
        Driver2Points = entry.Drivers.Count > 1 ? entry.Drivers[1].Points : 0,
    };

    public static TeamEntry ToDomain(TeamRecord record) => new()
    {
        TeamName = record.TeamName,
        Position = record.Position,
        Points = record.Points,
        BorderColor = record.BorderColor,
        GradientColor = record.GradientColor,
        Drivers = new List<DriverEntry>
        {
            new() { Name = record.Driver1Name, Points = record.Driver1Points },
            new() { Name = record.Driver2Name, Points = record.Driver2Points },
        },
    };
}
