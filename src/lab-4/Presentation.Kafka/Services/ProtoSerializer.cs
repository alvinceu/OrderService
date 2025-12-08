using Confluent.Kafka;
using Google.Protobuf;

namespace Presentation.Kafka.Services;

internal sealed class ProtoSerializer<T> : ISerializer<T> where T : IMessage<T>, new()
{
    public byte[] Serialize(T data, SerializationContext context)
    {
        return data.ToByteArray();
    }
}