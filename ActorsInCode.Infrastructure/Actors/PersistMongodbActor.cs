using ActorsInCode.Domain.Models.Response;
using ActorsInCode.Infrastructure.Actors.Message;
using ActorsInCode.Infrastructure.Repositories;

namespace ActorsInCode.Infrastructure.Actors;

public class PersistMongodbActor : BaseActor
{
    private readonly IMongoDbRepository _mongoDbRepository;
    public PersistMongodbActor(IMongoDbRepository mongoDbRepository)
    {
        _mongoDbRepository = mongoDbRepository;

        ReceiveAsync<WeatherForecastResponse>(HandlePersistToMongoDBActor);
    }

    public async Task  HandlePersistToMongoDBActor(WeatherForecastResponse message)
    {
        // var stoppingToken = new CancellationToken();
        try
        {
            await _mongoDbRepository.SaveResult(message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}