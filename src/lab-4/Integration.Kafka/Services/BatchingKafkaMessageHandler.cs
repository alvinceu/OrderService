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

        // Проблема?! мы не контролируем время и то, что размер batcha уже записан, чтобы его забрать от туда?
        // var streamOfMessageBatches = reader.ReadAllAsync(...).Chunk(batchSize...) и т.д. (chunk linq.async)
        // _logger.LogWarning("Start get chunk");

        // IAsyncEnumerable<KafkaMessage<TKey, TValue>[]> streamOfMessageBatches =
        //     reader
        //         .ReadAllAsync(ct).Chunk(_batchingKafkaMessageHandlerOptions.BatchSize);
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
        // не читаем если ничего нет
        while (await reader.WaitToReadAsync(ct))
        {
            var batch = new List<KafkaMessage<TKey, TValue>>(batchSize);

            var windowCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            windowCts.CancelAfter(window);

            // читаем пока есть время или пока не заполниться batch
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

            // нет смысла отправлять пустой батч
            if (batch.Count == 0)
            {
                continue;
            }

            yield return batch;
        }
    }
}
