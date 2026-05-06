using System.Net.Http.Json;
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
            var daysUntil = (eventDate - DateTime.UtcNow).TotalDays;
            if (daysUntil > 5) return null; // 5-day forecast max

            var query = $"{city},{country}";
            var url = $"{BaseUrl}?q={Uri.EscapeDataString(query)}&appid={SecretsConfig.OpenWeatherApiKey}&units=metric";

            var json = await _http.GetStringAsync(url);
            var response = JsonSerializer.Deserialize<OpenWeatherResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (response?.List.Count == 0) return null;

            return OpenWeatherMapper.ToForecast(response!, eventDate);
        }
        catch
        {
            return null;
        }
    }
}
