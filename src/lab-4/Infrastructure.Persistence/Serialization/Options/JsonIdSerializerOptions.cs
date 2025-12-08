using Application.Models.Primitives.EntityIds;
using Infrastructure.Persistence.Serialization.Converters;
using System.Text.Json;

namespace Infrastructure.Persistence.Serialization.Options;

public static class JsonIdSerializerOptions
{
    public static JsonSerializerOptions JsonOptions { get; } = new()
    {
        Converters =
        {
            new EntityIdConverter<OrderId>(),
            new EntityIdConverter<OrderItemId>(),
            new EntityIdConverter<OrderHistoryItemId>(),
            new EntityIdConverter<ProductId>(),
        },
    };
}