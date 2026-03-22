using Application.Abstractions.Repositories;
using Application.Models.Commons.Paginations;
using Application.Models.CreationParameters;
using Application.Models.Entities;
using Application.Models.Primitives.EntityIds;
using Application.Models.SearchingFilters;
using Npgsql;
using NpgsqlTypes;
using System.Runtime.CompilerServices;

namespace Infrastructure.Persistence.Repositories;

internal sealed class ProductRepository(NpgsqlDataSource npgsqlDataSource) : IProductRepository
{
    public async Task<Product> CreateProductAsync(ProductCreationParameters parameters, CancellationToken ct)
    {
        const string sql =
            """
            insert into products 
                (product_name, product_price)
            values 
                (:name, :price)
            returning 
                product_id, product_name, product_price
            """;

        await using NpgsqlConnection connection = await npgsqlDataSource.OpenConnectionAsync(ct);

        await using NpgsqlCommand command = connection.CreateCommand();

        command.CommandText = sql;

        command
            .Parameters
            .Add(":name", NpgsqlDbType.Text);

        command
            .Parameters
            .Add(":price", NpgsqlDbType.Money);

        await command.PrepareAsync(ct);

        command.Parameters[":name"].Value = parameters.Name;
        command.Parameters[":price"].Value = parameters.Price;

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(ct);

        if (!await reader.ReadAsync(ct))
            throw new InvalidOperationException("failed to insert product");

        Product product = MapToProduct(reader);

        return product;
    }

    public async IAsyncEnumerable<Product> FindProductsAsync(
        ProductSearchingFilters filters,
        PaginationInfo<ProductId> paginationInfo,
        [EnumeratorCancellation] CancellationToken ct)
    {
        const string sql =
            """
            select 
                product_id, product_name, product_price
            from 
                products
            where 
                (:page_token is null or product_id > :page_token)
                and (cardinality(:ids) = 0 or product_id = any(:ids))
                and (:min_price is null or :min_price <= product_price)
                and (:max_price is null or :max_price >= product_price)
                and (:product_sub_name is null or product_name like '%' || :product_sub_name || '%')
            order by product_id
            limit :page_size
            """;

        await using NpgsqlConnection connection = await npgsqlDataSource.OpenConnectionAsync(ct);

        await using NpgsqlCommand command = connection.CreateCommand();

        command.CommandText = sql;

        command
            .Parameters
            .Add(":page_token", NpgsqlDbType.Bigint);

        command
            .Parameters
            .Add(":page_size", NpgsqlDbType.Integer);

        command
            .Parameters
            .Add(":ids", NpgsqlDbType.Array | NpgsqlDbType.Bigint);

        command
            .Parameters
            .Add(":min_price", NpgsqlDbType.Money);

        command
            .Parameters
            .Add(":max_price", NpgsqlDbType.Money);

        command
            .Parameters
            .Add(":product_sub_name", NpgsqlDbType.Text);

        await command.PrepareAsync(ct);

        command.Parameters[":page_token"].Value =
            paginationInfo.PageToken?.Value is { } value
                ? value
                : DBNull.Value;

        command.Parameters[":page_size"].Value = paginationInfo.PageSize;

        command.Parameters[":ids"].Value =
            filters.Ids.Select(x => x.Value).ToArray();

        command.Parameters[":min_price"].Value =
            filters.MinPrice.HasValue
                ? filters.MinPrice.Value
                : DBNull.Value;

        command.Parameters[":max_price"].Value =
            filters.MaxPrice.HasValue
                ? filters.MaxPrice.Value
                : DBNull.Value;

        command.Parameters[":product_sub_name"].Value =
            string.IsNullOrWhiteSpace(filters.NameSubstring)
                ? DBNull.Value
                : filters.NameSubstring;

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(ct);

        while (await reader.ReadAsync(ct))
        {
            yield return MapToProduct(reader);
        }
    }

    private static Product MapToProduct(NpgsqlDataReader reader)
    {
        int idOrdinal = reader.GetOrdinal("product_id");
        int nameOrdinal = reader.GetOrdinal("product_name");
        int priceOrdinal = reader.GetOrdinal("product_price");

        var productId = ProductId.Create(reader.GetInt64(idOrdinal));

        Product product = new(
            productId,
            reader.GetString(nameOrdinal),
            reader.GetDecimal(priceOrdinal));

        return product;
    }
}