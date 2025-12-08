using Application.Models.Entities;

namespace Application.Contracts.Services;

public interface IProductService
{
    Task<Product> CreateProductAsync(string name, decimal price, CancellationToken ct);
}
