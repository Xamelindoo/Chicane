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
}
