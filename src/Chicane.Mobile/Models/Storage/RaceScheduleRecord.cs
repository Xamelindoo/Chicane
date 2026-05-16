using SQLite;

namespace Chicane.Mobile.Models.Storage;

[Table("RaceSchedule")]
public class RaceScheduleRecord
{
    [PrimaryKey]
    public int SessionKey { get; set; }

    [Indexed]
    public string CategoryId { get; set; } = string.Empty;

    public string EventName { get; set; } = string.Empty;

    public string CircuitName { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string WeatherCondition { get; set; } = string.Empty;
    public string WeatherIcon { get; set; } = string.Empty;
    public int TemperatureC { get; set; }
    public bool IsCompleted { get; set; }
    public string? Result { get; set; }
    public string? Top3Result { get; set; }
    public string MeetingName { get; set; } = string.Empty;
    public int MeetingKey { get; set; }
}
