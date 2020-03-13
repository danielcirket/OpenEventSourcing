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

        public Message CreateMessage<TEvent>(TEvent @event, Guid? causationId = null, Guid? correlationId = null, string userId = null) where TEvent : IEvent
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var eventName = typeof(TEvent).Name;

            return CreateMessage(eventName, @event, causationId, correlationId, userId);
        }
        public Message CreateMessage(IEvent @event, Guid? causationId = null, Guid? correlationId = null, string userId = null)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            var eventName = @event.GetType().Name;

            return CreateMessage(eventName, @event, causationId, correlationId, userId);
        }

        private Message CreateMessage(string eventName, IEvent @event, Guid? causationId = null, Guid? correlationId = null, string userId = null)
        {
            var body = Encoding.UTF8.GetBytes(_eventSerializer.Serialize(@event));

            return new Message
            {
                MessageId = @event.Id,
                Type = eventName,
                CorrelationId = correlationId,
                Body = body,
                UserId = userId,
                CausationId = causationId,
            };
        }
    }
}
