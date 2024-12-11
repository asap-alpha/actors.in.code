namespace ActorsInCode.Presentation.Repositories;

public interface IRedisRepository
{
 Task<bool> Add(List<WeatherForecast> payloads);

 Task<List<WeatherForecast>> IsKeyAlreadyExist(List<WeatherForecast> payloads);
}