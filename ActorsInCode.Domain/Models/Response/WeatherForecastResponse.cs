namespace ActorsInCode.Domain.Models.Response
{
    public class WeatherForecastResponse
    {
        public DateOnly Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }

        public ExtraData ExtraData { get; set; } = new ExtraData()
        {
            Duplicated = false
        };
    }

    public class ExtraData
    {
        public string RedisKey { get; set; }
        public bool Duplicated { get; set; }
    }
}