using Application.Models.Primitives.CompositeObjects;
using Infrastructure.Persistence.Serialization.Options;
using System.Text.Json;

namespace Infrastructure.Persistence.Serialization.Serializers;

internal static class OrderHistoryItemPayloadSerializer
{
    private static readonly JsonSerializerOptions Options = JsonIdSerializerOptions.JsonOptions;

    public static string SerializePayload(OrderHistoryEvent payload)
    {
        return JsonSerializer.Serialize(payload, Options); // payload.GetType(),
    }

    public static OrderHistoryEvent DeserializePayload(string json) // OrderHistoryItemKind kind,
    {
        // return kind switch
        // {
        //     OrderHistoryItemKind.Created =>
        //         JsonSerializer.Deserialize<OrderCreated>(json, JsonIdSerializerOptions.JsonOptions)
        //         ?? throw new InvalidOperationException("Failed to deserialize OrderCreated"),
        //
        //     OrderHistoryItemKind.ItemAdded =>
        //         JsonSerializer.Deserialize<ItemAdded>(json, JsonIdSerializerOptions.JsonOptions)
        //         ?? throw new InvalidOperationException("Failed to deserialize ItemAdded"),
        //
        //     OrderHistoryItemKind.ItemRemoved =>
        //         JsonSerializer.Deserialize<ItemRemoved>(json, JsonIdSerializerOptions.JsonOptions)
        //         ?? throw new InvalidOperationException("Failed to deserialize ItemRemoved"),
        //
        //     OrderHistoryItemKind.StateChanged =>
        //         JsonSerializer.Deserialize<StateChanged>(json, JsonIdSerializerOptions.JsonOptions)
        //         ?? throw new InvalidOperationException("Failed to deserialize StateChanged"),
        //
        //     _ => throw new UnreachableException("Unknown order history item kind"),
        // };
        return JsonSerializer.Deserialize<OrderHistoryEvent>(json, Options)
                        ?? throw new InvalidOperationException("Failed to deserialize payload");
    }
}