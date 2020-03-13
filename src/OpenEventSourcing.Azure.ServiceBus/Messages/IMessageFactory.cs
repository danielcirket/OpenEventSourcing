using System;
using Microsoft.Azure.ServiceBus;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.Azure.ServiceBus.Messages
{
    public interface IMessageFactory
    {
        Message CreateMessage<TEvent>(TEvent @event, Guid? correlationId = null) where TEvent : IEvent;
        Message CreateMessage(IEvent @event, Guid? correlationId = null);
    }
}
