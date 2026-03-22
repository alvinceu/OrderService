using Integration.Kafka.Commons;

namespace Integration.Kafka.Services;

internal sealed class AtLeastOnceWrappedHandler<TKey, TValue> : IKafkaMessageHandler<TKey, TValue>
{
    private readonly IKafkaMessageHandler<TKey, TValue> _innerHandler;

    public AtLeastOnceWrappedHandler(IKafkaMessageHandler<TKey, TValue> innerHandler)
    {
        _innerHandler = innerHandler;
    }

    public async Task HandleAsync(IReadOnlyList<KafkaMessage<TKey, TValue>> messageBuffer, CancellationToken ct)
    {
        await _innerHandler.HandleAsync(messageBuffer, ct);

        foreach (KafkaMessage<TKey, TValue> message in messageBuffer)
        {
            message.Commit();
        }
    }
}