namespace ActorsInCode.Presentation.Repositories;

public interface IRedisRepository
{
 Task<bool> Add(List<WeatherForecast> payloads);
}