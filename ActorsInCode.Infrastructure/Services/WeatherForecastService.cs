using ActorsInCode.Domain.Constants;
using ActorsInCode.Domain.Models;
using ActorsInCode.Domain.Models.Request;
using ActorsInCode.Domain.Models.Response;
using ActorsInCode.Infrastructure.Repositories;
using ActorsInCode.Presentation.Services;
using Mapster;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ActorsInCode.Infrastructure.Services;

public class WeatherForecastService : IWeatherForecastService
{
    private readonly IRedisRepository _redisRepository;
    private readonly ILogger<WeatherForecastService> _logger;
    private readonly IKafkaProducerService _kafkaProducerService;

    public WeatherForecastService(IRedisRepository redisRepository, ILogger<WeatherForecastService> logger,
        IKafkaProducerService kafkaProducerService)
    {
        _redisRepository = redisRepository;
        _logger = logger;
        _kafkaProducerService = kafkaProducerService;
    }

    public async Task<WeatherData> GetWeatherData()
    {
        var sanitizePayload = new List<WeatherForecastRequest>();
        var weatherForecastRangeData =
            Enumerable.Range(1, 5).Select(index => new WeatherForecastRequest
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Constants.Summaries[Random.Shared.Next(Constants.Summaries.Length)]
                })
                .ToArray();

        _logger.LogDebug("Total {Count} generated weather forecast data {Data}", weatherForecastRangeData.Length,
            JsonConvert.SerializeObject(weatherForecastRangeData));
        //loop over the range data and check for unique
        var remainingPayloads = await _redisRepository.IsKeyAlreadyExist(weatherForecastRangeData.ToList());
        
        //remove the already exist.
        foreach (var remainingPayload in remainingPayloads)
        {
            sanitizePayload = weatherForecastRangeData
                .Where(payload => !remainingPayload.ExtraData.Duplicated || payload != remainingPayload
                )
                .ToList();
        }

        //persist only the unique ones to redis 
        var persistToRedis = await _redisRepository.Add(sanitizePayload.ToList());

        if (persistToRedis)
        {
            //and produce the unique ones.
            await _kafkaProducerService.KafkaProducer(sanitizePayload.ToList());
            return new WeatherData
            {
                Data = weatherForecastRangeData.Adapt<List<WeatherForecastResponse>>(),
                IsPersisted = true
            };
        }

        return new WeatherData()
        {
            IsPersisted = false
        };
    }
}