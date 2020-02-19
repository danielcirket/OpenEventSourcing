using System;
using System.Linq;
using System.Threading.Tasks;
using OpenEventSourcing.Domain;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.EntityFrameworkCore
{
    internal sealed class AggregateRepository : IAggregateRepository
    {
        private readonly IEventStore _eventStore;
        private readonly IAggregateFactory _aggregateFactory;

        public AggregateRepository(IEventStore eventStore,
                                   IAggregateFactory aggregateFactory)
        {
            if (eventStore == null)
                throw new ArgumentNullException(nameof(eventStore));
            if (aggregateFactory == null)
                throw new ArgumentNullException(nameof(aggregateFactory));

            _eventStore = eventStore;
            _aggregateFactory = aggregateFactory;
        }

        public async Task<TAggregate> GetAsync<TAggregate, TState>(string subject)
            where TAggregate : Aggregate<TState>
            where TState : IAggregateState, new()
        {
            var events = await _eventStore.GetEventsAsync(subject);

            if (!events.Any())
                return default;

            var aggregate = _aggregateFactory.FromHistory<TAggregate, TState>(events);

            return aggregate;
        }
        public async Task SaveAsync<TState>(Aggregate<TState> aggregate, int? expectedVersion = null)
            where TState : IAggregateState, new()
        {
            if (aggregate == null)
                throw new ArgumentNullException(nameof(aggregate));

            var events = aggregate.GetUncommittedEvents();

            if (!events.Any())
                return;

            var currentVersion = await _eventStore.CountAsync(aggregate.Id);

            if (expectedVersion.GetValueOrDefault() != currentVersion)
                throw new ConcurrencyException(aggregate.Id, expectedVersion.GetValueOrDefault(), currentVersion);

            await _eventStore.SaveAsync(events);

            aggregate.ClearUncommittedEvents();
        }
    }
}
