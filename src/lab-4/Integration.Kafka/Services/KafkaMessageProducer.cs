using Confluent.Kafka;
using Integration.Kafka.Commons;
using Integration.Kafka.Configurations;
using Microsoft.Extensions.Options;

namespace Integration.Kafka.Services;

internal sealed class KafkaMessageProducer<TKey, TValue> : IKafkaMessageProducer<TKey, TValue>, IDisposable
{
    private readonly KafkaProducerOptions _producerOptions;
    private readonly IProducer<TKey, TValue> _producer;

    public KafkaMessageProducer(
        IOptions<KafkaProducerOptions> producerOptions,
        ISerializer<TKey> keySerializer,
        ISerializer<TValue> valueSerializer)
    {
        _producerOptions = producerOptions.Value;

        var config = new ProducerConfig
        {
            BootstrapServers = _producerOptions.BootstrapServers,
        };

        _producer = new ProducerBuilder<TKey, TValue>(config)
            .SetKeySerializer(keySerializer)
            .SetValueSerializer(valueSerializer)
            .Build();
    }

    public async Task ProduceAsync(Message<TKey, TValue> message, CancellationToken cancellationToken)
    {
        await _producer.ProduceAsync(_producerOptions.Topic, message, cancellationToken);
    }

    public void Dispose()
    {
        _producer.Dispose();
    }
}