using ActorsInCode.Consumer;
using ActorsInCode.Domain.Options;
using Serilog;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, service) =>
    {
        service.AddHostedService<ActorsInCodeConsumer>();
        service.Configure<KafkaConsumerConfig>(context.Configuration.GetSection(nameof(KafkaConsumerConfig)));
        service.RegisterServiceConfiguration(context.Configuration);
        service.RegisterServiceCollection();
    })
    .UseSerilog((context,loggerConfiguration) =>
    {
        loggerConfiguration.WriteTo.Console();
        loggerConfiguration.ReadFrom.Configuration(context.Configuration);
    }).Build();

// builder.Host. UseSerilog((context, loggerConfiguration) =>
// {
//     loggerConfiguration.WriteTo.Console();
//     loggerConfiguration.ReadFrom.Configuration(context.Configuration);
// });
// var service = builder.ConfigureServices();
// var config = builder.Configuration;
// service.AddHostedService<ActorsInCodeConsumer>();
// service.Configure<KafkaConsumerConfig>(c => config.GetSection(nameof(KafkaConsumerConfig)).Bind(c));
//
//
// var host = builder.Build();
await builder.RunAsync();