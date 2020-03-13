using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Commands;
using OpenEventSourcing.EntityFrameworkCore.DbContexts;
using OpenEventSourcing.Events;
using OpenEventSourcing.Serialization;

namespace OpenEventSourcing.EntityFrameworkCore.Stores
{
    internal sealed class EntityFrameworkCoreEventStore : IEventStore
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly IEventDeserializer _eventDeserializer;
        private readonly IEventModelFactory _eventModelFactory;
        private readonly IEventTypeCache _eventTypeCache;
        private readonly ILogger<EntityFrameworkCoreEventStore> _logger;

        public EntityFrameworkCoreEventStore(IDbContextFactory dbContextFactory,
                                             IEventDeserializer eventDeserializer,
                                             IEventModelFactory eventModelFactory,
                                             IEventTypeCache eventTypeCache,
                                             ILogger<EntityFrameworkCoreEventStore> logger)
        {
            if (dbContextFactory == null)
                throw new ArgumentNullException(nameof(dbContextFactory));
            if (eventDeserializer == null)
                throw new ArgumentNullException(nameof(eventDeserializer));
            if (eventModelFactory == null)
                throw new ArgumentNullException(nameof(eventModelFactory));
            if (eventTypeCache == null)
                throw new ArgumentNullException(nameof(eventTypeCache));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _dbContextFactory = dbContextFactory;
            _eventDeserializer = eventDeserializer;
            _eventModelFactory = eventModelFactory;
            _eventTypeCache = eventTypeCache;
            _logger = logger;
        }

        public async Task<long> CountAsync(Guid aggregateId)
        {
            using (var context = _dbContextFactory.Create())
            {
                var count = await context.Events.LongCountAsync(@event => @event.AggregateId == aggregateId);

                return count;
            }
        }

        public async Task<Page> GetEventsAsync(long offset)
        {
            var results = new List<IIntegrationEvent>();

            using (var context = _dbContextFactory.Create())
            {
                var events = await context.Events
                        .AsNoTracking()
                        .OrderBy(x => x.SequenceNo)
                        .Skip((int)offset)
                        .Take(250)
                        .ToListAsync();

                foreach (var @event in events)
                {
                    if (!_eventTypeCache.TryGet(@event.Type, out var type))
                        throw new InvalidOperationException($"Cannot find type for event '{@event.Name}' - '{@event.Type}'.");

                    var result = (IEvent)_eventDeserializer.Deserialize(@event.Data, type);

                    results.Add(new IntegrationEvent(@event.Id, @event.AggregateId, result, @event.CorrelationId, @event.CausationId, @event.Timestamp, result.Version, @event.UserId));
                }

                return new Page(offset + events.Count, offset, results);
            }
        }
        public async Task<IEnumerable<IIntegrationEvent>> GetEventsAsync(Guid aggregateId)
        {
            var results = new List<IIntegrationEvent>();

            using (var context = _dbContextFactory.Create())
            {
                var events = await context.Events
                    .AsNoTracking()
                    .OrderBy(x => x.SequenceNo)
                    .Where(x => x.AggregateId == aggregateId)
                    .ToListAsync();

                foreach (var @event in events)
                {
                    if (!_eventTypeCache.TryGet(@event.Type, out var type))
                        throw new InvalidOperationException($"Cannot find type for event '{@event.Name}' - '{@event.Type}'.");

                    var result = (IEvent)_eventDeserializer.Deserialize(@event.Data, type);

                    results.Add(new IntegrationEvent(@event.Id, @event.AggregateId, result, @event.CorrelationId, @event.CausationId, @event.Timestamp, result.Version, @event.UserId));
                }

                return results;
            }
        }
        public async Task<IEnumerable<IIntegrationEvent>> GetEventsAsync(Guid aggregateId, long offset)
        {
            var results = new List<IIntegrationEvent>();

            using (var context = _dbContextFactory.Create())
            {
                var events = await context.Events
                    .AsNoTracking()
                    .Where(x => x.AggregateId == aggregateId)
                    .OrderBy(x => x.SequenceNo)
                    .Skip((int)offset)
                    .ToListAsync();

                foreach (var @event in events)
                {
                    if (!_eventTypeCache.TryGet(@event.Type, out var type))
                        throw new InvalidOperationException($"Cannot find type for event '{@event.Name}' - '{@event.Type}'.");

                    var result = (IEvent)_eventDeserializer.Deserialize(@event.Data, type);

                    results.Add(new IntegrationEvent(@event.Id, @event.AggregateId, result, @event.CorrelationId, @event.CausationId, @event.Timestamp, result.Version, @event.UserId));
                }

                return results;
            }
        }
        public async Task SaveAsync(IEnumerable<IEvent> events, ICommand causation)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));
            if (causation == null)
                throw new ArgumentNullException(nameof(causation));

            using (var context = _dbContextFactory.Create())
            {
                foreach (var @event in events)
                    await context.Events.AddAsync(_eventModelFactory.Create(@event, causation));

                await context.SaveChangesAsync();
            }
        }

        public async Task SaveAsync(IEnumerable<IEvent> events, IIntegrationEvent causation)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));
            if (causation == null)
                throw new ArgumentNullException(nameof(causation));

            using (var context = _dbContextFactory.Create())
            {
                foreach (var @event in events)
                    await context.Events.AddAsync(_eventModelFactory.Create(@event, causation));

                await context.SaveChangesAsync();
            }
        }

        public async Task SaveAsync(IEnumerable<IEvent> events, Guid? causationId = null, Guid? correlationId = null, string userId = null)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));

            using (var context = _dbContextFactory.Create())
            {
                foreach (var @event in events)
                    await context.Events.AddAsync(_eventModelFactory.Create(@event, causationId, correlationId, userId));

                await context.SaveChangesAsync();
            }
        }
    }
}
