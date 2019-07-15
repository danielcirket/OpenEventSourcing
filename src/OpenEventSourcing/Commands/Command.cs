using System;
using OpenEventSourcing.Extensions;

namespace OpenEventSourcing.Commands
{
    public abstract class Command : ICommand
    {
        public Guid Id { get; }
        public Guid AggregateId { get; }
        public Guid CorrelationId { get; }
        public int Version { get; }
        public string UserId { get; }
        public DateTimeOffset Timestamp { get; }

        public Command(Guid aggregateId, Guid correlationId, int version, string userId)
        {
            Id = Guid.NewGuid().ToSequentialGuid();
            AggregateId = aggregateId;
            CorrelationId = correlationId;
            Version = version;
            UserId = userId;
            Timestamp = DateTimeOffset.UtcNow;
        }
    }
}
