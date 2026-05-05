using System.Text.Json;
using System.Text.Json.Serialization;
using Chicane.Mobile.Models;

namespace Chicane.Mobile.Services;

public class NewsApiService : INewsService
{
    private readonly HttpClient _http;

    public NewsApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<NewsItem>> GetNewsAsync(string categoryId)
    {
        if (categoryId != "f1") return await GetMockNewsAsync(categoryId);

        var key = SecretsConfig.NewsApiKey;
        if (string.IsNullOrWhiteSpace(key)) return await GetMockNewsAsync(categoryId);

        try
        {
            var url = $"https://newsapi.org/v2/everything?q=formula+1&apiKey={key}&pageSize=20&language=en&sortBy=publishedAt";
            var json = await _http.GetStringAsync(url);
            var response = JsonSerializer.Deserialize<NewsApiResponse>(json);

            return response?.Articles.Select(a => new NewsItem
            {
                Title = a.Title ?? "Untitled",
                Description = a.Description ?? "",
                ImageUrl = a.UrlToImage ?? "",
                SourceUrl = a.Url ?? "",
                PublishedAt = a.PublishedAt,
                Source = a.Source?.Name ?? "NewsAPI",
            }).ToList() ?? new();
        }
        catch
        {
            return await GetMockNewsAsync(categoryId);
        }
    }

    private static Task<List<NewsItem>> GetMockNewsAsync(string categoryId) =>
        Task.FromResult(new List<NewsItem>
        {
            new() { Title = "News coming soon", Description = "Stay tuned for updates.", Source = "Chicane", PublishedAt = DateTime.UtcNow }
        });

    private class NewsApiResponse
    {
        [JsonPropertyName("articles")]
        public List<NewsApiArticle> Articles { get; set; } = new();
    }

    private class NewsApiArticle
    {
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("urlToImage")]
        public string? UrlToImage { get; set; }

        [JsonPropertyName("publishedAt")]
        public DateTime PublishedAt { get; set; }

        [JsonPropertyName("source")]
        public NewsApiSource? Source { get; set; }
    }

    private class NewsApiSource
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}
