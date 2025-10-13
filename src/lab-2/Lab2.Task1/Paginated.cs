namespace Lab2.Task1;

public sealed record Paginated<T>(IReadOnlyList<T> Items, string? PageToken = null);