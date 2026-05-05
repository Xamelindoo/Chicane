using Chicane.Mobile.Models;
using Chicane.Mobile.Models.Api;

namespace Chicane.Mobile.Services;

public static class ErgastMapper
{
    public static List<StandingsEntry> ToDriverStandings(ErgastResponse response)
    {
        var list = response.MrData.StandingsTable?.StandingsLists.FirstOrDefault();
        if (list?.DriverStandings is null) return new();

        return list.DriverStandings.Select(d => new StandingsEntry
        {
            Position = int.Parse(d.Position),
            DriverName = $"{d.Driver.GivenName[0]}. {d.Driver.FamilyName.ToUpper()}",
            TeamName = d.Constructors.FirstOrDefault()?.Name ?? "Unknown",
            Points = int.Parse(d.Points),
        }).ToList();
    }

    public static List<ConstructorStandingsEntry> ToConstructorStandings(ErgastResponse response)
    {
        var list = response.MrData.StandingsTable?.StandingsLists.FirstOrDefault();
        if (list?.ConstructorStandings is null) return new();

        return list.ConstructorStandings.Select(c => new ConstructorStandingsEntry
        {
            Position = int.Parse(c.Position),
            TeamName = c.Constructor.Name,
            Points = int.Parse(c.Points),
        }).ToList();
    }

    public static RaceEvent ToRaceEvent(ErgastResponse response)
    {
        var race = response.MrData.RaceTable?.Races.FirstOrDefault();
        if (race is null) return new RaceEvent();

        return new RaceEvent
        {
            EventName = race.RaceName.ToUpper().Replace(" GRAND PRIX", " GP"),
            ScheduledDateTime = ParseDate(race.Date),
            Circuit = new CircuitInfo
            {
                Name = race.Circuit.CircuitName,
                Country = race.Circuit.Location.Country,
            },
            Weather = new WeatherInfo
            {
                Condition = "N/A",
                TemperatureC = 20,
                IconGlyph = "\u2600"
            }
        };
    }

    public static List<RaceScheduleEntry> ToSchedule(ErgastResponse response)
    {
        var races = response.MrData.RaceTable?.Races;
        if (races is null) return new();

        var now = DateTime.UtcNow;
        return races.Select(r =>
        {
            var date = ParseDate(r.Date);
            return new RaceScheduleEntry
            {
                EventName = r.RaceName.ToUpper().Replace(" GRAND PRIX", " GP"),
                CircuitName = r.Circuit.CircuitName,
                Country = r.Circuit.Location.Country,
                EventDate = date,
                WeatherCondition = "N/A",
                WeatherIcon = "\u2600",
                TemperatureC = 20,
                IsCompleted = date < now,
            };
        }).ToList();
    }

    public static List<RaceResultEntry> ToRaceResults(ErgastResponse response)
    {
        var race = response.MrData.RaceTable?.Races.FirstOrDefault();
        if (race?.Results is null) return new();

        var results = race.Results.Select(r => new RaceResultEntry
        {
            Position = int.Parse(r.Position),
            DriverName = $"{r.Driver.GivenName[0]}. {r.Driver.FamilyName.ToUpper()}",
            TeamName = r.Constructor.Name,
            TimeOrDnf = r.Status == "Finished" ? r.Time?.Value ?? "--:--.---" : "DNF",
            IsDnf = r.Status != "Finished",
            IsFastestLap = r.FastestLap?.Rank == "1",
        }).ToList();

        var dnfs = results.Where(r => r.IsDnf).OrderBy(r => r.DriverName).ToList();
        var nonDnfs = results.Where(r => !r.IsDnf).OrderBy(r => r.Position).ToList();
        return nonDnfs.Concat(dnfs).ToList();
    }

    private static DateTime ParseDate(string date) =>
        DateTime.TryParse(date, out var result) ? result : DateTime.UtcNow;
}
