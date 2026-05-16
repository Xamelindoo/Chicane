using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Chicane.Backend.Services;

public class OpenF1Service
{
    private readonly HttpClient _http;
    private readonly string? _username;
    private readonly string? _password;

    public OpenF1Service(HttpClient http, IConfiguration config)
    {
        _http = http;
        _username = config["OpenF1:Username"];
        _password = config["OpenF1:Password"];
        _http.BaseAddress = new Uri("https://api.openf1.org/v1/");
        _http.DefaultRequestHeaders.Add("User-Agent", "Chicane-Backend/1.0");
    }

    private async Task AuthenticateAsync()
    {
        if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password)) return;
        if (_http.DefaultRequestHeaders.Authorization is not null) return;
        try
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("username", _username),
                new KeyValuePair<string, string>("password", _password),
            });
            var response = await _http.PostAsync("https://api.openf1.org/token", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            var token = doc.RootElement.GetProperty("access_token").GetString()!;
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            Debug.WriteLine("[OpenF1] Authenticated");
        }
        catch (Exception ex) { Debug.WriteLine($"[OpenF1] Auth failed: {ex.Message}"); }
    }

    public async Task<string?> GetRawStringAsync(string path)
    {
        await AuthenticateAsync();
        try
        {
            return await _http.GetStringAsync(path);
        }
        catch { return null; }
    }
}
