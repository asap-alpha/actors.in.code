using ActorsInCode.Presentation.Model;

namespace ActorsInCode.Presentation.Services;

public interface IWeatherForecastService
{
    public Task<WeatherData> GetWeatherData();
}

