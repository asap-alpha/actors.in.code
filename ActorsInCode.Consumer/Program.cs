using ActorsInCode.Consumer;
using ActorsInCode.Domain.Options;
using Serilog;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, service) =>
    {
       
        service.RegisterServiceConfiguration(context.Configuration);
        service.RegisterServiceCollection();
    })
    .UseSerilog((context,loggerConfiguration) =>
    {
        loggerConfiguration.WriteTo.Console();
        loggerConfiguration.ReadFrom.Configuration(context.Configuration);
    }).Build();

await builder.RunAsync();