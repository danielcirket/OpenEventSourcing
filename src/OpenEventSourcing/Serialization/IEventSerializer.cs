namespace OpenEventSourcing.Serialization
{
    public interface IEventSerializer
    {
        string Serialize<T>(T data);
    }
}
