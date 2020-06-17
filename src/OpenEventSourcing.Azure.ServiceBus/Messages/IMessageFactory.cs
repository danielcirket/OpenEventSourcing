using Microsoft.Azure.ServiceBus;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.Azure.ServiceBus.Messages
{
    public interface IMessageFactory
    {
        Message CreateMessage<TEvent>(IEventContext<TEvent> context) where TEvent : IEvent;
        Message CreateMessage(IEventContext<IEvent> context);
    }
}
