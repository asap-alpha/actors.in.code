using ActorsInCode.Domain.Constants;
using ActorsInCode.Domain.Models.Request;
using ActorsInCode.Domain.Models.Response;
using ActorsInCode.Presentation.Model.Options;
using Mapster;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace ActorsInCode.Infrastructure.Repositories;

public class RedisRepository : IRedisRepository
{
    private readonly IDatabase _database;
    private readonly ILogger<RedisRepository> _logger;
    private readonly int _redisTTl;

    public RedisRepository(IOptions<RedisConfiguration> redisConfiguration, ILogger<RedisRepository> logger)
    {
        try
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
        catch (Exception e)
        {
            _logger.LogDebug(e, "Unable to retrieve appsettings for redis instance!... {StackTrace}, {Message}",
                e.StackTrace, e.Message);
            throw new ArgumentNullException(nameof(redisConfiguration), "Redis configuration is null!...");
        }
    }

    public async Task<bool> Add(List<WeatherForecastRequest> payloads)
    {
        var result = false;
        foreach (var payload in payloads)
        {
            payload.ExtraData = null;
            var key = RedisConstant.Key.RedisKeys.Replace("{summary}", payload.Summary);

            var serializePayload = JsonConvert.SerializeObject(payload);

            _logger.LogDebug("persisting payload {Payload} to redis \n with key {Key}", serializePayload, key);

            result = await _database.StringSetAsync(key, serializePayload,
                TimeSpan.FromMinutes(_redisTTl));

            _logger.LogInformation("payload was {Status} persisted to redis with key {Key}", result, key);
        }

        return result;
    }

    public async Task<List<WeatherForecastResponse>> IsKeyAlreadyExist(List<WeatherForecastRequest> payloads)
    {
        var remainingPayload = new List<WeatherForecastResponse>();
        foreach (var payload in payloads)
        {
            var serializePayload = JsonConvert.SerializeObject(payload);
            var key = RedisConstant.Key.RedisKeys.Replace("{summary}", payload.Summary);
            var keyAlreadyExist = await _database.KeyExistsAsync(key);
            if (keyAlreadyExist)
            {
                payload.ExtraData.Duplicated = true;
                payload.ExtraData.RedisKey = key;
                remainingPayload.Add(payload);
                _logger.LogDebug("Key already exist {Key} \n with payload {Payload}", key, serializePayload);
            }

            else
            {
                payload.ExtraData.Duplicated = false;
                payload.ExtraData.RedisKey = key;
                remainingPayload.Add(payload);
                _logger.LogDebug("new payload got! {Payload} \n with key {Key}", serializePayload, key);
            }
        }

        return remainingPayload;
    }

    public async Task<List<WeatherForecastResponse>> ReadWeatherData(List<WeatherForecastRequest> data)
    {
        var availableData = new List<WeatherForecastResponse>();
        foreach (var payload in data)
        {
            var key = RedisConstant.Key.RedisKeys.Replace("{summary}", payload.Summary);

            var retrieveAllData = await _database.StringGetAsync(key);
            availableData.Add(retrieveAllData.Adapt<WeatherForecastResponse>());
        }

        return availableData;
    }
}