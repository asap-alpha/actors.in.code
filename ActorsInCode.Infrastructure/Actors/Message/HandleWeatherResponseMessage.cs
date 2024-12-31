using ActorsInCode.Domain.Models.Response;

namespace ActorsInCode.Infrastructure.Actors.Message;

public struct HandleWeatherResponseMessage
{
    public readonly WeatherForecastResponse WeatherForecastResponse { get; } 
    public HandleWeatherResponseMessage(WeatherForecastResponse weatherForecastResponse)
    {
        WeatherForecastResponse = weatherForecastResponse;
    }
}
