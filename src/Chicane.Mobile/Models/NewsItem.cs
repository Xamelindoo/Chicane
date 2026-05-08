namespace Chicane.Mobile.Models;

public class NewsItem
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;
    public string SourceUrl { get; init; } = string.Empty;
    public DateTime PublishedAt { get; init; }
    public string Source { get; init; } = string.Empty;

    public bool HasImage => !string.IsNullOrWhiteSpace(ImageUrl);

    public string TimeAgo
    {
        get
        {
            var diff = DateTime.UtcNow - PublishedAt;
            if (diff.TotalMinutes < 1) return "just now";
            if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes}m ago";
            if (diff.TotalHours < 24) return $"{(int)diff.TotalHours}h ago";
            return $"{(int)diff.TotalDays}d ago";
        }
    }
}
