namespace Chicane.Mobile.Models;

public record PastRaceDetail
{
    public string EventName { get; init; } = string.Empty;
    public List<RaceResultEntry> Results { get; init; } = new();
    public int YellowFlagCount { get; init; }
    public List<StandingsEntry> DriverStandings { get; init; } = new();
    public List<ConstructorStandingsEntry> ConstructorStandings { get; init; } = new();
}
