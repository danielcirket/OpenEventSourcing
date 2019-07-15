using System;
using System.Collections.Generic;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.Domain
{
    internal sealed class AggregateFactory : IAggregateFactory
    {
        public TAggregate FromHistory<TAggregate, TState>(IEnumerable<IEvent> events)
            where TAggregate : Aggregate<TState>
            where TState : IAggregateState, new()
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));
            
            var aggregate = (TAggregate)Activator.CreateInstance(typeof(TAggregate), new object[] { new TState() });

            aggregate.FromHistory(events);

            return aggregate;
        }
    }
}
