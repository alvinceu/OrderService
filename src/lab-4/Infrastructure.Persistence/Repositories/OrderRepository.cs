using Application.Abstractions.Repositories;
using Application.Models.Commons.Paginations;
using Application.Models.CreationParameters;
using Application.Models.Entities;
using Application.Models.Primitives.EntityIds;
using Application.Models.Primitives.Enums;
using Application.Models.SearchingFilters;
using Npgsql;
using NpgsqlTypes;
using System.Runtime.CompilerServices;

namespace Infrastructure.Persistence.Repositories;

internal sealed class OrderRepository(NpgsqlDataSource npgsqlDataSource) : IOrderRepository
{
    public async Task<Order> CreateOrderAsync(OrderCreationParameters parameters, CancellationToken ct)
    {
        const string sql =
            """
            insert into orders 
                (order_state, order_created_at, order_created_by)
            values 
                (:value_state, :value_created_at, :value_created_by)
            returning 
                order_id, order_state, order_created_at, order_created_by
            """;

        await using NpgsqlConnection connection = await npgsqlDataSource.OpenConnectionAsync(ct);

        await using NpgsqlCommand command = connection.CreateCommand();

        command.CommandText = sql;

        command
            .Parameters
            .Add(new NpgsqlParameter
            {
                ParameterName = ":value_state",
                DataTypeName = "order_state",
            });

        command
            .Parameters
            .Add(":value_created_at", NpgsqlDbType.TimestampTz);

        command
            .Parameters
            .Add(":value_created_by", NpgsqlDbType.Text);

        await command.PrepareAsync(ct);

        command.Parameters[":value_state"].Value = parameters.State;
        command.Parameters[":value_created_at"].Value = parameters.CreatedAt;
        command.Parameters[":value_created_by"].Value = parameters.CreatedBy;

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(ct);

        if (!await reader.ReadAsync(ct))
            throw new InvalidOperationException("failed to insert order");

        Order order = MapToOrder(reader);

        return order;
    }

    public async Task ChangeStatusAsync(Order order, CancellationToken ct)
    {
        const string sql =
            """
            update orders
            set order_state = :value_state
            where order_id = :value_order_id
            """;

        await using NpgsqlConnection connection = await npgsqlDataSource.OpenConnectionAsync(ct);

        await using NpgsqlCommand command = connection.CreateCommand();

        command.CommandText = sql;

        command
            .Parameters
            .Add(new NpgsqlParameter
            {
                ParameterName = ":value_state",
                DataTypeName = "order_state",
            });

        command
            .Parameters
            .Add(":value_order_id", NpgsqlDbType.Bigint);

        await command.PrepareAsync(ct);

        command.Parameters[":value_state"].Value = order.State;
        command.Parameters[":value_order_id"].Value = order.Id.Value;

        await command.ExecuteNonQueryAsync(ct);
    }

    public async IAsyncEnumerable<Order> FindOrdersAsync(
        OrderSearchingFilters filters,
        PaginationInfo<OrderId> paginationInfo,
        [EnumeratorCancellation] CancellationToken ct)
    {
        const string sql =
            """
            select 
                order_id, order_state, order_created_at, order_created_by
            from 
                orders
            where 
                (:page_token is null or order_id > :page_token)
                and (cardinality(:ids) = 0 or order_id = any(:ids))
                and (:value_state is null or order_state = :value_state) 
                and (:author is null or order_created_by like :author)
            order by order_id
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
            .Add(":author", NpgsqlDbType.Text);

        command
            .Parameters
            .Add(new NpgsqlParameter
            {
                ParameterName = ":value_state",
                DataTypeName = "order_state",
            });

        await command.PrepareAsync(ct);

        command.Parameters[":page_token"].Value =
            paginationInfo.PageToken?.Value is { } value
                ? value
                : DBNull.Value;

        command.Parameters[":page_size"].Value = paginationInfo.PageSize;

        command.Parameters[":ids"].Value =
            filters.OrderIds.Select(x => x.Value).ToArray();

        command.Parameters[":author"].Value =
            filters.Author is null
                ? DBNull.Value
                : filters.Author;

        command.Parameters[":value_state"].Value =
            filters.State is null
                ? DBNull.Value
                : filters.State;

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(ct);

        while (await reader.ReadAsync(ct))
        {
            yield return MapToOrder(reader);
        }
    }

    public async Task<Order?> FindOrderByIdAsync(OrderId orderId, CancellationToken ct)
    {
        const string sql =
            """
            select 
                order_id, order_state, order_created_at, order_created_by
            from 
                orders
            where 
                 order_id = :value_order_id
            """;

        await using NpgsqlConnection connection = await npgsqlDataSource.OpenConnectionAsync(ct);

        await using NpgsqlCommand command = connection.CreateCommand();

        command.CommandText = sql;

        command
            .Parameters
            .Add(":value_order_id", NpgsqlDbType.Bigint);

        await command.PrepareAsync(ct);

        command.Parameters[":value_order_id"].Value = orderId.Value;

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(ct);

        if (!await reader.ReadAsync(ct))
            return null;

        return MapToOrder(reader);
    }

    private static Order MapToOrder(NpgsqlDataReader reader)
    {
        int idOrdinal = reader.GetOrdinal("order_id");
        int stateOrdinal = reader.GetOrdinal("order_state");
        int createdAtOrdinal = reader.GetOrdinal("order_created_at");
        int createdByOrdinal = reader.GetOrdinal("order_created_by");

        var orderId = OrderId.Create(reader.GetInt64(idOrdinal));

        Order order = new(
            orderId,
            reader.GetFieldValue<OrderState>(stateOrdinal),
            reader.GetDateTime(createdAtOrdinal),
            reader.GetString(createdByOrdinal));

        return order;
    }
}