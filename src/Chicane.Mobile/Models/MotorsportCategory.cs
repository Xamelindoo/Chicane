namespace Chicane.Mobile.Models;

public record MotorsportCategory
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string AccentColor { get; init; } = "#E10600";
    public string Emoji { get; init; } = "🏎️";

    public static readonly MotorsportCategory F1 = new()
    {
        Id = "f1", Name = "F1", AccentColor = "#E10600", Emoji = "🏎️"
    };

    public static readonly MotorsportCategory WEC = new()
    {
        Id = "wec", Name = "WEC", AccentColor = "#004225", Emoji = "🏁"
    };

    public static readonly MotorsportCategory WRC = new()
    {
        Id = "wrc", Name = "WRC", AccentColor = "#30839f", Emoji = "🏜️"
    };

    public static readonly MotorsportCategory MotoGP = new()
    {
        Id = "motogp", Name = "MotoGP", AccentColor = "#FF6600", Emoji = "🏍️"
    };

    public static readonly List<MotorsportCategory> All = new() { F1, WEC, WRC, MotoGP };
}
