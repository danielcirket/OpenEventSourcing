using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenEventSourcing.Events
{
    public interface IEventBusPublisher
    {
        Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent;
        Task PublishAsync(IEnumerable<IEvent> events);
    }
}
