using Microsoft.Azure.ServiceBus;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.Azure.ServiceBus.Messages
{
    public interface IEventContextFactory
    {
        IEventContext<IEvent> CreateContext(Message message);
    }
}
