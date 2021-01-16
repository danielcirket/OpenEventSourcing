using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.EntityFrameworkCore.DbContexts;
using OpenEventSourcing.Events;
using OpenEventSourcing.Serialization;

namespace OpenEventSourcing.EntityFrameworkCore.Stores
{
    internal sealed class EntityFrameworkCoreEventStore : IEventStore
    {
        private static readonly int DefaultReloadInterval = 2000;
        private static readonly int DefaultPageSize = 500;

        private readonly IDbContextFactory _dbContextFactory;
        private readonly IEventContextFactory _eventContextFactory;
        private readonly IEventModelFactory _eventModelFactory;
        private readonly IEventTypeCache _eventTypeCache;
        private readonly ILogger<EntityFrameworkCoreEventStore> _logger;

        public EntityFrameworkCoreEventStore(IDbContextFactory dbContextFactory,
                                             IEventContextFactory eventContextFactory,
                                             IEventModelFactory eventModelFactory,
                                             IEventTypeCache eventTypeCache,
                                             ILogger<EntityFrameworkCoreEventStore> logger)
        {
            if (dbContextFactory == null)
                throw new ArgumentNullException(nameof(dbContextFactory));
            if (eventContextFactory == null)
                throw new ArgumentNullException(nameof(eventContextFactory));
            if (eventModelFactory == null)
                throw new ArgumentNullException(nameof(eventModelFactory));
            if (eventTypeCache == null)
                throw new ArgumentNullException(nameof(eventTypeCache));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _dbContextFactory = dbContextFactory;
            _eventContextFactory = eventContextFactory;
            _eventModelFactory = eventModelFactory;
            _eventTypeCache = eventTypeCache;
            _logger = logger;
        }

        public async Task<long> CountAsync(string streamId, CancellationToken cancellationToken = default)
        {
            using (var context = _dbContextFactory.Create())
            {
                var count = await context.Events.LongCountAsync(@event => @event.StreamId == streamId, cancellationToken);

                return count;
            }
        }

        public async Task<Page> GetEventsAsync(long offset, CancellationToken cancellationToken = default)
        {
            var results = new List<IEventContext<IEvent>>();

            var events = await GetAllEventsForwardsInternalAsync(offset).ConfigureAwait(false);

            if (events.Count > 0 && events[0].SequenceNo != offset + 1)
            {
                _logger.LogInformation("Gap detected in stream. Expecting sequence no {ExpectedSequenceNo} but found sequence no {ActualSequenceNo}. Reloading events after {DefaultReloadInterval}ms.", offset + 1, events[0].SequenceNo, DefaultReloadInterval);
                events = await GetAllEventsAfterDelayInternalAsync(offset).ConfigureAwait(false);
            }

            for (var i = 0; i < events.Count - 1; i++)
            {
                if (events[i].SequenceNo + 1 != events[i + 1].SequenceNo)
                {
                    _logger.LogInformation("Gap detected in stream. Expecting sequence no {ExpectedSequenceNo} but found sequence no {ActualSequenceNo}. Reloading events after {DefaultReloadInterval}ms.", events[i].SequenceNo + 1, events[i + 1].SequenceNo, DefaultReloadInterval);
                    events = await GetAllEventsAfterDelayInternalAsync(offset).ConfigureAwait(false);
                    break;
                }
            }

            foreach (var @event in events)
            {
                if (!_eventTypeCache.TryGet(@event.Type, out var type))
                    throw new InvalidOperationException($"Cannot find type for event '{@event.Name}' - '{@event.Type}'.");

                var result = _eventContextFactory.CreateContext(@event);
                //var result = (IEvent)_eventDeserializer.Deserialize(@event.Data, type);

                results.Add(result);
            }

            return new Page(offset + events.Count, offset, results);
        }
        public async Task<IEnumerable<IEventContext<IEvent>>> GetEventsAsync(string streamId, CancellationToken cancellationToken = default)
        {
            var results = new List<IEventContext<IEvent>>();

            var events = await GetAllEventsForwardsForStreamInternalAsync(streamId, 0).ConfigureAwait(false);

            foreach (var @event in events)
            {
                if (!_eventTypeCache.TryGet(@event.Type, out var type))
                    throw new InvalidOperationException($"Cannot find type for event '{@event.Name}' - '{@event.Type}'.");

                var result = _eventContextFactory.CreateContext(@event);
                //var result = (IEvent)_eventDeserializer.Deserialize(@event.Data, type);

                results.Add(result);
            }

            return results;

        }
        public async Task<IEnumerable<IEventContext<IEvent>>> GetEventsAsync(string streamId, long offset, CancellationToken cancellationToken = default)
        {
            var results = new List<IEventContext<IEvent>>();

            var events = await GetAllEventsForwardsForStreamInternalAsync(streamId, offset).ConfigureAwait(false);

            foreach (var @event in events)
            {
                if (!_eventTypeCache.TryGet(@event.Type, out var type))
                    throw new InvalidOperationException($"Cannot find type for event '{@event.Name}' - '{@event.Type}'.");

                var result = _eventContextFactory.CreateContext(@event);
                //var result = (IEvent)_eventDeserializer.Deserialize(@event.Data, type);

                results.Add(result);
            }

            return results;
        }

        public async Task SaveAsync(string streamId, IEnumerable<IEventContext<IEvent>> events, CancellationToken cancellationToken = default)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));

            using (var context = _dbContextFactory.Create())
            {
                foreach (var @event in events)
                    await context.Events.AddAsync(_eventModelFactory.Create(streamId, @event));

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        private async Task<List<Entities.Event>> GetAllEventsForwardsInternalAsync(long offset)
        {
            using (var context = _dbContextFactory.Create())
            {
                var events = await context.Events.OrderBy(e => e.SequenceNo)
                                                 .Where(e => e.SequenceNo >= offset)
                                                 .Take(DefaultPageSize)
                                                 .AsNoTracking()
                                                 .ToListAsync();

                return events;
            }
        }
        private async Task<List<Entities.Event>> GetAllEventsForwardsForStreamInternalAsync(string streamId, long offset)
        {
            using (var context = _dbContextFactory.Create())
            {
                var events = await context.Events.OrderBy(e => e.SequenceNo)
                                                 .Where(e => e.SequenceNo >= offset)
                                                 .Where(e => e.StreamId == streamId)
                                                 .Take(DefaultPageSize)
                                                 .AsNoTracking()
                                                 .ToListAsync();

                return events;
            }
        }
        private async Task<List<Entities.Event>> GetAllEventsAfterDelayInternalAsync(long offset)
        {
            _logger.LogInformation("Reloading events after {DefaultReloadInterval}ms.", DefaultReloadInterval);

            await Task.Delay(DefaultReloadInterval).ConfigureAwait(false);
            return await GetAllEventsForwardsInternalAsync(offset).ConfigureAwait(false);
        }
    }
}
