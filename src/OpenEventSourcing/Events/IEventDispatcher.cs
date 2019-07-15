using System.Threading.Tasks;

namespace OpenEventSourcing.Events
{
    public interface IEventDispatcher
    {
        Task DispatchAsync<TEvent>(TEvent @event) where TEvent : IEvent;
        Task DispatchAsync(IEvent @event);
    }
}
