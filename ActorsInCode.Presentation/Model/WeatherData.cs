namespace ActorsInCode.Presentation.Model;

public record WeatherData
{
    public bool IsPersisted { get; init; }
    public List<WeatherForecast> Data { get; init; }
};