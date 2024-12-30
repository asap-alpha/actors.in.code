using System.Text.Json.Serialization;
using ActorsInCode.Domain.Models.Response;
using ActorsInCode.Domain.Options;
using ActorsInCode.Infrastructure.Repositories;
using Confluent.Kafka;
using Mapster;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ActorsInCode.Consumer;

public class ActorsInCodeConsumer : BackgroundService
{
    private readonly ILogger<ActorsInCodeConsumer> _logger;
    private readonly IConsumer<string, string> _consumer;
    private readonly KafkaConsumerConfig _consumerConfig;
    private readonly IMongoDbRepository _mongoDbRepository;


    public ActorsInCodeConsumer(ILogger<ActorsInCodeConsumer> logger, IOptions<KafkaConsumerConfig> consumerConfig, IMongoDbRepository mongoDbRepository)
    {
        _consumerConfig = consumerConfig.Value;
      
        var config = new ConsumerConfig()
        {
            BootstrapServers = _consumerConfig.BootstrapServers,
            GroupId = _consumerConfig.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        _logger = logger;
        _mongoDbRepository = mongoDbRepository;
        _consumer = new ConsumerBuilder<string, string>(config).Build();
        _consumer.Subscribe(_consumerConfig.Topic);
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
   
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var consumerResult =  _consumer.Consume(stoppingToken);
                if (consumerResult != null)
                {
                    _logger.LogDebug("consumer data {Data}", consumerResult.Message.Value);
                    var payload = JsonConvert.DeserializeObject<WeatherForecastResponse>(consumerResult.Message.Value);
                    payload = payload.Adapt<WeatherForecastResponse>();
                    _logger.LogDebug("payload {Payload}",payload );
                   await _mongoDbRepository.SaveResult(payload, stoppingToken);
                }

            }

            await Task.WhenAll();
        }
        catch (Exception e)
        {
            _logger.LogDebug(e,"exception occured {Trace}", e.StackTrace);
            throw;
        }
    }
    
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping service...");
        await base.StopAsync(cancellationToken);
    }
}