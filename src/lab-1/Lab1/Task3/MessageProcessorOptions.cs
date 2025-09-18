namespace Lab1.Task3;

public sealed record MessageProcessorOptions(int ChannelCapacity, int BatchCount, TimeSpan BatchTimeout);