namespace ActorsInCode.Presentation.Repositories;

public interface IRedisRepository
{
 Task<bool> Add(WeatherForecast payloads);

 Task<bool> IsKeyAlreadyExist(WeatherForecast payload);
}