using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.Azure.ServiceBus.Topics
{
    public interface ITopicMessageSender
    {
        Task SendAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent;
        Task SendAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken = default);
    }
}
