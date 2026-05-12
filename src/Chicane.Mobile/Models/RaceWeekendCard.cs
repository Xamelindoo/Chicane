namespace Chicane.Mobile.Models;

public class RaceWeekendCard
{
    public string MeetingName { get; init; } = string.Empty;
    public string CircuitName { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public DateTime DateStart { get; init; }
    public DateTime DateEnd { get; init; }
    public string CircuitImageUrl { get; set; } = string.Empty;
    public int MeetingKey { get; init; }
    public string WeatherCondition { get; set; } = string.Empty;
    public string WeatherIcon { get; set; } = string.Empty;
    public int TemperatureC { get; set; }
}
