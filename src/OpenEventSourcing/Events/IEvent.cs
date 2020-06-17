using System;
using OpenEventSourcing.Commands;

namespace OpenEventSourcing.Events
{
    public interface IEvent
    {
        Guid Id { get; }
        Guid AggregateId { get; }
        DateTimeOffset Timestamp { get; }
        int Version { get; }
    }
}
