using System;

namespace OpenEventSourcing.Serialization
{
    public interface ICommandDeserializer
    {
        object Deserialize(string data, Type type);
        T Deserialize<T>(string data);
    }
}
