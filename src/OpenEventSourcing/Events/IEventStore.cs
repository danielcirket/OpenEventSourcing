using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenEventSourcing.Commands;
using OpenEventSourcing.Domain;

namespace OpenEventSourcing.Events
{
    public interface IEventStore
    {
        Task<Page> GetEventsAsync(long offset);
        Task<IEnumerable<IIntegrationEvent>> GetEventsAsync(Guid aggregateId);
        Task<IEnumerable<IIntegrationEvent>> GetEventsAsync(Guid aggregateId, long offset);
        Task SaveAsync(IEnumerable<IEvent> events, ICommand causation);
        Task SaveAsync(IEnumerable<IEvent> events, IIntegrationEvent causation);
        Task SaveAsync(IEnumerable<IEvent> events, Guid? causationId = null, Guid? correlationId = null, string userId = null);
        Task<long> CountAsync(Guid aggregateId);
    }
}
