using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenEventSourcing.Events
{
    public interface IEventStore
    {
        Task<Page> GetEventsAsync(long offset, CancellationToken cancellationToken = default);
        Task<IEnumerable<IEventContext<IEvent>>> GetEventsAsync(string streamId, CancellationToken cancellationToken = default);
        Task<IEnumerable<IEventContext<IEvent>>> GetEventsAsync(string streamId, long offset, CancellationToken cancellationToken = default);
        Task SaveAsync(string streamId, IEnumerable<IEventContext<IEvent>> events, CancellationToken cancellationToken = default);
        Task<long> CountAsync(string streamId, CancellationToken cancellationToken = default);
    }
}
