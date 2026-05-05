namespace Chicane.Mobile.Models;

public class TeamSeasonStats
{
    public int Championships { get; init; }
    public int RaceWins { get; init; }
    public int Podiums { get; init; }
    public int PolePositions { get; init; }
    public int FastestLaps { get; init; }
    public string? FirstSeason { get; init; }
    public string? HomeRace { get; init; }
}
