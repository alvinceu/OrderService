namespace Application.Models.CreationParameters;

public sealed record ProductCreationParameters
{
    public string Name { get; }

    public decimal Price { get; }

    public ProductCreationParameters(string name, decimal price)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfNegative(price, nameof(price));

        Name = name;
        Price = price;
    }
}