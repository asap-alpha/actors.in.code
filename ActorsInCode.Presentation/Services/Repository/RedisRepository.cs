using ActorsInCode.Presentation.Model.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace ActorsInCode.Presentation.Services.Repository;

public class RedisRepository : IRedisRepository
{
    private readonly IDatabase _database;

    public RedisRepository(IOptions<RedisConfiguration> redisConfiguration)
    {
        var redisConfig = redisConfiguration.Value;
        var configurationOptions = new ConfigurationOptions
        {
            EndPoints = { redisConfig!.RedisInstance }
        };
        var connect = ConnectionMultiplexer.Connect(configurationOptions);

        _database = connect.GetDatabase(redisConfig.RedisDb);
    }

    public async Task<bool> Add(List<WeatherForecast> payloads)
    {
        var isSuccessful = false;
        foreach (var payload in payloads)
        {
            var key = $"weather:{payload.Summary}";
            var keyAlreadyExist = await _database.KeyExistsAsync(key, CommandFlags.None);
            if (!keyAlreadyExist)
            {
                var result = await _database.StringSetAsync(key, JsonConvert.SerializeObject(payload),
                    TimeSpan.FromMinutes(30));

                if (result)
                {
                    isSuccessful = true;
                }
            }
        }

        return isSuccessful;
    }
}