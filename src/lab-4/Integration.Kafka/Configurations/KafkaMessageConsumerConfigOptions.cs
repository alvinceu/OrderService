namespace Integration.Kafka.Configurations;

internal sealed record KafkaMessageConsumerConfigOptions
{
    public required string Host { get; set; }

    public required string GroupId { get; set; }

    public required string Topic { get; set; }
}
