using Chicane.Mobile.Models;
using Chicane.Mobile.Models.Api;

namespace Chicane.Mobile.Services;

public static class OpenWeatherMapper
{
    public static WeatherForecast ToForecast(OpenWeatherResponse response, DateTime targetDate)
    {
        var closest = response.List
            .OrderBy(e => Math.Abs((DateTimeOffset.FromUnixTimeSeconds(e.Dt).UtcDateTime - targetDate).TotalHours))
            .First();

        var condition = closest.Weather.FirstOrDefault();
        return new WeatherForecast
        {
            Condition = condition?.Description ?? "N/A",
            TemperatureC = (int)Math.Round(closest.Main.Temp),
            WindSpeed = closest.Wind.Speed,
            Humidity = closest.Main.Humidity,
            Pressure = closest.Main.Pressure,
            IconGlyph = MapIcon(condition?.Icon ?? "01d"),
            IsApproximate = false,
        };
    }

    private static string MapIcon(string owIcon) => owIcon switch
    {
        "01d" or "01n" => "\u2600",  // clear
        "02d" or "02n" => "\u26C5",  // few clouds
        "03d" or "03n" => "\u2601",  // scattered
        "04d" or "04n" => "\u2601",  // broken
        "09d" or "09n" => "\u2614",  // shower rain
        "10d" or "10n" => "\u26C8",  // rain
        "11d" or "11n" => "\u26C8",  // thunderstorm
        "13d" or "13n" => "\u2744",  // snow
        "50d" or "50n" => "\uD83C\uDF2B", // mist
        _ => "\u2600",
    };
}
