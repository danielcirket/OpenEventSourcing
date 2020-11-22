using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using OpenEventSourcing.Domain;
using OpenEventSourcing.EntityFrameworkCore.DbContexts;
using OpenEventSourcing.EntityFrameworkCore.InMemory;
using OpenEventSourcing.EntityFrameworkCore.Stores;
using OpenEventSourcing.EntityFrameworkCore.Tests.Fakes;
using OpenEventSourcing.Events;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.Serialization;
using OpenEventSourcing.Serialization.Json.Extensions;
using Xunit;

namespace OpenEventSourcing.EntityFrameworkCore.Tests.Stores.EventStore
{
    public class EventStoreTests
    {
        private readonly IServiceProvider _serviceProvider;

        public EventStoreTests()
        {
            var services = new ServiceCollection();

            services.AddOpenEventSourcing()
                    .AddEntityFrameworkCoreInMemory()
                    .AddJsonSerializers()
                    .Services
                    .AddLogging(configure => configure.AddConsole());

            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public void WhenConstructedWithNullDbContextFactoryShouldThrowArgumentNullException()
        {
            IDbContextFactory dbContextFactory = null;
            var eventContextFactory = new Mock<IEventContextFactory>().Object;
            var eventModelFactory = new Mock<IEventModelFactory>().Object;
            var eventTypeCache = new Mock<IEventTypeCache>().Object;
            var logger = new Mock<ILogger<EntityFrameworkCoreEventStore>>().Object;

            Action act = () => new EntityFrameworkCoreEventStore(dbContextFactory: dbContextFactory, eventContextFactory: eventContextFactory, eventModelFactory: eventModelFactory, eventTypeCache: eventTypeCache, logger: logger);

            act.Should().Throw<ArgumentNullException>().
                And.ParamName.Should().Be("dbContextFactory");
        }
        [Fact]
        public void WhenConstructedWithNullQuerySerializerShouldThrowArgumentNullException()
        {
            var dbContextFactory = new Mock<IDbContextFactory>().Object;
            IEventContextFactory eventContextFactory = null;
            var eventModelFactory = new Mock<IEventModelFactory>().Object;
            var eventTypeCache = new Mock<IEventTypeCache>().Object;
            var logger = new Mock<ILogger<EntityFrameworkCoreEventStore>>().Object;

            Action act = () => new EntityFrameworkCoreEventStore(dbContextFactory: dbContextFactory, eventContextFactory: eventContextFactory, eventModelFactory: eventModelFactory, eventTypeCache: eventTypeCache, logger: logger);

            act.Should().Throw<ArgumentNullException>().
                And.ParamName.Should().Be("eventContextFactory");
        }
        [Fact]
        public void WhenConstructedWithNullModelFactoryShouldThrowArgumentNullException()
        {
            var dbContextFactory = new Mock<IDbContextFactory>().Object;
            var eventContextFactory = new Mock<IEventContextFactory>().Object;
            IEventModelFactory eventModelFactory = null;
            var eventTypeCache = new Mock<IEventTypeCache>().Object;
            var logger = new Mock<ILogger<EntityFrameworkCoreEventStore>>().Object;

            Action act = () => new EntityFrameworkCoreEventStore(dbContextFactory: dbContextFactory, eventContextFactory: eventContextFactory, eventModelFactory: eventModelFactory, eventTypeCache: eventTypeCache, logger: logger);

            act.Should().Throw<ArgumentNullException>().
                And.ParamName.Should().Be("eventModelFactory");
        }
        [Fact]
        public void WhenConstructedWithNullEventTypeCacheShouldThrowArgumentNullException()
        {
            var dbContextFactory = new Mock<IDbContextFactory>().Object;
            var eventContextFactory = new Mock<IEventContextFactory>().Object;
            var eventModelFactory = new Mock<IEventModelFactory>().Object;
            IEventTypeCache eventTypeCache = null;
            var logger = new Mock<ILogger<EntityFrameworkCoreEventStore>>().Object;

            Action act = () => new EntityFrameworkCoreEventStore(dbContextFactory: dbContextFactory, eventContextFactory: eventContextFactory, eventModelFactory: eventModelFactory, eventTypeCache: eventTypeCache, logger: logger);

            act.Should().Throw<ArgumentNullException>().
                And.ParamName.Should().Be("eventTypeCache");
        }
        [Fact]
        public void WhenConstructedWithNullLoggerShouldThrowArgumentNullException()
        {
            var dbContextFactory = new Mock<IDbContextFactory>().Object;
            var eventContextFactory = new Mock<IEventContextFactory>().Object;
            var eventModelFactory = new Mock<IEventModelFactory>().Object;
            var eventTypeCache = new Mock<IEventTypeCache>().Object;
            ILogger<EntityFrameworkCoreEventStore> logger = null;

            Action act = () => new EntityFrameworkCoreEventStore(dbContextFactory: dbContextFactory, eventContextFactory: eventContextFactory, eventModelFactory: eventModelFactory, eventTypeCache: eventTypeCache, logger: logger);

            act.Should().Throw<ArgumentNullException>().
                And.ParamName.Should().Be("logger");
        }
        
        [Fact]
        public async Task WhenAggregateHasEventsThenGetAsyncShouldReturnExpectedEvents()
        {
            var store = _serviceProvider.GetRequiredService<IEventStore>();
            var factory = _serviceProvider.GetRequiredService<IAggregateFactory>();

            var aggregate = factory.FromHistory<FakeAggregate, FakeAggregateState>(Enumerable.Empty<IEvent>());

            aggregate.FakeAction();
            aggregate.FakeAction();
            aggregate.FakeAction();
            aggregate.FakeAction();
            aggregate.FakeAction();

            var contexts = aggregate.GetUncommittedEvents().Select(@event => new EventContext<IEvent>(streamId: aggregate.Id, @event: @event, correlationId: null, causationId: null, timestamp: DateTimeOffset.UtcNow, userId: null));
            
            await store.SaveAsync(aggregate.Id, contexts);

            var events = await store.GetEventsAsync(aggregate.Id);

            events.Count().Should().Be(5);
        }
        [Fact]
        public async Task WhenAggregateHasEventsThenGetAsyncWithOffsetShouldReturnExpectedEvents()
        {
            var store = _serviceProvider.GetRequiredService<IEventStore>();
            var factory = _serviceProvider.GetRequiredService<IAggregateFactory>();

            var aggregate = factory.FromHistory<FakeAggregate, FakeAggregateState>(Enumerable.Empty<IEvent>());

            aggregate.FakeAction();
            aggregate.FakeAction();
            aggregate.FakeAction();
            aggregate.FakeAction();
            aggregate.FakeAction();

            var contexts = aggregate.GetUncommittedEvents().Select(@event => new EventContext<IEvent>(streamId: aggregate.Id, @event: @event, correlationId: null, causationId: null, timestamp: DateTimeOffset.UtcNow, userId: null));

            await store.SaveAsync(aggregate.Id, contexts);

            var events = await store.GetEventsAsync(aggregate.Id, 3);

            events.Count().Should().Be(3);
        }
        [Fact]
        public async Task WhenStoreContainsEventsThenGetAsyncWithOffsetShouldReturnExpectedEvents()
        {
            var dbContext = _serviceProvider.GetRequiredService<IDbContextFactory>().Create();
            var store = _serviceProvider.GetRequiredService<IEventStore>();
            var factory = _serviceProvider.GetRequiredService<IAggregateFactory>();
            var aggregate = factory.FromHistory<FakeAggregate, FakeAggregateState>(Enumerable.Empty<IEvent>());

            aggregate.FakeAction();
            aggregate.FakeAction();
            aggregate.FakeAction();
            aggregate.FakeAction();
            aggregate.FakeAction();

            var contexts = aggregate.GetUncommittedEvents().Select(@event => new EventContext<IEvent>(streamId: aggregate.Id, @event: @event, correlationId: null, causationId: null, timestamp: DateTimeOffset.UtcNow, userId: null));

            await store.SaveAsync(aggregate.Id, contexts);

            var total = dbContext.Events.Count();
            var page = await store.GetEventsAsync(0);

            page.PreviousOffset.Should().Be(0);
            page.Offset.Should().Be(total);
            page.Events.Count().Should().Be(total);
        }


        [Fact]
        public void WhenSavingNullEventsThenShouldThrowArgumentNullException()
        {
            var dbContextFactory = new Mock<IDbContextFactory>().Object;
            var eventContextFactory = new Mock<IEventContextFactory>().Object;
            var eventModelFactory = new Mock<IEventModelFactory>().Object;
            var eventTypeCache = new Mock<IEventTypeCache>().Object;
            var logger = new Mock<ILogger<EntityFrameworkCoreEventStore>>().Object;
            var store = new EntityFrameworkCoreEventStore(dbContextFactory: dbContextFactory, eventContextFactory: eventContextFactory, eventModelFactory: eventModelFactory, eventTypeCache: eventTypeCache, logger: logger);

            IEnumerable<IEventContext<IEvent>> contexts = null;

            Func<Task> act = async () => await store.SaveAsync("fake-stream", contexts);

            act.Should().Throw<ArgumentNullException>().
                And.ParamName.Should().Be("events");
        }
        [Fact]
        public async Task WhenSavingSingleEventThenShouldStoreEvent()
        {
            var dbContext = _serviceProvider.GetRequiredService<IDbContextFactory>().Create();
            var store = _serviceProvider.GetRequiredService<IEventStore>();
            var serializer = _serviceProvider.GetRequiredService<IEventSerializer>();
            var factory = _serviceProvider.GetRequiredService<IAggregateFactory>();
            var id = Guid.NewGuid().ToSequentialGuid();

            var aggregate = factory.FromHistory<FakeAggregate, FakeAggregateState>(Enumerable.Empty<IEvent>());

            aggregate.FakeAction();

            var contexts = aggregate.GetUncommittedEvents().Select(@event => new EventContext<IEvent>(streamId: aggregate.Id, @event: @event, correlationId: null, causationId: null, timestamp: DateTimeOffset.UtcNow, userId: null));

            await store.SaveAsync(aggregate.Id, contexts);

            var count = dbContext.Events.Count(e => e.StreamId == aggregate.Id);
            count.Should().Be(1);
        }
        [Fact]
        public async Task WhenSavingMulitpleEventsThenShouldStoreExpectedEvents()
        {
            var dbContext = _serviceProvider.GetRequiredService<IDbContextFactory>().Create();
            var store = _serviceProvider.GetRequiredService<IEventStore>();
            var serializer = _serviceProvider.GetRequiredService<IEventSerializer>();
            var factory = _serviceProvider.GetRequiredService<IAggregateFactory>();
            var id = Guid.NewGuid().ToSequentialGuid();

            var aggregate = factory.FromHistory<FakeAggregate, FakeAggregateState>(Enumerable.Empty<IEvent>());

            aggregate.FakeAction();
            aggregate.FakeAction();
            aggregate.FakeAction();

            var contexts = aggregate.GetUncommittedEvents().Select(@event => new EventContext<IEvent>(streamId: aggregate.Id, @event: @event, correlationId: null, causationId: null, timestamp: DateTimeOffset.UtcNow, userId: null));

            await store.SaveAsync(aggregate.Id, contexts);

            var count = dbContext.Events.Count(e => e.StreamId == aggregate.Id);
            count.Should().Be(3);
        }
    }
}
