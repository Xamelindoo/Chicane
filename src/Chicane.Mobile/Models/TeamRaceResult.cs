namespace Chicane.Mobile.Models;

public class TeamRaceResult
{
    public string EventName { get; init; } = string.Empty;
    public DateTime EventDate { get; init; }
    public string Driver1Name { get; init; } = string.Empty;
    public int Driver1Points { get; init; }
    public int Driver1Position { get; init; }
    public string? Driver1BestLap { get; init; }
    public string Driver2Name { get; init; } = string.Empty;
    public int Driver2Points { get; init; }
    public int Driver2Position { get; init; }
    public string? Driver2BestLap { get; init; }
    public int TotalPoints { get; init; }
}
