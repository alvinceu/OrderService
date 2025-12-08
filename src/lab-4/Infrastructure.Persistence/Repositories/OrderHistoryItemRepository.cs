using Application.Abstractions.Repositories;
using Application.Models.Commons.Paginations;
using Application.Models.CreationParameters;
using Application.Models.Entities;
using Application.Models.Primitives.CompositeObjects;
using Application.Models.Primitives.EntityIds;
using Application.Models.Primitives.Enums;
using Application.Models.SearchingFilters;
using Infrastructure.Persistence.Serialization.Serializers;
using Npgsql;
using NpgsqlTypes;
using System.Runtime.CompilerServices;

namespace Infrastructure.Persistence.Repositories;

internal sealed class OrderHistoryItemRepository(NpgsqlDataSource npgsqlDataSource) : IOrderHistoryItemRepository
{
    public async Task<OrderHistoryItem> CreateOrderHistoryItemAsync(
        OrderHistoryItemCreationParameters parameters,
        CancellationToken ct)
    {
        const string sql =
            """
            insert into order_history
                (order_id, order_history_item_created_at, order_history_item_kind, order_history_item_payload)
            values 
                (:value_order_id, :value_created_at, :value_kind, :value_payload)
            returning 
                order_history_item_id, order_id, order_history_item_created_at,
                order_history_item_kind, order_history_item_payload
            """;

        await using NpgsqlConnection connection = await npgsqlDataSource.OpenConnectionAsync(ct);

        await using NpgsqlCommand command = connection.CreateCommand();

        command.CommandText = sql;

        command
            .Parameters
            .Add(":value_order_id", NpgsqlDbType.Bigint);

        command
            .Parameters
            .Add(":value_created_at", NpgsqlDbType.TimestampTz);

        command
            .Parameters
            .Add(new NpgsqlParameter
            {
                ParameterName = ":value_kind",
                DataTypeName = "order_history_item_kind",
            });

        command
            .Parameters
            .Add(":value_payload", NpgsqlDbType.Jsonb);

        await command.PrepareAsync(ct);

        command.Parameters[":value_order_id"].Value = parameters.OrderId.Value;
        command.Parameters[":value_created_at"].Value = parameters.CreatedAt;
        command.Parameters[":value_kind"].Value = parameters.Kind;
        command.Parameters[":value_payload"].Value =
            OrderHistoryItemPayloadSerializer.SerializePayload(parameters.Payload);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(ct);

        if (!await reader.ReadAsync(ct))
            throw new InvalidOperationException("failed to insert order history item");

        return MapToOrderHistoryItem(reader);
    }

    public async IAsyncEnumerable<OrderHistoryItem> FindOrderHistoryItemsAsync(
        OrderHistoryItemSearchingFilters filters,
        PaginationInfo<OrderHistoryItemId> paginationInfo,
        [EnumeratorCancellation] CancellationToken ct)
    {
        const string sql =
            """
            select
                order_history_item_id, order_id, order_history_item_created_at,
                order_history_item_kind, order_history_item_payload
            from
                order_history
            where (:page_token is null or order_history_item_id > :page_token)
                and (cardinality(:order_ids) = 0 or order_id = any(:order_ids))
                and (:value_kind is null or order_history_item_kind = :value_kind)
            order by order_history_item_id
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
            .Add(":order_ids", NpgsqlDbType.Array | NpgsqlDbType.Bigint);

        command.Parameters.Add(new NpgsqlParameter
        {
            ParameterName = ":value_kind",
            DataTypeName = "order_history_item_kind",
        });

        await command.PrepareAsync(ct);

        command.Parameters[":page_token"].Value =
            paginationInfo.PageToken?.Value is { } value
                ? value
                : DBNull.Value;

        command.Parameters[":page_size"].Value = paginationInfo.PageSize;

        command.Parameters[":order_ids"].Value =
            filters.OrderIds.Select(x => x.Value).ToArray();

        command.Parameters[":value_kind"].Value =
            filters.Kind is null
                ? DBNull.Value :
                filters.Kind;

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(ct);

        while (await reader.ReadAsync(ct))
        {
            yield return MapToOrderHistoryItem(reader);
        }
    }

    private static OrderHistoryItem MapToOrderHistoryItem(NpgsqlDataReader reader)
    {
        int idOrdinal = reader.GetOrdinal("order_history_item_id");
        int orderIdOrdinal = reader.GetOrdinal("order_id");
        int createdAtOrdinal = reader.GetOrdinal("order_history_item_created_at");
        int kindOrdinal = reader.GetOrdinal("order_history_item_kind");
        int payloadOrdinal = reader.GetOrdinal("order_history_item_payload");

        var id = OrderHistoryItemId.Create(reader.GetInt64(idOrdinal));
        var orderId = OrderId.Create(reader.GetInt64(orderIdOrdinal));
        DateTime createdAt = reader.GetDateTime(createdAtOrdinal);

        OrderHistoryItemKind kind = reader.GetFieldValue<OrderHistoryItemKind>(kindOrdinal);

        string json = reader.GetString(payloadOrdinal);

        OrderHistoryEvent payload = OrderHistoryItemPayloadSerializer.DeserializePayload(json); // kind,

        return new OrderHistoryItem(id, orderId, createdAt, kind, payload);
    }
}
