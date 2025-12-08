namespace Integration.Kafka.Configurations;

internal sealed record BatchingKafkaMessageHandlerOptions
{
    public required int BatchSize { get; set; }

    public required int TimeoutSeconds { get; set; }
}