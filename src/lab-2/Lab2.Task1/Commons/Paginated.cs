namespace Lab2.Task1.Commons;

public sealed record Paginated<T>(IReadOnlyList<T> Items, string? PageToken = null);