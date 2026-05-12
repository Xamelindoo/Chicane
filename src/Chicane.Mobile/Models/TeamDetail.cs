namespace Chicane.Mobile.Models;

public record TeamDetail
{
    public string TeamName { get; init; } = string.Empty;
    public string TeamLogo { get; init; } = string.Empty;
    public string TeamLogoTransparent { get; init; } = string.Empty;
    public string BorderColor { get; init; } = "#333333";
    public int TotalPoints { get; init; }
    public string Emoji { get; init; } = "🏎️";
    public string GradientColor { get; init; } = "#0A0A0A";
    public List<DriverEntry> Drivers { get; init; } = new();
    public List<TeamRaceResult> RaceResults { get; init; } = new();
    public TeamSeasonStats? SeasonStats { get; init; }
    public TeamProfile? Profile { get; init; }
}
