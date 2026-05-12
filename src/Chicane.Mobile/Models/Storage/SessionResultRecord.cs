using SQLite;

namespace Chicane.Mobile.Models.Storage;

[Table("SessionResults")]
public class SessionResultRecord
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int SessionKey { get; set; }
    public int Position { get; set; }
    public string DriverName { get; set; } = string.Empty;
    public string TeamName { get; set; } = string.Empty;
    public string HeadshotUrl { get; set; } = string.Empty;
    public string TimeOrDnf { get; set; } = string.Empty;
    public string? GapToLeader { get; set; }
    public int NumberOfLaps { get; set; }
    public bool IsFastestLap { get; set; }
    public bool IsDnf { get; set; }
}
