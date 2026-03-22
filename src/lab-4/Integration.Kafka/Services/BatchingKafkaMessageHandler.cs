using Integration.Kafka.Commons;
using Integration.Kafka.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace Integration.Kafka.Services;

internal sealed class BatchingKafkaMessageHandler<TKey, TValue>
{
    private readonly IServiceScopeFactory _scopeFactory;

    private readonly BatchingKafkaMessageHandlerOptions _batchingKafkaMessageHandlerOptions;

    public BatchingKafkaMessageHandler(
        IServiceScopeFactory serviceScopeFactory,
        IOptions<BatchingKafkaMessageHandlerOptions> options)
    {
        _scopeFactory = serviceScopeFactory;
        _batchingKafkaMessageHandlerOptions = options.Value;
    }

    public async Task HandleAsync(ChannelReader<KafkaMessage<TKey, TValue>> reader, CancellationToken ct)
    {
        await Task.Yield();

        var window = TimeSpan.FromSeconds(_batchingKafkaMessageHandlerOptions.TimeoutSeconds);

        IAsyncEnumerable<IReadOnlyList<KafkaMessage<TKey, TValue>>> streamOfMessageBatches =
            ReadBatchAsync(reader, _batchingKafkaMessageHandlerOptions.BatchSize, window, ct);

        await foreach (IReadOnlyList<KafkaMessage<TKey, TValue>> batch in streamOfMessageBatches)
        {
            await using AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();

            IKafkaMessageHandler<TKey, TValue> handler =
                scope
                    .ServiceProvider
                    .GetRequiredService<IKafkaMessageHandler<TKey, TValue>>();

            await handler.HandleAsync(batch, ct);
        }
    }

    private async IAsyncEnumerable<IReadOnlyList<KafkaMessage<TKey, TValue>>> ReadBatchAsync(
        ChannelReader<KafkaMessage<TKey, TValue>> reader,
        int batchSize,
        TimeSpan window,
        [EnumeratorCancellation] CancellationToken ct)
    {
        while (await reader.WaitToReadAsync(ct))
        {
            var batch = new List<KafkaMessage<TKey, TValue>>(batchSize);

            var windowCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            windowCts.CancelAfter(window);

            while (batch.Count < batchSize && windowCts.Token.IsCancellationRequested is false)
            {
                try
                {
                    KafkaMessage<TKey, TValue> message = await reader.ReadAsync(windowCts.Token);
                    batch.Add(message);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }

            if (batch.Count == 0)
            {
                continue;
            }

            yield return batch;
        }
    }
}
