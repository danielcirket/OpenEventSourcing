using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenEventSourcing.Events
{
    public interface IEventStore
    {
        Task<Page> GetEventsAsync(long offset, CancellationToken cancellationToken = default);
        Task<IEnumerable<IEventContext<IEvent>>> GetEventsAsync(Guid aggregateId, CancellationToken cancellationToken = default);
        Task<IEnumerable<IEventContext<IEvent>>> GetEventsAsync(Guid aggregateId, long offset, CancellationToken cancellationToken = default);
        Task SaveAsync(IEnumerable<IEventContext<IEvent>> events, CancellationToken cancellationToken = default);
        Task<long> CountAsync(Guid aggregateId, CancellationToken cancellationToken = default);
    }
}
