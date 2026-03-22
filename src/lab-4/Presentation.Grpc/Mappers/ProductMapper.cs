using Lab3.Contracts.Services.Product;
using DomainProduct = Application.Models.Entities.Product;

namespace Presentation.Grpc.Mappers;

internal static class ProductMapper
{
    public static CreateProductResponse ToGrpc(this DomainProduct product)
    {
        return new CreateProductResponse
        {
            Product = new Product
            {
                ProductId = product.Id.Value,
                Name = product.Name,
                Price = DecimalMapper.ToString(product.Price),
            },
        };
    }
}