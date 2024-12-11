using Confluent.Kafka;

namespace ActorsInCode.Domain.Models.Response;

public class DeliveryResponse
{
    public bool IsPersisted { get; set; }
    public DeliveryResult<string, string>? DeliveryResult { get; set; }
    
}