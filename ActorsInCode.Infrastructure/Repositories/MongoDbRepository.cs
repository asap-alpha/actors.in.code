using ActorsInCode.Domain.Models.Response;
using ActorsInCode.Domain.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace ActorsInCode.Infrastructure.Repositories;

public class MongoDbRepository : IMongoDbRepository
{
    private readonly IMongoDatabase _mongoDatabase;
    private readonly ILogger<MongoDbRepository> _logger;

    public MongoDbRepository(IOptions<MongoClientConfig> config, ILogger<MongoDbRepository> logger)
    {
        _logger = logger;
        var mongoClientConfig = config.Value;
        var client = new MongoClient(mongoClientConfig.DbConnectionString);
        _mongoDatabase = client.GetDatabase(mongoClientConfig.DatabaseInstance);
    }

    public async Task<bool> SaveResult(WeatherForecastResponse payload)
    {
        try
        {

            CancellationToken token = new CancellationToken();
            _logger.LogDebug("persisting payload {Payload}", JsonConvert.SerializeObject(payload));

            var collection = _mongoDatabase.GetCollection<WeatherForecastResponse>("WeatherData");
            await collection.InsertOneAsync(
                payload, new InsertOneOptions()
                {
                    BypassDocumentValidation = true
                }, token);

            return true;
        }
        catch (Exception e)
        {
            _logger.LogDebug(e, "exception occured {Trace}", e.StackTrace);
            return false;
        }
    }
}