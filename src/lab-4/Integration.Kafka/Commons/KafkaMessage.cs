using Confluent.Kafka;

namespace Integration.Kafka.Commons;

public sealed class KafkaMessage<TKey, TValue>
{
    private readonly IConsumer<TKey, TValue> _consumer;

    private readonly ConsumeResult<TKey, TValue> _consumeResult;

    internal KafkaMessage(IConsumer<TKey, TValue> consumer, ConsumeResult<TKey, TValue> consumeResult)
    {
        _consumer = consumer;
        _consumeResult = consumeResult;
    }

    public TKey Key => _consumeResult.Message.Key;

    public TValue Value => _consumeResult.Message.Value;

    public void Commit() => _consumer.Commit(_consumeResult);
}