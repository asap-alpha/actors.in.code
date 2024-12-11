namespace ActorsInCode.Presentation.Repositories;

public interface IRedisRepository
{
 Task<bool> Add(List<WeatherForecast> payload);

 Task<List<WeatherForecast>> IsKeyAlreadyExist(List<WeatherForecast> payloads);
}