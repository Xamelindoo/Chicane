namespace Chicane.Mobile.Models;

public class LiveDriverRow
{
    public int Position { get; init; }
    public int DriverNumber { get; init; }
    public string DriverCode { get; init; } = string.Empty;
    public string DriverDisplay { get; init; } = string.Empty;
    public string TeamName { get; init; } = string.Empty;
    public string TeamColour { get; init; } = "#333333";
    public string GapDisplay { get; init; } = string.Empty;
    public string IntervalDisplay { get; init; } = string.Empty;
    public string LastLapTime { get; init; } = string.Empty;
    public string HeadshotUrl { get; init; } = string.Empty;
    public string TyreCompound { get; init; } = string.Empty;

    // Telemetry (populated on tap)
    public int Speed { get; set; }
    public int Rpm { get; set; }
    public int Gear { get; set; }
    public int Throttle { get; set; }
    public int Brake { get; set; }
    public string DrsStatus { get; set; } = string.Empty;
    public string LastPit { get; set; } = string.Empty;

    public bool IsExpanded { get; set; }
}
