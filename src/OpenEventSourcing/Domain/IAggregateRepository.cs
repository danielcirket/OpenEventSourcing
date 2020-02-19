using System;
using System.Threading.Tasks;

namespace OpenEventSourcing.Domain
{
    public interface IAggregateRepository
    {
        Task<TAggregate> GetAsync<TAggregate, TState>(string subject)
            where TAggregate : Aggregate<TState>
            where TState : IAggregateState, new();
        Task SaveAsync<TState>(Aggregate<TState> aggregate, int? expectedVersion = null)
            where TState : IAggregateState, new();
    }
}
