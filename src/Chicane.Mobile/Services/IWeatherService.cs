using Chicane.Mobile.Models;

namespace Chicane.Mobile.Services;

public interface IWeatherService
{
    Task<WeatherForecast?> GetForecastAsync(string city, string country, DateTime eventDate);
}
