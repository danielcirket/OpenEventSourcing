using System.Threading;
using System.Threading.Tasks;

namespace OpenEventSourcing.Events
{
    public interface IEventHandler<TEvent> where TEvent : IEvent
    {
        Task HandleAsync(IEventContext<TEvent> context, CancellationToken cancellationToken = default);
    }
}
