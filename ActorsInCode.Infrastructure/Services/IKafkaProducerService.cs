using ActorsInCode.Domain.Models.Request;
using ActorsInCode.Domain.Models.Response;

namespace ActorsInCode.Infrastructure.Services;

public interface IKafkaProducerService
{
    Task KafkaProducer(List<WeatherForecastRequest> data);
}


  

