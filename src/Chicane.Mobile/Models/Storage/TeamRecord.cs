using SQLite;

namespace Chicane.Mobile.Models.Storage;

[Table("Teams")]
public class TeamRecord
{
    [PrimaryKey]
    public string TeamName { get; set; } = string.Empty;

    [Indexed]
    public string CategoryId { get; set; } = string.Empty;

    public int Position { get; set; }
    public int Points { get; set; }
    public string BorderColor { get; set; } = string.Empty;
    public string GradientColor { get; set; } = string.Empty;
    public string Driver1Name { get; set; } = string.Empty;
    public int Driver1Points { get; set; }
    public string Driver2Name { get; set; } = string.Empty;
    public int Driver2Points { get; set; }
    public string TeamLogo { get; set; } = string.Empty;
}
