using ActorsInCode.Presentation.Model;
using ActorsInCode.Presentation.Repositories;
using Newtonsoft.Json;

namespace ActorsInCode.Presentation.Services;

public class WeatherForecastService : IWeatherForecastService
{
    private readonly IRedisRepository _redisRepository;
    private readonly ILogger<WeatherForecastService> _logger;

    public WeatherForecastService(IRedisRepository redisRepository, ILogger<WeatherForecastService> logger)
    {
        _redisRepository = redisRepository;
        _logger = logger;
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
        var persistToRedis = await _redisRepository.Add(weatherForecastRangeData.ToList());

        if (persistToRedis)
        {
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