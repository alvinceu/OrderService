using Confluent.Kafka;
using Google.Protobuf;

namespace Presentation.Kafka.Services;

internal sealed class ProtoDeserializer<T> : IDeserializer<T> where T : IMessage<T>, new()
{
    private readonly MessageParser<T> _parser = new(() => new T());

    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull)
        {
            throw new NullReferenceException();
        }

        return _parser.ParseFrom(data);
    }
}