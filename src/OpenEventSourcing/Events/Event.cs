using System;
using OpenEventSourcing.Extensions;

namespace OpenEventSourcing.Events
{
    public abstract class Event : IEvent
    {
        public Guid Id { get; protected set; }
        public Guid AggregateId { get; protected set; }
        public DateTimeOffset Timestamp { get; protected set; }
        public int Version { get; protected set; }

        public Event(Guid aggregateId, int version)
        {
            Id = Guid.NewGuid().ToSequentialGuid();
            AggregateId = aggregateId;
            Timestamp = DateTimeOffset.UtcNow;
            Version = version;
        }
    }
}
