using ActorsInCode.Domain.Models.Response;

namespace ActorsInCode.Domain.Models;

public record WeatherData
{
    public bool IsPersisted { get; init; }
    public List<WeatherForecastResponse> Data { get; init; }
};