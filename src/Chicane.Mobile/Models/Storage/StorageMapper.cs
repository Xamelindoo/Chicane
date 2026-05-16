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
        DriverNumber = entry.DriverNumber,
        PositionStart = entry.PositionStart,
        HeadshotUrl = entry.HeadshotUrl,
    };

    public static StandingsEntry ToDomain(StandingsRecord record) => new()
    {
        Position = record.Position,
        DriverName = record.DriverName,
        TeamName = record.TeamName,
        Points = record.Points,
        DriverNumber = record.DriverNumber,
        PositionStart = record.PositionStart,
        HeadshotUrl = record.HeadshotUrl,
    };

    // Constructor standings
    public static ConstructorStandingsRecord ToRecord(ConstructorStandingsEntry entry, string categoryId) => new()
    {
        CategoryId = categoryId,
        Position = entry.Position,
        TeamName = entry.TeamName,
        Points = entry.Points,
        TeamLogo = entry.TeamLogo,
    };

    public static ConstructorStandingsEntry ToDomain(ConstructorStandingsRecord record) => new()
    {
        Position = record.Position,
        TeamName = record.TeamName,
        Points = record.Points,
        TeamLogo = record.TeamLogo,
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
        SessionKey = entry.SessionKey,
        Top3Result = entry.Top3Result,
        MeetingName = entry.MeetingName,
        MeetingKey = entry.MeetingKey,
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
        SessionKey = record.SessionKey,
        Top3Result = record.Top3Result,
        MeetingName = record.MeetingName,
        MeetingKey = record.MeetingKey,
    };

    // Session Results
    public static SessionResultRecord ToRecord(RaceResultEntry entry, int sessionKey) => new()
    {
        SessionKey = sessionKey,
        Position = entry.Position,
        DriverName = entry.DriverName,
        TeamName = entry.TeamName,
        HeadshotUrl = entry.HeadshotUrl,
        TimeOrDnf = entry.TimeOrDnf,
        GapToLeader = entry.GapToLeader,
        NumberOfLaps = entry.NumberOfLaps,
        IsFastestLap = entry.IsFastestLap,
        IsDnf = entry.IsDnf,
        DriverAcronym = entry.DriverAcronym,
    };

    public static RaceResultEntry ToDomain(SessionResultRecord record) => new()
    {
        Position = record.Position,
        DriverName = record.DriverName,
        TeamName = record.TeamName,
        HeadshotUrl = record.HeadshotUrl,
        TimeOrDnf = record.TimeOrDnf,
        GapToLeader = record.GapToLeader,
        NumberOfLaps = record.NumberOfLaps,
        IsFastestLap = record.IsFastestLap,
        IsDnf = record.IsDnf,
        DriverAcronym = record.DriverAcronym,
    };

    // Race Weekends
    public static RaceWeekendRecord ToRecord(RaceWeekendCard card) => new()
    {
        MeetingKey = card.MeetingKey,
        MeetingName = card.MeetingName,
        CircuitName = card.CircuitName,
        Country = card.Country,
        DateStart = card.DateStart,
        DateEnd = card.DateEnd,
        CircuitImageUrl = card.CircuitImageUrl,
        WeatherCondition = card.WeatherCondition,
        WeatherIcon = card.WeatherIcon,
        TemperatureC = card.TemperatureC,
    };

    public static RaceWeekendCard ToDomain(RaceWeekendRecord record) => new()
    {
        MeetingKey = record.MeetingKey,
        MeetingName = record.MeetingName,
        CircuitName = record.CircuitName,
        Country = record.Country,
        DateStart = record.DateStart,
        DateEnd = record.DateEnd,
        CircuitImageUrl = record.CircuitImageUrl,
        WeatherCondition = record.WeatherCondition,
        WeatherIcon = record.WeatherIcon,
        TemperatureC = (int)record.TemperatureC,
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
        TeamLogo = entry.TeamLogo,
    };

    public static TeamEntry ToDomain(TeamRecord record) => new()
    {
        TeamName = record.TeamName,
        Position = record.Position,
        Points = record.Points,
        BorderColor = record.BorderColor,
        GradientColor = record.GradientColor,
        TeamLogo = record.TeamLogo,
        Drivers = new List<DriverEntry>
        {
            new() { Name = record.Driver1Name, Points = record.Driver1Points },
            new() { Name = record.Driver2Name, Points = record.Driver2Points },
        },
    };
}
