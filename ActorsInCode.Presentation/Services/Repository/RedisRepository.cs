using ActorsInCode.Presentation.Model.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace ActorsInCode.Presentation.Services.Repository;

public class RedisRepository : IRedisRepository
{
    private readonly IDatabase _database;
    private readonly ILogger<RedisRepository> _logger;
    private readonly int _redisTTl;

    public RedisRepository(IOptions<RedisConfiguration> redisConfiguration, ILogger<RedisRepository> logger)
    {
        var redisConfig = redisConfiguration.Value;
        var configurationOptions = new ConfigurationOptions
        {
            EndPoints = { redisConfig!.RedisInstance }
        };

        _redisTTl = redisConfig.Ttl;
        var connect = ConnectionMultiplexer.Connect(configurationOptions);

        _logger = logger;
        _database = connect.GetDatabase(redisConfig.RedisDb);
    }

    public async Task<bool> Add(List<WeatherForecast> payloads)
    {
        var isSuccessful = false;

        _logger.LogDebug("Total payloads received {Count}", payloads.Count);
        foreach (var payload in payloads)
        {
            var key = $"weather:{payload.Summary}";
            var keyAlreadyExist = await _database.KeyExistsAsync(key);
            if (!keyAlreadyExist)
            {
                var serializePayload = JsonConvert.SerializeObject(payload);

                _logger.LogDebug("persisting payload {Payload} to redis \n with key {Key}", serializePayload, key);

                var result = await _database.StringSetAsync(key, serializePayload,
                    TimeSpan.FromMinutes(_redisTTl));

                if (result)
                {
                    isSuccessful = true;
                }
            }

            _logger.LogInformation("payload was {Status} persisted to redis with key {Key}",
                isSuccessful ? "successful" : "unsuccessful", key);
        }


        return isSuccessful;
    }
}