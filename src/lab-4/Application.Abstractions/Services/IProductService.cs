using Application.Models.Entities;

namespace Application.Abstractions.Services;

public interface IProductService
{
    Task<Product> CreateProductAsync(string name, decimal price, CancellationToken ct);
}
