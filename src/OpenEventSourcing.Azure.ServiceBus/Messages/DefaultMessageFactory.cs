using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.ServiceBus;
using OpenEventSourcing.Events;
using OpenEventSourcing.Serialization;

namespace OpenEventSourcing.Azure.ServiceBus.Messages
{
    internal sealed class DefaultMessageFactory : IMessageFactory
    {
        private readonly IEventSerializer _eventSerializer;

        public DefaultMessageFactory(IEventSerializer eventSerializer)
        {
            if (eventSerializer == null)
                throw new ArgumentNullException(nameof(eventSerializer));

            _eventSerializer = eventSerializer;
        }

        public Message CreateMessage<TEvent>(TEvent @event, Guid? correlationId = null) where TEvent : IEvent
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var eventName = typeof(TEvent).Name;

            return CreateMessage(eventName, @event, correlationId);
        }
        public Message CreateMessage(IEvent @event, Guid? correlationId = null)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var eventName = @event.GetType().Name;

            return CreateMessage(eventName, @event, correlationId);
        }
        private Message CreateMessage(string eventName, IEvent @event, Guid? correlationId)
        {
            var body = Encoding.UTF8.GetBytes(_eventSerializer.Serialize(@event));

            var message = new Message
            {
                MessageId = @event.Id.ToString(),
                Body = body,
                Label = eventName,
                CorrelationId = correlationId?.ToString()
            };

            return message;
        }
    }
}
