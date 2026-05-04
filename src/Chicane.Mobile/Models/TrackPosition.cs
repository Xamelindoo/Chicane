namespace Chicane.Mobile.Models;

public class TrackPosition
{
    public int Position { get; init; }
    public string DriverName { get; init; } = string.Empty;
    public int Sector { get; init; }
    public string LastTime { get; init; } = string.Empty;
}
