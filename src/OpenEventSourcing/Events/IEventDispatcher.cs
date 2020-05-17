using System.Threading;
using System.Threading.Tasks;

namespace OpenEventSourcing.Events
{
    public interface IEventDispatcher
    {
        Task DispatchAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent;
        Task DispatchAsync(IEvent @event, CancellationToken cancellationToken = default);
    }
}
