using Lab1.Task3;

var options = new MessageProcessorOptions(
    ChannelCapacity: 1000,
    BatchCount: 10,
    BatchTimeout: TimeSpan.FromSeconds(5));

var implementation = new MessageProcessor(new ConsoleMessageHandler(), options);

MessageProcessor processor = implementation;
MessageProcessor sender = implementation;

using var cts = new CancellationTokenSource();

Task t = processor.ProcessAsync(cts.Token);

await Parallel.ForEachAsync(
    Enumerable.Range(1, 100_000),
    cancellationToken: cts.Token,
    async (x, ct) =>
    {
        string messageText = (x % 3, x % 5) switch
        {
            (0, 0) => "FizzBuzz",
            (0, _) => "Fizz",
            (_, 0) => "Buzz",
            _ => $"{x}",
        };
        await Task.Delay(50, ct);
        await sender.SendAsync(new Message($"{x}", messageText), ct);
    });

processor.Complete();
await t;