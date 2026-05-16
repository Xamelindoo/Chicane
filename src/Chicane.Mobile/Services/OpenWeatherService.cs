using System.Diagnostics;
using System.Text.Json;
using Chicane.Mobile.Models;
using Chicane.Mobile.Models.Api;

namespace Chicane.Mobile.Services;

public class OpenWeatherService : IWeatherService
{
    private readonly HttpClient _http;
    private const string BaseUrl = "https://api.openweathermap.org/data/2.5/forecast";

    public OpenWeatherService(HttpClient http)
    {
        _http = http;
    }

    public async Task<WeatherForecast?> GetForecastAsync(string city, string country, DateTime eventDate)
    {
        try
        {
            var utcDate = eventDate.Kind == DateTimeKind.Utc ? eventDate : eventDate.ToUniversalTime();
            var daysUntil = (utcDate - DateTime.UtcNow).TotalDays;
            if (daysUntil > 7) return null;

            var query = $"{city},{country}";
            var url = $"{BaseUrl}?q={Uri.EscapeDataString(query)}&appid={Env.Get("OPENWEATHER_API_KEY")}&units=metric";

            Debug.WriteLine($"[Weather] GET {url[..Math.Min(url.IndexOf("&appid="), url.Length)]}...");
            var json = await _http.GetStringAsync(url);
            var response = JsonSerializer.Deserialize<OpenWeatherResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (response?.List.Count == 0) return null;

            return OpenWeatherMapper.ToForecast(response!, eventDate);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[Weather] OpenWeather failed: {ex.Message}");
            return null;
        }
    }
}
