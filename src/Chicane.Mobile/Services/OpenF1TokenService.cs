using System.Diagnostics;
using System.Text.Json;

namespace Chicane.Mobile.Services;

public class OpenF1TokenService
{
    private readonly HttpClient _http;
    private string? _accessToken;
    private DateTime _expiresAt = DateTime.MinValue;
    private Timer? _refreshTimer;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private const string BaseAuthUrl = "https://api.openf1.org/token";

    public OpenF1TokenService(HttpClient http)
    {
        _http = http;
    }

    public async Task<string> GetTokenAsync()
    {
        if (_accessToken is not null && DateTime.UtcNow < _expiresAt.AddMinutes(-5))
            return _accessToken;

        await _lock.WaitAsync();
        try
        {
            if (_accessToken is not null && DateTime.UtcNow < _expiresAt.AddMinutes(-5))
                return _accessToken;

            Debug.WriteLine("[OpenF1] POST /token...");
            var response = await _http.PostAsync(BaseAuthUrl, new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", Env.Get("OPENF1_USERNAME")),
                new KeyValuePair<string, string>("password", Env.Get("OPENF1_PASSWORD")),
            }));

            Debug.WriteLine($"[OpenF1] Token HTTP {response.StatusCode}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            _accessToken = root.GetProperty("access_token").GetString()
                ?? throw new InvalidOperationException("No access_token");

            if (root.TryGetProperty("expires_in", out var exp))
            {
                _expiresAt = exp.ValueKind == JsonValueKind.String
                    ? DateTime.UtcNow.AddSeconds(int.Parse(exp.GetString()!))
                    : DateTime.UtcNow.AddSeconds(exp.GetInt32());
            }
            else
            {
                _expiresAt = DateTime.UtcNow.AddHours(1);
            }

            Debug.WriteLine($"[OpenF1] Token ok, expires at {_expiresAt}");

            StartRefreshTimer();

            return _accessToken;
        }
        finally
        {
            _lock.Release();
        }
    }

    private void StartRefreshTimer()
    {
        _refreshTimer?.Dispose();
        var interval = TimeSpan.FromMinutes(55);
        _refreshTimer = new Timer(async _ =>
        {
            try { _accessToken = null; await GetTokenAsync(); }
            catch (Exception ex) { Debug.WriteLine($"[OpenF1] Refresh failed: {ex.Message}"); }
        }, null, interval, interval);
    }
}
