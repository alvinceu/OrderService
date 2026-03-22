using Application.Models.Commons;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Persistence.Serialization.Converters;

internal sealed class EntityIdConverter<TId> : JsonConverter<TId> where TId : struct, IId<TId>, IEquatable<TId>
{
    public override TId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return TId.Create(reader.GetInt64());
    }

    public override void Write(Utf8JsonWriter writer, TId value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Value);
    }
}