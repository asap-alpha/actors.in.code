using System.Text.Json.Serialization;
using ActorsInCode.Domain.Options;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace ActorsInCode.Consumer;

public class ActorsInCodeConsumer : BackgroundService
{
    private readonly ILogger<ActorsInCodeConsumer> _logger;
    private readonly IConsumer<string, string> _consumer;
    private readonly KafkaConsumerConfig _consumerConfig;


    public ActorsInCodeConsumer(ILogger<ActorsInCodeConsumer> logger, IOptions<KafkaConsumerConfig> consumerConfig)
    {
        _consumerConfig = consumerConfig.Value;
      
        var config = new ConsumerConfig()
        {
            BootstrapServers = _consumerConfig.BootstrapServers,
            GroupId = _consumerConfig.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        _logger = logger;
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
}