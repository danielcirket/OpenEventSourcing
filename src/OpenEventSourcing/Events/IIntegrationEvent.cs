using System;
using OpenEventSourcing.Commands;

namespace OpenEventSourcing.Events
{
    public interface IIntegrationEvent
    {
        Guid Id { get; }
        Guid AggregateId { get; }
        IEvent Payload { get; }
        Guid? CorrelationId { get; }
        Guid? CausationId { get; }
        DateTimeOffset Timestamp { get; }
        int Version { get; }
        string UserId { get; }
    }
}
