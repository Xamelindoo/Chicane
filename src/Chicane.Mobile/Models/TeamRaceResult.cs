namespace Chicane.Mobile.Models;

public class TeamRaceResult
{
    public string EventName { get; init; } = string.Empty;
    public string SessionName { get; init; } = string.Empty;
    public int SessionKey { get; init; }
    public int MeetingKey { get; init; }
    public string CircuitName { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public DateTime EventDate { get; init; }
    public string Driver1Name { get; init; } = string.Empty;
    public string Driver1Code { get; init; } = string.Empty;
    public int Driver1Position { get; init; }
    public string? Driver1BestLap { get; init; }
    public string Driver2Name { get; init; } = string.Empty;
    public string Driver2Code { get; init; } = string.Empty;
    public int Driver2Position { get; init; }
    public string? Driver2BestLap { get; init; }

    public bool HasDriverData => !string.IsNullOrEmpty(Driver1Name);
}
