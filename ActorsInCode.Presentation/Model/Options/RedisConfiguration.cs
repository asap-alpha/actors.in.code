namespace ActorsInCode.Presentation.Model.Options;

public class RedisConfiguration
{
    public string? RedisInstance { get; set; }
    public int RedisDb { get; set; }
    public int Ttl { get; set; }
}

 