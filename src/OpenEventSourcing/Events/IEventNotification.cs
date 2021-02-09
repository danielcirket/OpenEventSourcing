using System;

namespace OpenEventSourcing.Events
{
    public interface IEventNotification<out TEvent> where TEvent : IEvent
    {
        string StreamId { get; }
        CorrelationId? CorrelationId { get; }
        CausationId? CausationId { get; }
        TEvent Payload { get; }
        DateTimeOffset Timestamp { get; }
        string UserId { get; }
    }
}
