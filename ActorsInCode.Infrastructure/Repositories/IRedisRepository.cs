using ActorsInCode.Domain.Models.Request;
using ActorsInCode.Domain.Models.Response;

namespace ActorsInCode.Infrastructure.Repositories;

public interface IRedisRepository
{
 Task<bool> Add(List<WeatherForecastRequest> payloads);

 Task<List<WeatherForecastResponse>> IsKeyAlreadyExist(List<WeatherForecastRequest> payloads);
}