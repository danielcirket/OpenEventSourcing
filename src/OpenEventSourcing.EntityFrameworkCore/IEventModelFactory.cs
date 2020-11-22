using OpenEventSourcing.Events;

namespace OpenEventSourcing.EntityFrameworkCore
{
    public interface IEventModelFactory
    {
        Entities.Event Create(string streamId, IEventContext<IEvent> context);
    }
}
