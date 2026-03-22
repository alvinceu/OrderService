namespace Integration.Kafka.Commons;

public interface IKafkaMessageHandler<TKey, TValue>
{
    Task HandleAsync(IReadOnlyList<KafkaMessage<TKey, TValue>> messages, CancellationToken ct);
}
