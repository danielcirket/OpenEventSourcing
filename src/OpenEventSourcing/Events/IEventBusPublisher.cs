using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenEventSourcing.Commands;

namespace OpenEventSourcing.Events
{
    public interface IEventBusPublisher
    {
        Task PublishAsync<TEvent>(TEvent @event, ICommand causation) where TEvent : IEvent;
        Task PublishAsync<TEvent>(TEvent @event, IIntegrationEvent causation) where TEvent : IEvent;
        Task PublishAsync<TEvent>(TEvent @event, Guid? causationId = null, Guid? correlationId = null, string userId = null) where TEvent : IEvent;
        Task PublishAsync(IEnumerable<IEvent> events, ICommand causation);
        Task PublishAsync(IEnumerable<IEvent> events, IIntegrationEvent causation);
        Task PublishAsync(IEnumerable<IEvent> events, Guid? causationId, Guid? correlationId, string userId);
    }
}
