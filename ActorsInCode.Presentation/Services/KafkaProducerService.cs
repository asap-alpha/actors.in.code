using ActorsInCode.Presentation.Model.Options;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ActorsInCode.Presentation.Services;

public class KafkaProducerService:IKafkaProducerService
{
    private readonly KafkaProducerConfig _kafkaProducerConfig;
    private readonly ILogger<KafkaProducerService> _logger;
    
    public KafkaProducerService(IOptions<KafkaProducerConfig> kafkaProducerConfig, ILogger<KafkaProducerService> logger)
    {
        _logger = logger;
        _kafkaProducerConfig = kafkaProducerConfig.Value;
    }
    public async Task KafkaProducer(List<WeatherForecast> data)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = _kafkaProducerConfig.BootstrapServers
        };

        using var producer = new ProducerBuilder<string, string>(config).Build();

        foreach (var message in data)
        {
            message.ExtraData = null;
            var payload = JsonConvert.SerializeObject(message);
            
            var deliveryReport = await producer.ProduceAsync(_kafkaProducerConfig.Topic, new Message<string, string>
            {
                Key = new Random().Next().ToString(), 
                Value = payload
            });
            
            _logger.LogDebug("delivery status {Status} for payload {Payload}", deliveryReport.Status, payload);
        }
 
    }
}