using System.Xml.Linq;
using Chicane.Mobile.Models;

namespace Chicane.Mobile.Services;

public class RssNewsService : INewsService
{
    private readonly HttpClient _http;

    public RssNewsService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<NewsItem>> GetNewsAsync(string categoryId)
    {
        if (categoryId != "f1") return await GetMockNewsAsync(categoryId);

        try
        {
            var stream = await _http.GetStreamAsync("https://www.skysports.com/rss/12433");
            var doc = XDocument.Load(stream);
            var ns = doc.Root?.GetDefaultNamespace() ?? XNamespace.None;
            var mediaNs = XNamespace.Get("http://search.yahoo.com/mrss/");

            var items = doc.Descendants(ns + "item").Take(15).Select(item =>
            {
                var enclosure = item.Element(ns + "enclosure");
                var mediaContent = item.Element(mediaNs + "content");
                var imageUrl = mediaContent?.Attribute("url")?.Value
                    ?? enclosure?.Attribute("url")?.Value
                    ?? string.Empty;

                var pubDateStr = item.Element(ns + "pubDate")?.Value ?? "";
                DateTime.TryParse(pubDateStr, out var pubDate);

                return new NewsItem
                {
                    Title = item.Element(ns + "title")?.Value ?? "Untitled",
                    Description = StripHtml(item.Element(ns + "description")?.Value ?? ""),
                    ImageUrl = imageUrl,
                    SourceUrl = item.Element(ns + "link")?.Value ?? "",
                    PublishedAt = pubDate,
                    Source = "Sky Sports F1",
                };
            }).ToList();

            return items;
        }
        catch
        {
            return await GetMockNewsAsync(categoryId);
        }
    }

    private static Task<List<NewsItem>> GetMockNewsAsync(string categoryId) =>
        Task.FromResult(new List<NewsItem>
        {
            new() { Title = "Racing news coming soon", Description = "Stay tuned for the latest updates.", Source = "Chicane", PublishedAt = DateTime.UtcNow }
        });

    private static string StripHtml(string html)
    {
        var text = System.Text.RegularExpressions.Regex.Replace(html, "<[^>]+>", "");
        text = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ");
        return text.Trim().Length > 250 ? text.Trim()[..250] + "..." : text.Trim();
    }
}
