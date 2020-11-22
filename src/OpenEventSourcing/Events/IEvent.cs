using System;
using OpenEventSourcing.Commands;

namespace OpenEventSourcing.Events
{
    public interface IEvent
    {
        string Id { get; }
        string Subject { get; }
        DateTimeOffset Timestamp { get; }
        int Version { get; }
    }
}
