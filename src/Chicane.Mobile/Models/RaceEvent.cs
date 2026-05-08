namespace Chicane.Mobile.Models;

public class RaceEvent
{
    public string EventName { get; init; } = string.Empty;
    public string MeetingName { get; init; } = string.Empty;
    public string CircuitImageUrl { get; init; } = string.Empty;
    public CircuitInfo Circuit { get; init; } = new();
    public DateTime ScheduledDateTime { get; init; }
    public WeatherInfo Weather { get; init; } = new();
    public int SessionKey { get; init; }
}
