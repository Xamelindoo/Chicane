using System.Security.Cryptography;
using System.Text;

namespace Chicane.Mobile.Services;

/// <summary>
/// Downloads remote images, stores them on local disk, and returns the local file path.
/// Subsequent requests for the same URL return the cached file instantly (no network).
/// </summary>
public class ImageCacheService
{
    private readonly HttpClient _http;
    private readonly string _cacheDir;

    public ImageCacheService(HttpClient http)
    {
        _http = http;
        _cacheDir = Path.Combine(FileSystem.AppDataDirectory, "img_cache");
        Directory.CreateDirectory(_cacheDir);
    }

    /// <summary>
    /// Returns the local file path for a remote image URL.
    /// Downloads and saves the image on first call; returns cached file on subsequent calls.
    /// Falls back to the original URL if download fails.
    /// </summary>
    public async Task<string> CacheAsync(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return string.Empty;

        // Generate a unique filename from the URL using SHA256 hash
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(url))).ToLowerInvariant();
        var ext = Path.GetExtension(new Uri(url).AbsolutePath);
        if (string.IsNullOrEmpty(ext)) ext = ".jpg";
        var localPath = Path.Combine(_cacheDir, $"{hash}{ext}");

        // Return cached file if it already exists
        if (File.Exists(localPath))
            return localPath;

        try
        {
            var bytes = await _http.GetByteArrayAsync(url);
            await File.WriteAllBytesAsync(localPath, bytes);
            System.Diagnostics.Debug.WriteLine($"[ImgCache] Saved: {url} → {hash}{ext}");
            return localPath;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ImgCache] Failed: {url} — {ex.Message}");
            return url; // Fallback: keep the remote URL
        }
    }
}
