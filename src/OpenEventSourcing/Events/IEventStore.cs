using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenEventSourcing.Domain;

namespace OpenEventSourcing.Events
{
    public interface IEventStore
    {
        Task<Page> GetEventsAsync(long offset);
        Task<IEnumerable<IEvent>> GetEventsAsync(Guid aggregateId);
        Task<IEnumerable<IEvent>> GetEventsAsync(Guid aggregateId, long offset);
        Task SaveAsync<TState>(Aggregate<TState> aggregate, int expectedVersion)
            where TState : IAggregateState, new();
    }
}
