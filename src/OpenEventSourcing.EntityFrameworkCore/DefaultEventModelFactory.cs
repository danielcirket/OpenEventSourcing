using System;
using OpenEventSourcing.Commands;
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

        public Entities.Event Create(IEvent @event, ICommand causation)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));
            if (causation == null)
                throw new ArgumentNullException(nameof(causation));

            var type = @event.GetType();

            return new Entities.Event
            {
                AggregateId = @event.AggregateId,
                CorrelationId = causation.CorrelationId,
                CausationId = causation.Id,
                Data = _eventSerializer.Serialize(@event),
                Id = @event.Id,
                Name = type.Name,
                Type = type.FullName,
                Timestamp = @event.Timestamp,
                UserId = causation.UserId,
            };
        }
        public Entities.Event Create(IEvent @event, IIntegrationEvent causation)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));
            if (causation == null)
                throw new ArgumentNullException(nameof(causation));

            var type = @event.GetType();

            return new Entities.Event
            {
                AggregateId = @event.AggregateId,
                CorrelationId = causation.CorrelationId,
                CausationId = causation.Id,
                Data = _eventSerializer.Serialize(@event),
                Id = @event.Id,
                Name = type.Name,
                Type = type.FullName,
                Timestamp = @event.Timestamp,
                UserId = causation.UserId,
            };
        }
        public Entities.Event Create(IEvent @event, Guid? causationId, Guid? correlationId, string userId)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var type = @event.GetType();

            return new Entities.Event
            {
                AggregateId = @event.AggregateId,
                CorrelationId = correlationId,
                CausationId = causationId,
                Data = _eventSerializer.Serialize(@event),
                Id = @event.Id,
                Name = type.Name,
                Type = type.FullName,
                Timestamp = @event.Timestamp,
                UserId = userId,
            };
        }
    }
}
