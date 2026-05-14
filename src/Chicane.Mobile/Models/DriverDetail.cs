namespace Chicane.Mobile.Models;

public record DriverDetail
{
    public int DriverNumber { get; init; }
    public string BroadcastName { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string NameAcronym { get; init; } = string.Empty;
    public string TeamName { get; init; } = string.Empty;
    public string TeamColour { get; init; } = "#333333";
    public string HeadshotUrl { get; init; } = string.Empty;
    public int Points { get; init; }
    public int Position { get; init; }
    public List<DriverRaceResult> RaceResults { get; init; } = new();
}

public record DriverRaceResult
{
    public string SessionName { get; init; } = string.Empty;
    public string CircuitName { get; init; } = string.Empty;
    public string? MeetingName { get; init; }
    public int SessionKey { get; init; }
    public int Position { get; init; }
    public double Points { get; init; }
    public DateTime EventDate { get; init; }
}
