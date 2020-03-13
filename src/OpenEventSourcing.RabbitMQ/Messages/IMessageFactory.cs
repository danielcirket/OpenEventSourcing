using System;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.RabbitMQ.Messages
{
    public interface IMessageFactory
    {
        Message CreateMessage<TEvent>(TEvent @event, Guid? causationId = null, Guid? correlationId = null, string userId = null) where TEvent : IEvent;
        Message CreateMessage(IEvent @event, Guid? causationId = null, Guid? correlationId = null, string userId = null);
    }
}
