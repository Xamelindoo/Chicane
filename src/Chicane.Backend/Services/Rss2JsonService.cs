using System.Text.Json;

namespace Chicane.Backend.Services;

public class Rss2JsonService
{
    private readonly HttpClient _http;
    private readonly string? _apiKey;
    private readonly ILogger<Rss2JsonService> _logger;

    public Rss2JsonService(HttpClient http, IConfiguration config, ILogger<Rss2JsonService> logger)
    {
        _http = http;
        _apiKey = config["Rss2Json:ApiKey"];
        _logger = logger;
        _logger.LogInformation("ApiKey configured: {HasKey}", !string.IsNullOrEmpty(_apiKey));
    }

    public async Task<List<Dictionary<string, object?>>> GetNewsAsync()
    {
        var feeds = new (string Url, string Label)[]
        {
            ("https://www.skysports.com/rss/12433", "Sky Sports F1"),
            ("https://it.motorsport.com/rss/f1/news", "Motorsport Italia"),
            ("https://www.formulapassion.it/feed", "FormulaPassion"),
            ("https://www.fuoritraiettoria.com/feed", "FuoriTraiettoria"),
            ("https://www.p300.it/feed", "P300.it"),
        };

        var results = new List<Dictionary<string, object?>>();
        foreach (var (url, label) in feeds)
        {
            try
            {
                var encodedUrl = Uri.EscapeDataString(url);
                var apiUrl = $"https://api.rss2json.com/v1/api.json?rss_url={encodedUrl}&api_key={_apiKey}";
                _logger.LogInformation("Fetching {Label}...", label);
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
                        ["publishedAt"] = SafeDateTime(item, "pubDate"),
                        ["source"] = label,
                    });
                }
                _logger.LogInformation("{Label}: {Count} items", label, items.GetArrayLength());
            }
            catch (Exception ex) { _logger.LogWarning(ex, "{Label} failed", label); }
        }

        return results
            .Where(r => IsF1Relevant(r))
            .OrderByDescending(r => (DateTime)(r["publishedAt"]!))
            .Take(20).ToList();
    }

    private static string SafeString(JsonElement item, string key)
    {
        if (item.TryGetProperty(key, out var el) && el.ValueKind == JsonValueKind.String)
            return el.GetString() ?? "";
        return "";
    }

    private static DateTime SafeDateTime(JsonElement item, string key)
    {
        if (!item.TryGetProperty(key, out var el)) return DateTime.UtcNow;
        if (el.ValueKind == JsonValueKind.String && DateTime.TryParse(el.GetString(), out var d))
            return d;
        try { return el.GetDateTime(); }
        catch { return DateTime.UtcNow; }
    }

    private static readonly string[] _f1Keywords = ["F1", "Formula 1", "Formula One", "FIA", "Grand Prix", "GP",
        "Verstappen", "Norris", "Leclerc", "Hamilton", "Piastri", "Russell", "Sainz", "Alonso",
        "circuit", "Monza", "Imola", "Spa", "Silverstone", "Monaco", "Miami GP", "Las Vegas GP",
        "pit stop", "pole position", "championship", "paddock", "MotoGP", "WEC", "WRC", "Formula E"];

    private static bool IsF1Relevant(Dictionary<string, object?> article)
    {
        var title = (article["title"] as string) ?? "";
        var desc = (article["description"] as string) ?? "";
        var combined = $"{title} {desc}";
        return _f1Keywords.Any(k => combined.Contains(k, StringComparison.OrdinalIgnoreCase));
    }
}
