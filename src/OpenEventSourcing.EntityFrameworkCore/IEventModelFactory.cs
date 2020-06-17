using OpenEventSourcing.Events;

namespace OpenEventSourcing.EntityFrameworkCore
{
    public interface IEventModelFactory
    {
        Entities.Event Create(IEventContext<IEvent> context);
    }
}
