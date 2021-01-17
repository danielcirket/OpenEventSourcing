using System;
using System.Linq;
using System.Threading.Tasks;
using OpenEventSourcing.Commands;
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

        public async Task<TAggregate> GetAsync<TAggregate, TState>(string id)
            where TAggregate : Aggregate<TState>
            where TState : IAggregateState, new()
        {
            var events = await _eventStore.GetEventsAsync(id);

            if (!events.Any())
                return default;

            var aggregate = _aggregateFactory.FromHistory<TAggregate, TState>(events.Select(e => e.Payload));

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

            var contexts = events.Select(@event => new EventContext<IEvent>(streamId: aggregate.Id, @event: @event, correlationId: null, causationId: null, @event.Timestamp, userId: "unknown"));

            await _eventStore.SaveAsync(aggregate.Id, contexts);

            aggregate.ClearUncommittedEvents();
        }
        public async Task SaveAsync<TState>(Aggregate<TState> aggregate, ICommand causation, int? expectedVersion = null)
            where TState : IAggregateState, new()
        {
            if (aggregate == null)
                throw new ArgumentNullException(nameof(aggregate));
            if (causation == null)
                throw new ArgumentNullException(nameof(causation));

            var events = aggregate.GetUncommittedEvents();

            if (!events.Any())
                return;

            var currentVersion = await _eventStore.CountAsync(aggregate.Id);

            if (expectedVersion.GetValueOrDefault() != currentVersion)
                throw new ConcurrencyException(aggregate.Id, expectedVersion.GetValueOrDefault(), currentVersion);

            var contexts = events.Select(@event => new EventContext<IEvent>(streamId: aggregate.Id, @event: @event, correlationId: causation.CorrelationId, causationId: CausationId.From(causation.Id), @event.Timestamp, userId: causation.UserId));

            await _eventStore.SaveAsync(aggregate.Id, contexts);

            aggregate.ClearUncommittedEvents();
        }
        public async Task SaveAsync<TState>(Aggregate<TState> aggregate, IEventContext<IEvent> causation, int? expectedVersion = null)
            where TState : IAggregateState, new()
        {
            if (aggregate == null)
                throw new ArgumentNullException(nameof(aggregate));
            if (causation == null)
                throw new ArgumentNullException(nameof(causation));

            var events = aggregate.GetUncommittedEvents();

            if (!events.Any())
                return;

            var currentVersion = await _eventStore.CountAsync(aggregate.Id);

            if (expectedVersion.GetValueOrDefault() != currentVersion)
                throw new ConcurrencyException(aggregate.Id, expectedVersion.GetValueOrDefault(), currentVersion);

            var contexts = events.Select(@event => new EventContext<IEvent>(streamId: aggregate.Id, @event: @event, correlationId: causation.CorrelationId, causationId: CausationId.From(causation.Payload.Id), @event.Timestamp, userId: causation.UserId));

            await _eventStore.SaveAsync(aggregate.Id, contexts);

            aggregate.ClearUncommittedEvents();
        }
    }
}
