using System;
using System.Text;
using OpenEventSourcing.Events;
using OpenEventSourcing.Serialization;

namespace OpenEventSourcing.RabbitMQ.Messages
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

        public Message CreateMessage<TEvent>(TEvent @event) where TEvent : IEvent
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var eventName = typeof(TEvent).Name;

            return CreateMessage(eventName, @event);
        }
        public Message CreateMessage(IEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var eventName = @event.GetType().Name;

            return CreateMessage(eventName, @event);
        }

        private Message CreateMessage(string eventName, IEvent @event)
        { 
            var body = Encoding.UTF8.GetBytes(_eventSerializer.Serialize(@event));

            return new Message
            {
                MessageId = @event.Id,
                Type = eventName,
                CorrelationId = @event.CorrelationId,
                Body = body,
            };
        }
    }
}
