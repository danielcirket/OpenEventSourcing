using System;

namespace OpenEventSourcing.Events
{
    public sealed class EventContext<TEvent> : IEventContext<TEvent>
        where TEvent : IEvent
    {
        public string StreamId { get; }
        public CorrelationId? CorrelationId { get; }
        public CausationId? CausationId { get; }
        public TEvent Payload { get; }
        public DateTimeOffset Timestamp { get; }
        public Actor Actor { get; }

        public EventContext(string streamId, TEvent @event, CorrelationId? correlationId, CausationId? causationId, DateTimeOffset timestamp, Actor actor)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            StreamId = streamId;
            CorrelationId = correlationId;
            CausationId = causationId;
            Payload = @event;
            Timestamp = timestamp;
            Actor = actor;
        }
    }
}
