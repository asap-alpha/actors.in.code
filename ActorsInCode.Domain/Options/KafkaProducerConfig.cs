namespace ActorsInCode.Domain.Options;

public class KafkaProducerConfig
{
    public string? BootstrapServers { get; set; }
    public string? Topic { get; set; }
}