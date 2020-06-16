using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenEventSourcing.Events
{
    public interface IEventBusPublisher
    {
        Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent;
        Task PublishAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken = default);
    }
}
