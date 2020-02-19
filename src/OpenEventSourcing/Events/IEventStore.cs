using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenEventSourcing.Domain;

namespace OpenEventSourcing.Events
{
    public interface IEventStore
    {
        Task<Page> GetEventsAsync(long offset);
        Task<IEnumerable<IEvent>> GetEventsAsync(string subject);
        Task<IEnumerable<IEvent>> GetEventsAsync(string subject, long offset);
        Task SaveAsync(IEnumerable<IEvent> events);
        Task<long> CountAsync(string subject);
    }
}
