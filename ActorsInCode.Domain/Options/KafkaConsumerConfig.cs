namespace ActorsInCode.Domain.Options;

public class KafkaConsumerConfig
{
    public string? BootstrapServers { get; set; }
    public string? Topic { get; set; }
    public string? GroupId { get; set; }
}