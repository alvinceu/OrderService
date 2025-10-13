namespace Lab2.Task2;

internal sealed record TimerOptions
{
    public const string SectionName = "TimerSettings";

    public int IntervalSeconds { get; set; }
}