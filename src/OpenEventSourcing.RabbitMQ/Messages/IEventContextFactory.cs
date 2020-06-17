using OpenEventSourcing.Events;

namespace OpenEventSourcing.RabbitMQ.Messages
{
    public interface IEventContextFactory
    {
        IEventContext<IEvent> CreateContext(ReceivedMessage message);
    }
}
