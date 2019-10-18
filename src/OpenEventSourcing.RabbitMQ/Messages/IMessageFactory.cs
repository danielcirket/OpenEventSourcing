using OpenEventSourcing.Events;

namespace OpenEventSourcing.RabbitMQ.Messages
{
    public interface IMessageFactory
    {
        Message CreateMessage<TEvent>(TEvent @event) where TEvent : IEvent;
        Message CreateMessage(IEvent @event);
    }
}
