using ActorsInCode.Presentation.Repositories;
using ActorsInCode.Presentation.Services;
using Microsoft.AspNetCore.Mvc;

namespace ActorsInCode.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IWeatherForecastService _weatherForecastService;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(IWeatherForecastService weatherForecastService, ILogger<WeatherForecastController> logger)
        {
            _weatherForecastService = weatherForecastService;
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IActionResult> Get()
        {
            var result = await _weatherForecastService.GetWeatherData();
            if (!result.IsPersisted)
            {
                _logger.LogInformation("Failed to persist weather forecast data to redis");
                return BadRequest(result.IsPersisted);
            }
            
            return Ok(result);
        }
    }
}