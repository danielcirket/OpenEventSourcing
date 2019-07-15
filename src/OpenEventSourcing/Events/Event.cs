using System;
using OpenEventSourcing.Commands;
using OpenEventSourcing.Extensions;

namespace OpenEventSourcing.Events
{
    public abstract class Event : IEvent
    {
        public Guid Id { get; protected set; }
        public Guid AggregateId { get; protected set; }
        public Guid? CorrelationId { get; protected set; }
        public Guid? CausationId { get; protected set; }
        public DateTimeOffset Timestamp { get; protected set; }
        public int Version { get; protected set; }
        public string UserId { get; protected set; }

        public Event(Guid aggregateId, int version)
        {
            Id = Guid.NewGuid().ToSequentialGuid();
            AggregateId = aggregateId;
            Timestamp = DateTimeOffset.UtcNow;
            Version = version;
        }

        public void UpdateFrom(ICommand command)
        {
            CorrelationId = command.CorrelationId;
            CausationId = command.Id;
            UserId = command.UserId;
        }
    }
}
