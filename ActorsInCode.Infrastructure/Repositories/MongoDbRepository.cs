using ActorsInCode.Domain.Models.Response;
using ActorsInCode.Domain.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ActorsInCode.Infrastructure.Repositories;

public class MongoDbRepository:IMongoDbRepository
{

    private readonly IMongoDatabase _mongoDatabase;
    
    public MongoDbRepository(IOptions<MongoClientConfig> config)
    {
        var mongoClientConfig = config.Value;
        var client = new MongoClient(mongoClientConfig.DbConnectionString);
        _mongoDatabase = client.GetDatabase(mongoClientConfig.DatabaseInstance);

    }
    
    public async Task<bool> SaveResult(List<WeatherForecastResponse> payloads, CancellationToken token)
    {

        foreach (var payload in payloads)
        {
            var collection = _mongoDatabase.GetCollection<WeatherForecastResponse>("WeatherData");
            await collection.InsertOneAsync(payload, token);
        }
    }
}

 