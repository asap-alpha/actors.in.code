using ActorsInCode.Presentation.Model.Options;
using ActorsInCode.Presentation.Repositories;
using ActorsInCode.Presentation.Services;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo
    .Console()
    .CreateLogger();

Log.Information("starting server.");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var service = builder.Services;
var config = builder.Configuration;


builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    loggerConfiguration.WriteTo.Console();
    loggerConfiguration.ReadFrom.Configuration(context.Configuration);
});
service.Configure<RedisConfiguration>(c => config.GetSection(nameof(RedisConfiguration)).Bind(c));
service.Configure<KafkaProducerConfig>(c => config.GetSection(nameof(KafkaProducerConfig)).Bind(c));
service.AddScoped<IWeatherForecastService, WeatherForecastService>();
service.AddScoped<IRedisRepository, RedisRepository>();
service.AddScoped<IKafkaProducerService, KafkaProducerService>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
