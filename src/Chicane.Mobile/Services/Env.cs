using System.Diagnostics;

namespace Chicane.Mobile.Services;

public static class Env
{
    private static readonly Dictionary<string, string> _values = new();

    static Env()
    {
        try
        {
            using var stream = FileSystem.Current.OpenAppPackageFileAsync("env")
                .GetAwaiter().GetResult();
            using var reader = new StreamReader(stream);
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();
                if (string.IsNullOrEmpty(line) || line.StartsWith('#')) continue;
                var eq = line.IndexOf('=');
                if (eq <= 0) continue;
                var key = line[..eq].Trim();
                var value = line[(eq + 1)..].Trim();
                _values[key] = value;
                Debug.WriteLine($"[Env] Loaded: {key}");
            }
            Debug.WriteLine($"[Env] OK — {_values.Count} keys loaded");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[Env] FAILED: {ex.Message}");
        }
    }

    public static string Get(string key) =>
        _values.TryGetValue(key, out var val) ? val : string.Empty;
}
