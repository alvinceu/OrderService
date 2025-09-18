using System.Text;

namespace Lab1.Task3;

public class ConsoleMessageHandler : IMessageHandler
{
    public ValueTask HandleAsync(IEnumerable<Message> messages, CancellationToken cancellationToken)
    {
        var builder = new StringBuilder();
        foreach (Message message in messages)
        {
            builder.AppendLine($"Title: {message.Title}\tText: {message.Text}\n");
        }

        Console.WriteLine(builder.ToString());

        return ValueTask.CompletedTask;
    }
}