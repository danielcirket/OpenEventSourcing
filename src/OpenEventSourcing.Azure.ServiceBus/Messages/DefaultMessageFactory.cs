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

        public Message CreateMessage<TEvent>(IEventContext<TEvent> context) where TEvent : IEvent
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (context.Payload == null)
                throw new ArgumentNullException($"{nameof(context)}.{nameof(context.Payload)}");

            var eventName = typeof(TEvent).Name;

            return CreateMessage(eventName, (IEventContext<IEvent>)context);
        }
        public Message CreateMessage(IEventContext<IEvent> context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (context.Payload == null)
                throw new ArgumentNullException($"{nameof(context)}.{nameof(context.Payload)}");

            var @event = context.Payload;
            var eventName = @event.GetType().Name;

            return CreateMessage(eventName, context);
        }
        private Message CreateMessage(string eventName, IEventContext<IEvent> context)
        {
            var @event = context.Payload;
            var body = Encoding.UTF8.GetBytes(_eventSerializer.Serialize(@event));

            var message = new Message
            {
                MessageId = @event.Id.ToString(),
                Body = body,
                Label = eventName,
                CorrelationId = context.CorrelationId.ToString(),
                UserProperties =
                {
                    { nameof(context.CausationId), context.CausationId?.ToString() },
                    { nameof(context.UserId), context.UserId },
                    { nameof(context.Timestamp), context.Timestamp },
                },
            };

            return message;
        }
    }
}
