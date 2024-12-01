namespace ActorsInCode.Presentation.Services.Repository;

public interface IRedisRepository
{
 Task<bool> Add(List<WeatherForecast> payloads);
}