using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.Models.CreationParameters;
using Application.Models.Entities;

namespace Application.Services;

internal sealed class ProductService(IProductRepository productRepository) : IProductService
{
    public async Task<Product> CreateProductAsync(string name, decimal price, CancellationToken ct)
    {
        ProductCreationParameters productParameters = new(name, price);
        Product product = await productRepository.CreateProductAsync(productParameters, ct);
        return product;
    }
}