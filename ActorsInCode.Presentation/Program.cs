using ActorsInCode.Presentation.Model.Options;
using ActorsInCode.Presentation.Services.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var service = builder.Services;
var config = builder.Configuration;

service.Configure<RedisConfiguration>(c => config.GetSection(nameof(RedisConfiguration)).Bind(c));
service.AddScoped<IRedisRepository, RedisRepository>();


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

app.Run();
