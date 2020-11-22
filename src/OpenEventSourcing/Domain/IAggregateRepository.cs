using System;
using System.Threading.Tasks;
using OpenEventSourcing.Commands;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.Domain
{
    public interface IAggregateRepository
    {
        Task<TAggregate> GetAsync<TAggregate, TState>(string id)
            where TAggregate : Aggregate<TState>
            where TState : IAggregateState, new();
        Task SaveAsync<TState>(Aggregate<TState> aggregate, int? expectedVersion = null)
            where TState : IAggregateState, new();
        Task SaveAsync<TState>(Aggregate<TState> aggregate, ICommand causation, int? expectedVersion = null)
            where TState : IAggregateState, new();
        Task SaveAsync<TState>(Aggregate<TState> aggregate, IEventContext<IEvent> causation, int? expectedVersion = null)
            where TState : IAggregateState, new();
    }
}
