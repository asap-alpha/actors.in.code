using System.Text.Json.Serialization;
using ActorsInCode.Domain.Models.Response;
using ActorsInCode.Domain.Options;
using ActorsInCode.Infrastructure.Actors;
using ActorsInCode.Infrastructure.Repositories;
using Akka.Actor;
using Akka.Hosting;
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
    private IRequiredActor<PersistMongodbActor> _requiredActor;


    public ActorsInCodeConsumer(ILogger<ActorsInCodeConsumer> logger, IOptions<KafkaConsumerConfig> consumerConfig,
        IRequiredActor<PersistMongodbActor> requiredActor)
    {
        _consumerConfig = consumerConfig.Value;

        var config = new ConsumerConfig()
        {
            BootstrapServers = _consumerConfig.BootstrapServers,
            GroupId = _consumerConfig.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        _logger = logger;
        _requiredActor = requiredActor;
        _consumer = new ConsumerBuilder<string, string>(config).Build();
        _consumer.Subscribe(_consumerConfig.Topic);
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var consumerResult = _consumer.Consume(stoppingToken);
                if (consumerResult != null)
                {
                    var payload = JsonConvert.DeserializeObject<WeatherForecastResponse>(consumerResult.Message.Value);
                    payload = payload.Adapt<WeatherForecastResponse>();
                    _logger.LogDebug("payload {Payload}", payload);

                    IActorRef mongoDbActorRef = await _requiredActor.GetAsync(stoppingToken);
                    mongoDbActorRef.Tell(payload);
                }

                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogDebug(e, "exception occured {Trace}", e.StackTrace);
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping service...");
        await base.StopAsync(cancellationToken);
    }
}