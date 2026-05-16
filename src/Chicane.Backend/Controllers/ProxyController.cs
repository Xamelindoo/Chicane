using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Chicane.Backend.Services;

namespace Chicane.Backend.Controllers;

[ApiController]
[Route("api/proxy")]
public class ProxyController : ControllerBase
{
    private readonly OpenF1Service _f1;
    private readonly IDistributedCache _cache;

    public ProxyController(OpenF1Service f1, IDistributedCache cache) { _f1 = f1; _cache = cache; }

    [HttpGet("{**path}")]
    public async Task<IActionResult> Get(string path)
    {
        var queryString = Request.QueryString.Value ?? "";
        var openF1Path = $"{path}{queryString}";

        var cached = await _cache.GetStringAsync(openF1Path);
        if (cached is not null) return Content(cached, "application/json");

        var raw = await _f1.GetRawStringAsync(openF1Path);
        if (raw is null) return StatusCode(502, "{\"error\":\"upstream_unavailable\"}");

        var ttl = GetTtl(path);
        await _cache.SetStringAsync(openF1Path, raw, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl
        });

        return Content(raw, "application/json");
    }

    private static TimeSpan GetTtl(string path)
    {
        var lower = path.ToLowerInvariant();
        if (lower.Contains("position") || lower.Contains("intervals") || lower.Contains("car_data") ||
            lower.Contains("location") || lower.Contains("race_control") || lower.Contains("pit") ||
            lower.Contains("stints") || lower.Contains("overtakes") || lower.Contains("laps"))
            return TimeSpan.FromSeconds(3);
        if (lower.Contains("championship") || lower.Contains("standings"))
            return TimeSpan.FromMinutes(30);
        if (lower.Contains("meetings") || lower.Contains("sessions") || lower.Contains("schedule"))
            return TimeSpan.FromHours(1);
        if (lower.Contains("session_result") || lower.Contains("results"))
            return TimeSpan.FromHours(24);
        return TimeSpan.FromMinutes(5);
    }
}
