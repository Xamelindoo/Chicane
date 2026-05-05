using System.Net.Http.Json;
using Chicane.Mobile.Models.Api;

namespace Chicane.Mobile.Services;

public class OpenF1Service
{
    private readonly HttpClient _http;
    private const string BaseUrl = "https://api.openf1.org/v1";

    public OpenF1Service(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<OpenF1Session>> GetSessionsAsync(int year)
    {
        try
        {
            var url = $"{BaseUrl}/sessions?year={year}";
            return await _http.GetFromJsonAsync<List<OpenF1Session>>(url) ?? new();
        }
        catch
        {
            return new();
        }
    }

    public async Task<List<OpenF1Position>> GetPositionsAsync(int sessionKey)
    {
        try
        {
            var url = $"{BaseUrl}/position?session_key={sessionKey}";
            return await _http.GetFromJsonAsync<List<OpenF1Position>>(url) ?? new();
        }
        catch
        {
            return new();
        }
    }
}
