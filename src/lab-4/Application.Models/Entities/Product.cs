using Application.Models.Commons;
using Application.Models.Primitives.EntityIds;

namespace Application.Models.Entities;

public sealed class Product : Entity<ProductId>
{
    public string Name { get; }

    public decimal Price { get; }

    public Product(ProductId id, string name, decimal price)
        : base(id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfNegative(price, nameof(price));

        Name = name;
        Price = price;
    }
}