using System;

namespace OpenEventSourcing.Events
{
    public interface IEventTypeCache
    {
        bool TryGet(string name, out Type type);
    }
}
