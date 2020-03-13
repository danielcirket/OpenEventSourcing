using System;

namespace OpenEventSourcing.Events
{
    public sealed class IntegrationEvent : IIntegrationEvent
    {
        public Guid Id { get; }
        public Guid AggregateId { get; }
        public IEvent Payload { get; }
        public Guid? CorrelationId { get; }
        public Guid? CausationId { get; }
        public DateTimeOffset Timestamp { get; }
        public int Version { get; }
        public string UserId { get; }

        public IntegrationEvent(Guid id, Guid aggregateId, IEvent payload, Guid? correlationId, Guid? causationId, DateTimeOffset timestamp, int version, string userId)
        {
            Id = id;
            AggregateId = aggregateId;
            Payload = payload;
            CorrelationId = correlationId;
            CausationId = causationId;
            Timestamp = timestamp;
            Version = version;
            UserId = userId;
        }
    }
}
