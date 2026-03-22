using Application.Contracts.Services;
using Grpc.Core;
using Lab3.Contracts.Services.Product;
using Presentation.Grpc.Commons;
using Presentation.Grpc.Mappers;
using Product = Application.Models.Entities.Product;

namespace Presentation.Grpc.Services;

internal sealed class ProductServiceGrpc : ProductService.ProductServiceBase
{
    private readonly IProductService _productService;

    private readonly IRequestValidator<CreateProductRequest> _createProductRequestValidator;

    public ProductServiceGrpc(IProductService productService, IRequestValidator<CreateProductRequest> createProductRequestValidator)
    {
        _productService = productService;
        _createProductRequestValidator = createProductRequestValidator;
    }

    public override async Task<CreateProductResponse> CreateProduct(CreateProductRequest request, ServerCallContext context)
    {
        _createProductRequestValidator.Validate(request);

        Product product = await _productService.CreateProductAsync(request.Name, request.Price.ToDecimal(), context.CancellationToken);

        return product.ToGrpc();
    }
}
