using ActorsInCode.Presentation.Services.Repository;
using Microsoft.AspNetCore.Mvc;

namespace ActorsInCode.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly IRedisRepository _redisRepository;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IRedisRepository redisRepository)
        {
            _logger = logger;
            _redisRepository = redisRepository;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public  async Task<IEnumerable<WeatherForecast>> Get()
        {
            var weatherForecast = 
             Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

            var persistToRedis = await _redisRepository.Add(weatherForecast.ToList());

            if (persistToRedis)
            {
                return weatherForecast;
            }

            return new List<WeatherForecast>();
        }
    }
}
