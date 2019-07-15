namespace OpenEventSourcing.Serialization
{
    public interface ICommandSerializer
    {
        string Serialize<T>(T data);
    }
}
