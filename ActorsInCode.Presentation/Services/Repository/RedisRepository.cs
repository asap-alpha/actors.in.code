using Newtonsoft.Json;
using StackExchange.Redis;

namespace ActorsInCode.Presentation.Services.Repository;

public class RedisRepository:IRedisRepository
{
    private readonly IDatabase _database;
  

    public RedisRepository()
    {
        var configurationOptions = new ConfigurationOptions
        {
            EndPoints = { "127.0.0.1:6379" }
            
        };
        var connect = ConnectionMultiplexer.Connect(configurationOptions);

        _database = connect.GetDatabase(6);
        
    }

    public async Task<bool> Add(List<WeatherForecast> payloads)
    {
        var isSuccessful  = false;
        foreach (var payload in payloads)
        {
            var key = $"weather:{payload.Summary}";

           var result   = await _database.StringSetAsync( key, JsonConvert.SerializeObject(payload), TimeSpan.FromMinutes(30));

           if (result)
           {
               isSuccessful = true;
           }
        }
        
        return isSuccessful;
    }
}