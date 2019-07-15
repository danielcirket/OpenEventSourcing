using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenEventSourcing.Events
{
    public interface IEventBus
    {
        Task StartAsync();
        Task StopAsync();
        Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent;
        Task PublishAsync(IEnumerable<IEvent> events);
        Task<string> PublishAsync<TEvent>(TEvent @event, DateTimeOffset publishOnUtc) where TEvent : IEvent;
        Task CancelScheduledMessageAsync(string identifier);
    }
}
