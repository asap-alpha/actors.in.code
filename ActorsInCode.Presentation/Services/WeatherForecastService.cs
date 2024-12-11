using ActorsInCode.Presentation.Model;
using ActorsInCode.Presentation.Repositories;
using Newtonsoft.Json;

namespace ActorsInCode.Presentation.Services;

public class WeatherForecastService : IWeatherForecastService
{
    private readonly IRedisRepository _redisRepository;
    private readonly ILogger<WeatherForecastService> _logger;
    private readonly IKafkaProducerService _kafkaProducerService;

    public WeatherForecastService(IRedisRepository redisRepository, ILogger<WeatherForecastService> logger, IKafkaProducerService kafkaProducerService)
    {
        _redisRepository = redisRepository;
        _logger = logger;
        _kafkaProducerService = kafkaProducerService;
    }

    public async Task<WeatherData> GetWeatherData()
    {
        var weatherForecastRangeData =
            Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Constants.Summaries[Random.Shared.Next(Constants.Summaries.Length)]
                })
                .ToArray();

        _logger.LogDebug("Total generated weather forecast data {Data}", JsonConvert.SerializeObject(weatherForecastRangeData));
        //loop over the range data and check for unique
        //remove the already exist.
        //persist only the unique ones to redis 
        //and produce the unique ones.
        
        var persistToRedis = await _redisRepository.Add(weatherForecastRangeData[0]);

        if (persistToRedis)
        {
            await _kafkaProducerService.KafkaProducer(weatherForecastRangeData.ToList());
            return new WeatherData
            {
                Data = weatherForecastRangeData.ToList(),
                IsPersisted = true
            };
        }

        return new WeatherData()
        {
            IsPersisted = false
        };
    }
}