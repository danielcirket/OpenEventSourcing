using System;

namespace OpenEventSourcing.Serialization
{
    public interface IQueryDeserializer
    {
        object Deserialize(string data, Type type);
        T Deserialize<T>(string data);
    }
}
