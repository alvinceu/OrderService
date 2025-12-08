using Gateway.Dtos;
using Lab3.Contracts.Services.Product;
using CreateProductResponseGrpc = Lab3.Contracts.Services.Product.CreateProductResponse;

namespace Gateway.Mappers;

internal static class ProductMapper
{
    public static CreateProductRequest ToGrpc(this CreateProductDto dto)
    {
        return new CreateProductRequest
        {
            Name = dto.Name,
            Price = dto.Price,
        };
    }

    public static ProductCreatedDto ToDto(this CreateProductResponseGrpc grpc)
    {
        return new ProductCreatedDto
        {
            ProductId = grpc.Product.ProductId,
            Name = grpc.Product.Name,
            Price = grpc.Product.Price,
        };
    }
}