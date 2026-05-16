using System.Diagnostics;
using System.Text.Json;

namespace Chicane.Backend.Services;

public class Rss2JsonService
{
    private readonly HttpClient _http;
    private readonly string? _apiKey;

    public Rss2JsonService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _apiKey = config["Rss2Json:ApiKey"];
        Debug.WriteLine($"[Rss2Json] ApiKey configured: {!string.IsNullOrEmpty(_apiKey)}");
    }

    public async Task<List<Dictionary<string, object?>>> GetNewsAsync()
    {
        var feeds = new (string Url, string Label)[]
        {
            ("https://www.skysports.com/rss/12040", "Sky Sports F1"),
            ("https://it.motorsport.com/rss/f1/news", "Motorsport Italia"),
        };

        var results = new List<Dictionary<string, object?>>();
        foreach (var (url, label) in feeds)
        {
            try
            {
                var encodedUrl = Uri.EscapeDataString(url);
                var apiUrl = $"https://api.rss2json.com/v1/api.json?rss_url={encodedUrl}&api_key={_apiKey}";
                Debug.WriteLine($"[Rss2Json] Fetching {label}...");
                var json = await _http.GetStringAsync(apiUrl);
                var doc = JsonDocument.Parse(json);
                var items = doc.RootElement.GetProperty("items");

                foreach (var item in items.EnumerateArray())
                {
                    results.Add(new()
                    {
                        ["title"] = item.GetProperty("title").GetString() ?? "",
                        ["description"] = SafeString(item, "description"),
                        ["imageUrl"] = SafeString(item, "thumbnail"),
                        ["sourceUrl"] = item.GetProperty("link").GetString() ?? "",
                        ["publishedAt"] = item.GetProperty("pubDate").GetDateTime(),
                        ["source"] = label,
                    });
                }
                Debug.WriteLine($"[Rss2Json] {label}: {items.GetArrayLength()} items");
            }
            catch (Exception ex) { Debug.WriteLine($"[Rss2Json] {label} failed: {ex.Message}"); }
        }

        return results.OrderByDescending(r => (DateTime)(r["publishedAt"]!)).Take(20).ToList();
    }

    private static string SafeString(JsonElement item, string key)
    {
        if (item.TryGetProperty(key, out var el) && el.ValueKind == JsonValueKind.String)
            return el.GetString() ?? "";
        return "";
    }
}
