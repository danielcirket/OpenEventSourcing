using Microsoft.Azure.ServiceBus;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.Azure.ServiceBus.Messages
{
    public interface IMessageFactory
    {
        Message CreateMessage<TEvent>(IEventNotification<TEvent> context) where TEvent : IEvent;
        Message CreateMessage(IEventNotification<IEvent> context);
    }
}
