using OpenEventSourcing.Events;

namespace OpenEventSourcing.EntityFrameworkCore
{
    public interface IEventContextFactory
    {
        IEventContext<IEvent> CreateContext(Entities.Event dbEvent);
    }
}
