using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenEventSourcing.Commands;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.RabbitMQ.Queues
{
    public interface IQueueMessageSender
    {
        Task SendAsync<TEvent>(TEvent @event, ICommand causation) where TEvent : IEvent;
        Task SendAsync<TEvent>(TEvent @event, IIntegrationEvent causation) where TEvent : IEvent;
        Task SendAsync<TEvent>(TEvent @event, Guid? causationId = null, Guid? correlationId = null, string userId = null) where TEvent : IEvent;
        Task SendAsync(IEnumerable<IEvent> events, ICommand causation);
        Task SendAsync(IEnumerable<IEvent> events, IIntegrationEvent causation);
        Task SendAsync(IEnumerable<IEvent> events, Guid? causationId = null, Guid? correlationId = null, string userId = null);
        //Task<string> SendAsync<TEvent>(TEvent @event, DateTimeOffset publishOnUtc) where TEvent : IEvent;
        //Task CancelScheduledMessageAsync(string identifier);
    }
}
