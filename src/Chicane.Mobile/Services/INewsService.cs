using Chicane.Mobile.Models;

namespace Chicane.Mobile.Services;

public interface INewsService
{
    Task<List<NewsItem>> GetNewsAsync(string categoryId);
}
