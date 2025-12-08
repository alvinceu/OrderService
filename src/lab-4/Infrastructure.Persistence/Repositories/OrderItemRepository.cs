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

internal sealed class OrderItemRepository(NpgsqlDataSource npgsqlDataSource) : IOrderItemRepository
{
    public async Task<OrderItem> CreateOrderItemAsync(OrderItemCreationParameters parameters, CancellationToken ct)
    {
        const string sql =
            """
            insert into order_items
                (order_id, product_id, order_item_quantity, order_item_deleted)
            values 
                (:value_order_id, :value_product_id, :value_order_item_quantity, :value_order_item_deleted)
            returning 
                order_item_id, order_id, product_id, order_item_quantity, order_item_deleted
            """;

        await using NpgsqlConnection connection = await npgsqlDataSource.OpenConnectionAsync(ct);

        await using NpgsqlCommand command = connection.CreateCommand();

        command.CommandText = sql;

        command
            .Parameters
            .Add(":value_order_id", NpgsqlDbType.Bigint);

        command
            .Parameters
            .Add(":value_product_id", NpgsqlDbType.Bigint);

        command
            .Parameters
            .Add(":value_order_item_quantity", NpgsqlDbType.Integer);

        command
            .Parameters
            .Add(":value_order_item_deleted", NpgsqlDbType.Boolean);

        await command.PrepareAsync(ct);

        command.Parameters[":value_order_id"].Value = parameters.OrderId.Value;
        command.Parameters[":value_product_id"].Value = parameters.ProductId.Value;
        command.Parameters[":value_order_item_quantity"].Value = parameters.Quantity;
        command.Parameters[":value_order_item_deleted"].Value = parameters.IsDeleted;

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(ct);

        if (!await reader.ReadAsync(ct))
            throw new InvalidOperationException("failed to insert order item");

        OrderItem orderItem = MapToOrderItem(reader);

        return orderItem;
    }

    public async Task SoftDeleteOrderItemAsync(OrderItem orderItem, CancellationToken ct)
    {
        const string sql =
            """
            update order_items
            set order_item_deleted = true
            where order_item_id = :value_order_item_id
            """;

        await using NpgsqlConnection connection = await npgsqlDataSource.OpenConnectionAsync(ct);

        await using NpgsqlCommand command = connection.CreateCommand();

        command.CommandText = sql;

        command
            .Parameters
            .Add(":value_order_item_id", NpgsqlDbType.Bigint);

        await command.PrepareAsync(ct);

        command.Parameters[":value_order_item_id"].Value = orderItem.Id.Value;

        await command.ExecuteNonQueryAsync(ct);
    }

    public async Task<OrderItem?> FindOrderItemByIdAsync(OrderItemId orderItemId, CancellationToken ct)
    {
        const string sql =
            """
            select 
               order_item_id, order_id, product_id, order_item_quantity, order_item_deleted
            from
               order_items
            where 
                 order_item_id = :value_order_item_id
            """;

        await using NpgsqlConnection connection = await npgsqlDataSource.OpenConnectionAsync(ct);

        await using NpgsqlCommand command = connection.CreateCommand();

        command.CommandText = sql;

        command
            .Parameters
            .Add(":value_order_item_id", NpgsqlDbType.Bigint);

        await command.PrepareAsync(ct);

        command.Parameters[":value_order_item_id"].Value = orderItemId.Value;

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(ct);

        if (!await reader.ReadAsync(ct))
            return null;

        return MapToOrderItem(reader);
    }

    public async IAsyncEnumerable<OrderItem> FindOrderItemsAsync(
        OrderItemSearchingFilters filters,
        PaginationInfo<OrderItemId> paginationInfo,
        [EnumeratorCancellation] CancellationToken ct)
    {
        const string sql =
            """
            select 
                order_item_id, order_id, product_id, order_item_quantity, order_item_deleted
            from
                order_items
            where 
                (:page_token is null or order_item_id > :page_token)
                and (cardinality(:order_ids) = 0 or order_id = any(:order_ids))
                and (cardinality(:product_ids) = 0 or product_id = any(:product_ids))
                and (:value_order_item_deleted is null or order_item_deleted = :value_order_item_deleted)
            order by order_item_id
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
            .Add(":product_ids", NpgsqlDbType.Array | NpgsqlDbType.Bigint);

        command
            .Parameters
            .Add(":order_ids", NpgsqlDbType.Array | NpgsqlDbType.Bigint);

        command
            .Parameters
            .Add(":value_order_item_deleted", NpgsqlDbType.Boolean);

        await command.PrepareAsync(ct);

        command.Parameters[":page_token"].Value =
            paginationInfo.PageToken?.Value is { } value
                ? value
                : DBNull.Value;

        command.Parameters[":page_size"].Value = paginationInfo.PageSize;

        command.Parameters[":product_ids"].Value =
            filters.ProductIds.Select(x => x.Value).ToArray();

        command.Parameters[":order_ids"].Value =
            filters.OrderIds.Select(x => x.Value).ToArray();

        command.Parameters[":value_order_item_deleted"].Value =
            filters.IsDeleted is null
                ? DBNull.Value
                : filters.IsDeleted;

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(ct);

        while (await reader.ReadAsync(ct))
        {
            yield return MapToOrderItem(reader);
        }
    }

    private static OrderItem MapToOrderItem(NpgsqlDataReader reader)
    {
        int idOrdinal = reader.GetOrdinal("order_item_id");
        int orderIdOrdinal = reader.GetOrdinal("order_id");
        int productIdOrdinal = reader.GetOrdinal("product_id");
        int quantityOrdinal = reader.GetOrdinal("order_item_quantity");
        int isDeletedOrdinal = reader.GetOrdinal("order_item_deleted");

        var orderItemId = OrderItemId.Create(reader.GetInt64(idOrdinal));
        var orderId = OrderId.Create(reader.GetInt64(orderIdOrdinal));
        var productId = ProductId.Create(reader.GetInt64(productIdOrdinal));

        var orderItem = new OrderItem(
            orderItemId,
            orderId,
            productId,
            reader.GetInt32(quantityOrdinal),
            reader.GetBoolean(isDeletedOrdinal));

        return orderItem;
    }
}