using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Chicane.Backend.Services;
using Chicane.Backend;

namespace Chicane.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly OpenWeatherService _weather;
    private readonly IDistributedCache _cache;

    public WeatherController(OpenWeatherService weather, IDistributedCache cache) { _weather = weather; _cache = cache; }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string city, [FromQuery] string country,
        [FromQuery] string? date = null)
    {
        var cacheKey = $"api_weather_{city}_{country}_{date}";
        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached is not null) return Content(cached, "application/json");

        DateTime eventDate;
        if (!DateTime.TryParse(date, out eventDate))
            eventDate = DateTime.UtcNow.AddDays(7);

        var forecast = await _weather.GetForecastAsync(city, country, eventDate);
        var result = JsonSerializer.Serialize(forecast, JsonConfig.CamelCaseOpts);

        await _cache.SetStringAsync(cacheKey, result, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        });

        return Content(result, "application/json");
    }
}
