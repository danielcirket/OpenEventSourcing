using System;
using System.Threading.Tasks;

namespace OpenEventSourcing.Domain
{
    public interface IAggregateRepository
    {
        Task<TAggregate> GetAsync<TAggregate, TState>(Guid id)
            where TAggregate : Aggregate<TState>
            where TState : IAggregateState, new();
        Task SaveAsync<TAggregate, TState>(TAggregate aggregate, int? expectedVersion = null)
            where TAggregate : Aggregate<TState>
            where TState : IAggregateState, new();
    }
}
