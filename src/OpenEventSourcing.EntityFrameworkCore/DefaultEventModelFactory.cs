using System;
using OpenEventSourcing.Events;
using OpenEventSourcing.Serialization;

namespace OpenEventSourcing.EntityFrameworkCore
{
    internal sealed class DefaultEventModelFactory : IEventModelFactory
    {
        private readonly IEventSerializer _eventSerializer;

        public DefaultEventModelFactory(IEventSerializer eventSerializer)
        {
            if (eventSerializer == null)
                throw new ArgumentNullException(nameof(eventSerializer));

            _eventSerializer = eventSerializer;
        }

        public Entities.Event Create(IEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var type = @event.GetType();

            return new Entities.Event
            {
                AggregateId = @event.AggregateId,
                CorrelationId = @event.CorrelationId.Value,
                CausationId = @event.CausationId.Value,
                Data = _eventSerializer.Serialize(@event),
                Id = @event.Id,
                Name = type.Name,
                Type = type.FullName,
                Timestamp = @event.Timestamp,
                UserId = @event.UserId,
            };
        }
    }
}
