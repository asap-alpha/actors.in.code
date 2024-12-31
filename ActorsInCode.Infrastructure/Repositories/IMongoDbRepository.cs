using ActorsInCode.Domain.Models.Response;

namespace ActorsInCode.Infrastructure.Repositories;

public interface IMongoDbRepository
{
    Task<bool> SaveResult(WeatherForecastResponse payload);
}