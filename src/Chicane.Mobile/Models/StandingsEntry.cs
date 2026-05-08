namespace Chicane.Mobile.Models;

public class StandingsEntry
{
    public int Position { get; init; }
    public string DriverName { get; init; } = string.Empty;
    public string TeamName { get; init; } = string.Empty;
    public int Points { get; init; }
    public int DriverNumber { get; init; }
    public int PositionStart { get; init; }
    public string HeadshotUrl { get; init; } = string.Empty;
}
