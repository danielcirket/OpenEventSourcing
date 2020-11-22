using System;

namespace OpenEventSourcing.Events
{
    public sealed class EventNotification<TEvent> : IEventNotification<TEvent>
        where TEvent : IEvent
    {
        public string StreamId { get; }
        public string CorrelationId { get; }
        public string CausationId { get; }
        public TEvent Payload { get; }
        public DateTimeOffset Timestamp { get; }
        public string UserId { get; }

        public EventNotification(string streamId, TEvent @event, string correlationId, string causationId, DateTimeOffset timestamp, string userId)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            StreamId = streamId;
            CorrelationId = correlationId;
            CausationId = causationId;
            Payload = @event;
            Timestamp = timestamp;
            UserId = userId;
        }
    }
}
