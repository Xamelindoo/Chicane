using System.Text.Json;

namespace Chicane.Backend;

public static class JsonConfig
{
    public static readonly JsonSerializerOptions CamelCaseOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
}
