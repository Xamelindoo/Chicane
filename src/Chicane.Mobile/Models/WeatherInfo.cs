namespace Chicane.Mobile.Models;

public class WeatherInfo
{
    public string Condition { get; init; } = string.Empty;
    public int TemperatureC { get; init; }
    public string IconGlyph { get; init; } = string.Empty;
}
