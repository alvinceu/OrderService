namespace Lab1.Task3;

public interface IMessageProcessor
{
    Task ProcessAsync(CancellationToken cancellationToken);

    void Complete();
}