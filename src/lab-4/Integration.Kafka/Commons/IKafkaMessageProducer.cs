using Confluent.Kafka;

namespace Integration.Kafka.Commons;

public interface IKafkaMessageProducer<TKey, TValue>
{
    Task ProduceAsync(Message<TKey, TValue> message, CancellationToken cancellationToken);
}