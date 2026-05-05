namespace Chicane.Mobile.Models;

public class TeamEntry
{
    public int Position { get; init; }
    public string TeamName { get; init; } = string.Empty;
    public int Points { get; init; }
    public List<string> Drivers { get; init; } = new();
    public string BorderColor { get; init; } = "#333333";
}
