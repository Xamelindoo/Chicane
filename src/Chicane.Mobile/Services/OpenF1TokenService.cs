using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

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
            // Double-check inside lock
            if (_accessToken is not null && DateTime.UtcNow < _expiresAt.AddMinutes(-5))
                return _accessToken;

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", SecretsConfig.OpenF1Username),
                new KeyValuePair<string, string>("password", SecretsConfig.OpenF1Password),
            });

            var response = await _http.PostAsync(BaseAuthUrl, content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<OpenF1TokenResponse>(json);

            _accessToken = tokenResponse?.AccessToken ?? throw new InvalidOperationException("No token");
            _expiresAt = DateTime.UtcNow.AddSeconds(tokenResponse?.ExpiresIn ?? 3600);

            Debug.WriteLine($"[OpenF1] Token refreshed, expires at {_expiresAt}");

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

    private class OpenF1TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
