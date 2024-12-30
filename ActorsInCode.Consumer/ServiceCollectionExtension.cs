using ActorsInCode.Domain.Options;
using ActorsInCode.Infrastructure.Repositories;

namespace ActorsInCode.Consumer;

public static class ServiceCollectionExtension
{
    public static IServiceCollection RegisterServiceConfiguration(this IServiceCollection service, IConfiguration config)
    {
        service.Configure<MongoClientConfig>(c => config.GetSection(nameof(MongoClientConfig)).Bind(c));
        service.Configure<KafkaConsumerConfig>(config.GetSection(nameof(KafkaConsumerConfig)));
        return service;
    }

    public static IServiceCollection RegisterServiceCollection(this IServiceCollection service)
    {
        service.AddSingleton<IMongoDbRepository, MongoDbRepository>();
        service.AddHostedService<ActorsInCodeConsumer>();
        return service;
    }
    
}