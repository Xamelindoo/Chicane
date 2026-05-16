using System.Diagnostics;

namespace Chicane.Mobile.Services;

public class CacheService : ICacheService
{
    public (double TotalMB, double DbMB, double ImgMB) GetCacheInfo()
    {
        double dbBytes = 0, imgBytes = 0;
        try
        {
            var appDir = FileSystem.AppDataDirectory;

            // SQLite database + WAL/journal files
            foreach (var pattern in new[] { "chicane.db", "chicane.db-wal", "chicane.db-shm", "chicane.db-journal" })
            {
                var path = Path.Combine(appDir, pattern);
                if (File.Exists(path))
                    dbBytes += new FileInfo(path).Length;
            }

            // Image cache
            var imgCacheDir = Path.Combine(appDir, "img_cache");
            if (Directory.Exists(imgCacheDir))
            {
                foreach (var file in Directory.GetFiles(imgCacheDir))
                    imgBytes += new FileInfo(file).Length;
            }

            Debug.WriteLine($"[Cache] DB={dbBytes / 1024.0:F1}KB  IMG={imgBytes / 1024.0:F1}KB");
        }
        catch
        {
            return (0, 0, 0);
        }

        double total = dbBytes + imgBytes;
        return (Math.Round(total / 1048576.0, 1), Math.Round(dbBytes / 1048576.0, 1), Math.Round(imgBytes / 1048576.0, 1));
    }

    public async Task ClearAllCacheAsync()
    {
        await Task.Run(() =>
        {
            var appDir = FileSystem.AppDataDirectory;

            // Delete all DB files (DB + WAL + journal + shm)
            foreach (var pattern in new[] { "chicane.db", "chicane.db-wal", "chicane.db-shm", "chicane.db-journal" })
            {
                var path = Path.Combine(appDir, pattern);
                try { File.Delete(path); } catch { }
            }

            // Delete all image cache files
            var imgCacheDir = Path.Combine(appDir, "img_cache");
            if (Directory.Exists(imgCacheDir))
            {
                foreach (var file in Directory.GetFiles(imgCacheDir))
                {
                    try { File.Delete(file); } catch { }
                }
            }
        });
    }
}
