namespace Chicane.Mobile.Models;

public class DriverStandingsEntry
{
    public int Position { get; init; }
    public string DriverName { get; init; } = string.Empty;
    public string TeamName { get; init; } = string.Empty;
    public int Points { get; init; }
    public string BorderColor { get; init; } = "#333333";
    public int DriverNumber { get; init; }
    public int PositionStart { get; init; }
    public string TeamLogo { get; init; } = string.Empty;
    public string HeadshotUrl { get; init; } = string.Empty;
    public int PositionChange => PositionStart > 0 ? PositionStart - Position : 0;

    public string PositionArrow => PositionChange switch
    {
        > 0 => $"▲",
        < 0 => $"▼",
        _ => "-"
    };

    public string PositionArrowColor => PositionChange switch
    {
        > 0 => "#10B981",
        < 0 => "#EF4444",
        _ => "#9CA3AF"
    };
}
