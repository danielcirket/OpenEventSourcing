using System;

namespace OpenEventSourcing.Events
{
    public interface IEventNotification<out TEvent> where TEvent : IEvent
    {
        string StreamId { get; }
        string CorrelationId { get; }
        string CausationId { get; }
        TEvent Payload { get; }
        DateTimeOffset Timestamp { get; }
        string UserId { get; }
    }
}
