using OpenEventSourcing.Events;

namespace OpenEventSourcing.RabbitMQ.Messages
{
    public interface IMessageFactory
    {
        Message CreateMessage<TEvent>(IEventContext<TEvent> context) where TEvent : IEvent;
        Message CreateMessage(IEventContext<IEvent> context);
    }
}
