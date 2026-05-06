namespace Chicane.Mobile.Models;

public class WeatherForecast
{
    public string Condition { get; init; } = string.Empty;
    public int TemperatureC { get; init; }
    public double WindSpeed { get; init; }
    public int Humidity { get; init; }
    public int Pressure { get; init; }
    public string IconGlyph { get; init; } = "\u2600";
    public bool IsApproximate { get; init; }
}
