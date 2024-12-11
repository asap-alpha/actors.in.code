using ActorsInCode.Domain.Models.Response;
using Confluent.Kafka;

namespace ActorsInCode.Presentation.Services;

public interface IKafkaProducerService
{
    Task KafkaProducer(List<WeatherForecast> data);
}


  

