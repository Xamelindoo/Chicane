using System.Diagnostics;
using System.Text.Json;
using Chicane.Backend.Models.Api;

namespace Chicane.Backend.Services;

public class OpenWeatherService
{
    private readonly HttpClient _http;
    private readonly string? _apiKey;

    public OpenWeatherService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _apiKey = config["OpenWeather:ApiKey"];
    }

    public async Task<Dictionary<string, object?>> GetForecastAsync(string city, string country, DateTime eventDate)
    {
        if (string.IsNullOrEmpty(_apiKey)) return new() { ["condition"] = "No API key" };

        try
        {
            var utcDate = eventDate.Kind == DateTimeKind.Utc ? eventDate : eventDate.ToUniversalTime();
            var daysUntil = (utcDate - DateTime.UtcNow).TotalDays;
            if (daysUntil > 7) return new() { ["condition"] = "Check later" };

            var query = $"{city},{country}";
            var url = $"https://api.openweathermap.org/data/2.5/forecast?q={Uri.EscapeDataString(query)}&appid={_apiKey}&units=metric";
            var json = await _http.GetStringAsync(url);
            var response = JsonSerializer.Deserialize<OpenWeatherResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (response?.List.Count == 0) return new() { ["condition"] = "No data" };

            var target = response!.List
                .Where(f => DateTimeOffset.FromUnixTimeSeconds(f.Dt).DateTime <= eventDate.AddDays(1))
                .MinBy(f => Math.Abs((DateTimeOffset.FromUnixTimeSeconds(f.Dt).DateTime - eventDate).TotalHours));

            if (target is null) return new() { ["condition"] = "No forecast" };

            return new()
            {
                ["condition"] = target.Weather.FirstOrDefault()?.Description ?? "N/A",
                ["temperatureC"] = (int)Math.Round(target.Main.Temp),
                ["iconGlyph"] = WeatherIcon(target.Weather.FirstOrDefault()?.Icon ?? ""),
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[Weather] Failed: {ex.Message}");
            return new() { ["condition"] = "Error" };
        }
    }

    private static string WeatherIcon(string icon) => icon switch
    {
        "01d" or "01n" => "\u2600",
        "02d" or "02n" => "\u26C5",
        "03d" or "03n" or "04d" or "04n" => "\u2601",
        "09d" or "09n" or "10d" or "10n" => "\U0001F327",
        "11d" or "11n" => "\u26C8",
        "13d" or "13n" => "\U0001F328",
        "50d" or "50n" => "\U0001F32B",
        _ => "\u2600"
    };
}
