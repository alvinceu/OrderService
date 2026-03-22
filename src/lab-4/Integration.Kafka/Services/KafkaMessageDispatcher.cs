using Integration.Kafka.Commons;
using Integration.Kafka.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Threading.Channels;

namespace Integration.Kafka.Services;

internal sealed class KafkaMessageDispatcher<TKey, TValue> : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    private readonly KafkaBatchingBoundedChannelOptions _kafkaBatchingBoundedChannelOptions;

    public KafkaMessageDispatcher(
        IServiceScopeFactory scopeFactory,
        IOptions<KafkaBatchingBoundedChannelOptions> kafkaBatchingBoundedChannelOptions)
    {
        _kafkaBatchingBoundedChannelOptions = kafkaBatchingBoundedChannelOptions.Value;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var channelOptions = new BoundedChannelOptions(_kafkaBatchingBoundedChannelOptions.BoundedCapacity)
        {
            SingleReader = true,
            SingleWriter = true,
            FullMode = BoundedChannelFullMode.Wait,
        };

        var channel = Channel.CreateBounded<KafkaMessage<TKey, TValue>>(channelOptions);

        await using AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();

        BatchingKafkaMessageHandler<TKey, TValue> kafkaMessageHandlerBase =
            scope
                .ServiceProvider
                .GetRequiredService<BatchingKafkaMessageHandler<TKey, TValue>>();

        KafkaMessageIngestWorker<TKey, TValue> kafkaMessageIngestWorker =
            scope
                .ServiceProvider
                .GetRequiredService<KafkaMessageIngestWorker<TKey, TValue>>();

        await Task.WhenAll(
            kafkaMessageIngestWorker.PumpAsync(channel.Writer, stoppingToken),
            kafkaMessageHandlerBase.HandleAsync(channel.Reader, stoppingToken));
    }
}