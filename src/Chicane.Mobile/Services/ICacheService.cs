namespace Chicane.Mobile.Services;

public interface ICacheService
{
    double GetCacheSizeMB();
    Task ClearImageCacheAsync();
}
