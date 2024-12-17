using ActorsInCode.Domain.Models;
using ActorsInCode.Presentation.Model;

namespace ActorsInCode.Presentation.Services;

public interface IWeatherForecastService
{
     Task<WeatherData> GetWeatherData();
}

