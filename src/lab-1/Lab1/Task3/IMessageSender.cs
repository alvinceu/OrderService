namespace Lab1.Task3;

public interface IMessageSender
{
    ValueTask SendAsync(Message message, CancellationToken cancellationToken);
}