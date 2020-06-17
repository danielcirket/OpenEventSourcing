using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenEventSourcing.Events
{
    public interface IEventBusPublisher
    {
        Task PublishAsync<TEvent>(IEventContext<TEvent> context, CancellationToken cancellationToken = default) where TEvent : IEvent;
        Task PublishAsync(IEnumerable<IEventContext<IEvent>> contexts, CancellationToken cancellationToken = default);
    }
}
