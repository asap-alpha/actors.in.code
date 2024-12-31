using System.Reflection;
using System.Text.RegularExpressions;
using ActorsInCode.Domain.Options;
using ActorsInCode.Infrastructure.Actors;
using ActorsInCode.Infrastructure.Repositories;
using Akka.Actor;
using Akka.Hosting;
using Akka.Routing; 

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


    public static IServiceCollection AddActorSystems(this IServiceCollection services, Action<ActorConfig> config)
    {

        var actorSystemName = Regex.Replace(Assembly.GetExecutingAssembly().GetName().Name ?? "ActorSystemName",
            @"[^a-zA-Z\s]+", "", RegexOptions.None, TimeSpan.FromMilliseconds(100));
        
        ActorConfig actorConfig = new();
        config.Invoke(actorConfig);

        services.AddAkka(actorSystemName, builder =>
        {
            builder.WithActors((system, registry, resolver) =>
            {
                //define strategy
                var defaultStrategy = new OneForOneStrategy(
                    3, TimeSpan.FromSeconds(3), exception =>
                    {
                        if (exception is not ActorInitializationException)
                            return Directive.Resume;
                        system.Terminate().Wait(1000);

                        return Directive.Stop;
                    });
                
                //define strategy for roundRobin
                RouterConfig routerConfig = new RoundRobinPool(actorConfig.NumberOfInstances, new DefaultResizer(actorConfig.NumberOfInstances, actorConfig.UpperBound)); 
                
                // props mongoDBActor
                var persistToMongoActorProps = resolver
                    .Props<PersistMongodbActor>()
                    .WithSupervisorStrategy(defaultStrategy)
                    .WithRouter(routerConfig);
                
                //add actor to system
                var mongoDbActor = system.ActorOf( persistToMongoActorProps,nameof(PersistMongodbActor));
                
                 registry.Register<PersistMongodbActor>(mongoDbActor);

            });
        });
        
        return services;
    }
}