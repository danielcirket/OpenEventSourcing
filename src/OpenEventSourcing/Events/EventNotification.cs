using System;

namespace OpenEventSourcing.Events
{
    public sealed class EventNotification<TEvent> : IEventNotification<TEvent>
        where TEvent : IEvent
    {
        public Guid? CorrelationId { get; }
        public Guid? CausationId { get; }
        public TEvent Payload { get; }
        public DateTimeOffset Timestamp { get; }
        public string UserId { get; }

        public EventNotification(TEvent @event, Guid? correlationId, Guid? causationId, DateTimeOffset timestamp, string userId)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            CorrelationId = correlationId;
            CausationId = causationId;
            Payload = @event;
            Timestamp = timestamp;
            UserId = userId;
        }
    }
}
