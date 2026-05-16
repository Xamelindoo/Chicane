using SQLite;

namespace Chicane.Mobile.Models.Storage;

[Table("RaceWeekends")]
public class RaceWeekendRecord
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int MeetingKey { get; set; }
    public string MeetingName { get; set; } = string.Empty;
    public string CircuitName { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
    public string CircuitImageUrl { get; set; } = string.Empty;
    public string WeatherCondition { get; set; } = string.Empty;
    public string WeatherIcon { get; set; } = string.Empty;
    public double TemperatureC { get; set; }
}
