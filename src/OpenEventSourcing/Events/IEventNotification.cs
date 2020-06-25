using System;

namespace OpenEventSourcing.Events
{
    public interface IEventNotification<out TEvent> where TEvent : IEvent
    {
        Guid? CorrelationId { get; }
        Guid? CausationId { get; }
        TEvent Payload { get; }
        DateTimeOffset Timestamp { get; }
        string UserId { get; }
    }
}
