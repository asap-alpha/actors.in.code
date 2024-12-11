namespace ActorsInCode.Presentation.Model.Options;

public class KafkaProducerConfig
{
    public string BootstrapServers { get; set; }
    public string Topic { get; set; }
}