namespace ActorsInCode.Presentation.Model;

public record WeatherData
{
    public List<WeatherForecast> Data { get; init; }
    public bool IsPersisted { get; init; }
};