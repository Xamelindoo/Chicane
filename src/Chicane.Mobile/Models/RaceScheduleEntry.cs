namespace Chicane.Mobile.Models;

public class RaceScheduleEntry
{
    public string EventName { get; init; } = string.Empty;
    public string MeetingName { get; init; } = string.Empty;
    public string CircuitName { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public DateTime EventDate { get; init; }
    public string WeatherCondition { get; init; } = string.Empty;
    public string WeatherIcon { get; init; } = string.Empty;
    public int TemperatureC { get; init; }
    public string? Result { get; set; }
    public bool IsCompleted { get; init; }
    public int SessionKey { get; init; }
    public string? Top3Result { get; set; }
    public int MeetingKey { get; init; }
}
