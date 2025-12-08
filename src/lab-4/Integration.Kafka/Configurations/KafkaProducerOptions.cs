namespace Integration.Kafka.Configurations;

internal sealed record KafkaProducerOptions
{
    public required string BootstrapServers { get; set; }

    public required string Topic { get; set; }
}
