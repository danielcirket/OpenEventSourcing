namespace OpenEventSourcing.Serialization
{
    public interface IQuerySerializer
    {
        string Serialize<T>(T data);
    }
}
