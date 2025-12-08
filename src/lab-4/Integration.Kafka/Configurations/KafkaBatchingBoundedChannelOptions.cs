namespace Integration.Kafka.Configurations;

internal sealed record KafkaBatchingBoundedChannelOptions
{
    public required int BoundedCapacity { get; set; }
}
