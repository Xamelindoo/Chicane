using System.Text.Json.Serialization;

namespace Chicane.Mobile.Models.Api;

public class OpenWeatherResponse
{
    [JsonPropertyName("list")]
    public List<OpenWeatherEntry> List { get; set; } = new();
}

public class OpenWeatherEntry
{
    [JsonPropertyName("dt")]
    public long Dt { get; set; }

    [JsonPropertyName("main")]
    public OpenWeatherMain Main { get; set; } = new();

    [JsonPropertyName("weather")]
    public List<OpenWeatherCondition> Weather { get; set; } = new();

    [JsonPropertyName("wind")]
    public OpenWeatherWind Wind { get; set; } = new();
}

public class OpenWeatherMain
{
    [JsonPropertyName("temp")]
    public double Temp { get; set; }

    [JsonPropertyName("humidity")]
    public int Humidity { get; set; }

    [JsonPropertyName("pressure")]
    public int Pressure { get; set; }
}

public class OpenWeatherCondition
{
    [JsonPropertyName("main")]
    public string Main { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("icon")]
    public string Icon { get; set; } = string.Empty;
}

public class OpenWeatherWind
{
    [JsonPropertyName("speed")]
    public double Speed { get; set; }
}
