using Gateway.Dtos;
using Gateway.Mappers;
using Lab3.Contracts.Services.Product;

namespace Gateway.Clients;

public sealed class ProductServiceClient(ProductService.ProductServiceClient grpcClient)
{
    public async Task<ProductCreatedDto> CreateProductAsync(CreateProductDto dto, CancellationToken ct)
    {
        CreateProductRequest request = dto.ToGrpc();

        CreateProductResponse grpcProduct = await grpcClient.CreateProductAsync(request, cancellationToken: ct);

        ProductCreatedDto response = grpcProduct.ToDto();

        return response;
    }
}