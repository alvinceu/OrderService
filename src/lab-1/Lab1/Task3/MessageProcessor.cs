using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace Lab1.Task3;

public class MessageProcessor(IMessageHandler handler, MessageProcessorOptions options) : IMessageSender, IMessageProcessor
{
    private readonly Channel<Message> _channel = Channel.CreateBounded<Message>(new BoundedChannelOptions(options.ChannelCapacity)
    {
        FullMode = BoundedChannelFullMode.DropOldest,
        SingleReader = true,
    });

    public async ValueTask SendAsync(Message message, CancellationToken cancellationToken)
    {
        await _channel.Writer.WriteAsync(message, cancellationToken);
    }

    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
        ConfiguredCancelableAsyncEnumerable<IReadOnlyList<Message>> messages = _channel.Reader.ReadAllAsync(cancellationToken)
            .ChunkAsync(options.BatchCount, options.BatchTimeout)
            .WithCancellation(cancellationToken);

        await foreach (IReadOnlyList<Message> message in messages)
        {
            await handler.HandleAsync(message, cancellationToken).ConfigureAwait(false);
        }
    }

    public void Complete()
    {
        _channel.Writer.Complete();
    }
}