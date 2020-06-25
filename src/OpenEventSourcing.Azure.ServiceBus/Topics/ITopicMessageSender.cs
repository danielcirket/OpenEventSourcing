using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.Azure.ServiceBus.Topics
{
    public interface ITopicMessageSender
    {
        Task SendAsync<TEvent>(IEventNotification<TEvent> context, CancellationToken cancellationToken = default) where TEvent : IEvent;
        Task SendAsync(IEnumerable<IEventNotification<IEvent>> contexts, CancellationToken cancellationToken = default);
    }
}
