namespace Chicane.Mobile.Models;

public class LiveSessionData
{
    public string SessionName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public int CurrentLap { get; init; }
    public int TotalLaps { get; init; }
    public List<DriverGap> PositionBoard { get; init; } = new();
}
