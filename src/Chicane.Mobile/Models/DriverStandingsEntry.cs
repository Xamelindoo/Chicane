namespace Chicane.Mobile.Models;

public class DriverStandingsEntry
{
    public int Position { get; init; }
    public string DriverName { get; init; } = string.Empty;
    public string TeamName { get; init; } = string.Empty;
    public int Points { get; init; }
    public string BorderColor { get; init; } = "#333333";
}
