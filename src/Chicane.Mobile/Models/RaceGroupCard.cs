namespace Chicane.Mobile.Models;

public class RaceGroupCard
{
    public string MeetingName { get; init; } = string.Empty;
    public int MeetingKey { get; init; }
    public DateTime EventDate { get; init; }
    public string CircuitName { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string Driver1Name { get; init; } = string.Empty;
    public string Driver1Code { get; init; } = string.Empty;
    public int Driver1Position { get; init; }
    public string Driver2Name { get; init; } = string.Empty;
    public string Driver2Code { get; init; } = string.Empty;
    public int Driver2Position { get; init; }
}
