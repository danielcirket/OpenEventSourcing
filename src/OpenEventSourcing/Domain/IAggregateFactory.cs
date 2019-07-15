using System.Collections.Generic;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.Domain
{
    public interface IAggregateFactory
    {
        TAggregate FromHistory<TAggregate, TState>(IEnumerable<IEvent> events)
            where TAggregate : Aggregate<TState>
            where TState : IAggregateState, new();
    }
}
