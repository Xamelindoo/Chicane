namespace Chicane.Mobile.Models;

public class RaceResultEntry
{
    public int Position { get; init; }
    public string DriverName { get; init; } = string.Empty;
    public string TeamName { get; init; } = string.Empty;
    public string TimeOrDnf { get; init; } = string.Empty;
    public bool IsFastestLap { get; init; }
    public bool IsDnf { get; init; }
    public bool IsDns { get; init; }
    public bool IsDsq { get; init; }
    public string? GapToLeader { get; set; }
    public int NumberOfLaps { get; init; }
}
