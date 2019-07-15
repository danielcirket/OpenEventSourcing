using OpenEventSourcing.Events;

namespace OpenEventSourcing.EntityFrameworkCore
{
    public interface IEventModelFactory
    {
        Entities.Event Create(IEvent @event);
    }
}
