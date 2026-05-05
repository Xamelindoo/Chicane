using SQLite;

namespace Chicane.Mobile.Models.Storage;

[Table("DriverStandings")]
public class StandingsRecord
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public string CategoryId { get; set; } = string.Empty;

    public int Position { get; set; }
    public string DriverName { get; set; } = string.Empty;
    public string TeamName { get; set; } = string.Empty;
    public int Points { get; set; }
}
