namespace Chicane.Mobile.Services;

public class CacheService : ICacheService
{
    public double GetCacheSizeMB()
    {
        try
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "chicane.db");
            var imgCacheDir = Path.Combine(FileSystem.AppDataDirectory, "img_cache");

            double totalBytes = 0;

            if (File.Exists(dbPath))
                totalBytes += new FileInfo(dbPath).Length;

            if (Directory.Exists(imgCacheDir))
            {
                foreach (var file in Directory.GetFiles(imgCacheDir))
                    totalBytes += new FileInfo(file).Length;
            }

            return Math.Round(totalBytes / (1024.0 * 1024.0), 1);
        }
        catch
        {
            return 0;
        }
    }

    public async Task ClearImageCacheAsync()
    {
        var imgCacheDir = Path.Combine(FileSystem.AppDataDirectory, "img_cache");
        if (Directory.Exists(imgCacheDir))
        {
            await Task.Run(() =>
            {
                foreach (var file in Directory.GetFiles(imgCacheDir))
                {
                    try { File.Delete(file); } catch { }
                }
            });
        }
    }
}
