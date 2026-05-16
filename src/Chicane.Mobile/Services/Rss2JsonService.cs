using System.Text.Json;
using System.Text.Json.Serialization;
using Chicane.Mobile.Models;

namespace Chicane.Mobile.Services;

public class Rss2JsonService : INewsService
{
    private readonly HttpClient _http;

    private static readonly Dictionary<string, (string Url, string Label)[]> FeedMap = new()
    {
        ["f1"] = new[] {
            ("https://www.skysports.com/rss/12433", "Sky Sports F1"),
            ("https://it.motorsport.com/rss/f1/news/", "Motorsport Italia")
        },
        ["wec"] = new[] {
            ("https://www.motorsport.com/rss/wec/news/", "Motorsport WEC")
        },
        ["wrc"] = new[] {
            ("https://www.motorsport.com/rss/wrc/news/", "Motorsport WRC")
        },
        ["motogp"] = new[] {
            ("https://www.motorsport.com/rss/motogp/news/", "Motorsport MotoGP")
        },
    };

    public Rss2JsonService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<NewsItem>> GetNewsAsync(string categoryId)
    {
        if (!FeedMap.TryGetValue(categoryId, out var feeds))
            return await GetMockNewsAsync(categoryId);

        try
        {
            var tasks = feeds.Select(f => FetchFeedAsync(f.Url, f.Label));
            var results = await Task.WhenAll(tasks);

            return results
                .SelectMany(r => r)
                .OrderByDescending(i => i.PublishedAt)
                .Take(25)
                .ToList();
        }
        catch
        {
            return await GetMockNewsAsync(categoryId);
        }
    }

    private async Task<List<NewsItem>> FetchFeedAsync(string rssUrl, string sourceName)
    {
        try
        {
            var encodedUrl = Uri.EscapeDataString(rssUrl);
            var apiUrl = $"https://api.rss2json.com/v1/api.json?rss_url={encodedUrl}&api_key={Env.Get("RSS2JSON_API_KEY")}";

            var json = await _http.GetStringAsync(apiUrl);
            var response = JsonSerializer.Deserialize<Rss2JsonResponse>(json);

            return response?.Items.Select(i => new NewsItem
            {
                Title = i.Title ?? "Untitled",
                Description = StripHtml(i.Description ?? ""),
                ImageUrl = i.Thumbnail ?? "",
                SourceUrl = i.Link ?? "",
                PublishedAt = DateTime.TryParse(i.PubDate, out var d) ? d : DateTime.UtcNow,
                Source = sourceName,
            }).ToList() ?? new();
        }
        catch
        {
            return new();
        }
    }

    private static Task<List<NewsItem>> GetMockNewsAsync(string categoryId) =>
        Task.FromResult(new List<NewsItem>
        {
            new() { Title = "News coming soon", Description = "Stay tuned for updates.", Source = "Chicane", PublishedAt = DateTime.UtcNow }
        });

    private static string StripHtml(string html)
    {
        var text = System.Text.RegularExpressions.Regex.Replace(html, "<[^>]+>", "");
        text = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ");
        return text.Trim().Length > 250 ? text.Trim()[..250] + "..." : text.Trim();
    }

    private class Rss2JsonResponse
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("items")]
        public List<Rss2JsonItem> Items { get; set; } = new();
    }

    private class Rss2JsonItem
    {
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("link")]
        public string? Link { get; set; }

        [JsonPropertyName("pubDate")]
        public string? PubDate { get; set; }

        [JsonPropertyName("thumbnail")]
        public string? Thumbnail { get; set; }

        [JsonPropertyName("author")]
        public string? Author { get; set; }
    }
}
