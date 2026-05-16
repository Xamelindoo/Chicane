using System.Text.Json;
using Chicane.Mobile.Models;

namespace Chicane.Mobile.Services;

public class OpenWeatherService : IWeatherService
{
    private readonly HttpClient _http;

    public OpenWeatherService(HttpClient http) => _http = http;

    public async Task<WeatherForecast?> GetForecastAsync(string city, string country, DateTime eventDate)
    {
        try
        {
            var dateStr = eventDate.ToString("yyyy-MM-dd");
            var json = await _http.GetStringAsync($"/api/weather?city={Uri.EscapeDataString(city)}&country={Uri.EscapeDataString(country)}&date={dateStr}");
            var doc = JsonDocument.Parse(json).RootElement;
            return new WeatherForecast
            {
                Condition = doc.TryGetProperty("condition", out var c) ? c.GetString() ?? "" : "",
                TemperatureC = doc.TryGetProperty("temperatureC", out var t) ? (int)t.GetDouble() : 20,
                IconGlyph = doc.TryGetProperty("iconGlyph", out var i) ? i.GetString() ?? "\u2600" : "\u2600",
            };
        }
        catch { return null; }
    }
}
