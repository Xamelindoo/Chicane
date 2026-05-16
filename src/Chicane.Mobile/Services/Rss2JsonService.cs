using System.Text.Json;
using Chicane.Mobile.Models;

namespace Chicane.Mobile.Services;

public class Rss2JsonService : INewsService
{
    private readonly HttpClient _http;

    public Rss2JsonService(HttpClient http) => _http = http;

    public async Task<List<NewsItem>> GetNewsAsync(string categoryId)
    {
        try
        {
            var json = await _http.GetStringAsync("/api/news");
            var arr = JsonDocument.Parse(json).RootElement;
            var items = new List<NewsItem>();
            foreach (var n in arr.EnumerateArray())
            {
                items.Add(new NewsItem
                {
                    Title = n.TryGetProperty("title", out var t) ? t.GetString() ?? "" : "",
                    ImageUrl = n.TryGetProperty("imageUrl", out var i) ? i.GetString() ?? "" : "",
                    SourceUrl = n.TryGetProperty("sourceUrl", out var u) ? u.GetString() ?? "" : "",
                    Source = n.TryGetProperty("source", out var s) ? s.GetString() ?? "" : "",
                    PublishedAt = n.TryGetProperty("publishedAt", out var p) ? p.GetDateTime() : DateTime.UtcNow,
                });
            }
            return items;
        }
        catch { return new(); }
    }
}
