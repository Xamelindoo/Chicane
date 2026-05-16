using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Chicane.Backend.Services;

namespace Chicane.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewsController : ControllerBase
{
    private readonly Rss2JsonService _news;
    private readonly IDistributedCache _cache;

    public NewsController(Rss2JsonService news, IDistributedCache cache) { _news = news; _cache = cache; }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var cached = await _cache.GetStringAsync("api_news");
        if (cached is not null) return Content(cached, "application/json");

        var items = await _news.GetNewsAsync();
        var result = JsonSerializer.Serialize(items);
        await _cache.SetStringAsync("api_news", result, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) });
        return Content(result, "application/json");
    }
}
