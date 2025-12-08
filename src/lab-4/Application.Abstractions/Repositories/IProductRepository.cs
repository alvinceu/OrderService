using Application.Models.Commons.Paginations;
using Application.Models.CreationParameters;
using Application.Models.Entities;
using Application.Models.Primitives.EntityIds;
using Application.Models.SearchingFilters;

namespace Application.Abstractions.Repositories;

public interface IProductRepository
{
    Task<Product> CreateProductAsync(ProductCreationParameters parameters, CancellationToken ct);

    IAsyncEnumerable<Product> FindProductsAsync(
        ProductSearchingFilters filters,
        PaginationInfo<ProductId> paginationInfo,
        CancellationToken ct);
}