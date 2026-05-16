namespace Chicane.Mobile.Services;

public interface ICacheService
{
    (double TotalMB, double DbMB, double ImgMB) GetCacheInfo();
    Task ClearAllCacheAsync();
}
