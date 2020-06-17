using System.Threading;
using System.Threading.Tasks;

namespace OpenEventSourcing.Events
{
    public interface IEventDispatcher
    {
        Task DispatchAsync<TEvent>(IEventContext<TEvent> context, CancellationToken cancellationToken = default) where TEvent : IEvent;
        Task DispatchAsync(IEventContext<IEvent> context, CancellationToken cancellationToken = default);
    }
}
