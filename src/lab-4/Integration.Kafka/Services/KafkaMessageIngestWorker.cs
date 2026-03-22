using Confluent.Kafka;
using Integration.Kafka.Commons;
using Integration.Kafka.Configurations;
using Microsoft.Extensions.Options;
using System.Threading.Channels;

namespace Integration.Kafka.Services;

internal sealed class KafkaMessageIngestWorker<TKey, TValue>
{
    private readonly KafkaMessageConsumerConfigOptions _kafkaConsumerConfigOptions;

    private readonly IDeserializer<TKey> _keyDeserializer;

    private readonly IDeserializer<TValue> _valueDeserializer;

    public KafkaMessageIngestWorker(
        IDeserializer<TKey> keyDeserializer,
        IDeserializer<TValue> valueDeserializer,
        IOptions<KafkaMessageConsumerConfigOptions> options)
    {
        _keyDeserializer = keyDeserializer;
        _valueDeserializer = valueDeserializer;
        _kafkaConsumerConfigOptions = options.Value;
    }

    public async Task PumpAsync(ChannelWriter<KafkaMessage<TKey, TValue>> writer, CancellationToken ct)
    {
        await Task.Yield();

        var config = new ConsumerConfig
        {
            BootstrapServers = _kafkaConsumerConfigOptions.Host,
            GroupId = _kafkaConsumerConfigOptions.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
        };

        using IConsumer<TKey, TValue> consumer = new ConsumerBuilder<TKey, TValue>(config)
            .SetKeyDeserializer(_keyDeserializer)
            .SetValueDeserializer(_valueDeserializer)
            .Build();

        consumer.Subscribe(_kafkaConsumerConfigOptions.Topic);

        try
        {
            while (ct.IsCancellationRequested is false)
            {
                ConsumeResult<TKey, TValue> consumerResult = consumer.Consume(ct);

                await writer.WriteAsync(new KafkaMessage<TKey, TValue>(consumer, consumerResult), ct);
            }
        }
        finally
        {
            consumer.Close();
        }
    }
}
