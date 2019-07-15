using System;
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
            var eventDeserializer = new Mock<IEventDeserializer>().Object;
            var eventModelFactory = new Mock<IEventModelFactory>().Object;
            var eventTypeCache = new Mock<IEventTypeCache>().Object;
            var logger = new Mock<ILogger<EntityFrameworkCoreEventStore>>().Object;

            Action act = () => new EntityFrameworkCoreEventStore(dbContextFactory: dbContextFactory, eventDeserializer: eventDeserializer, eventModelFactory: eventModelFactory, eventTypeCache: eventTypeCache, logger: logger);

            act.Should().Throw<ArgumentNullException>().
                And.ParamName.Should().Be("dbContextFactory");
        }
        [Fact]
        public void WhenConstructedWithNullQuerySerializerShouldThrowArgumentNullException()
        {
            var dbContextFactory = new Mock<IDbContextFactory>().Object;
            IEventDeserializer eventDeserializer = null;
            var eventModelFactory = new Mock<IEventModelFactory>().Object;
            var eventTypeCache = new Mock<IEventTypeCache>().Object;
            var logger = new Mock<ILogger<EntityFrameworkCoreEventStore>>().Object;

            Action act = () => new EntityFrameworkCoreEventStore(dbContextFactory: dbContextFactory, eventDeserializer: eventDeserializer, eventModelFactory: eventModelFactory, eventTypeCache: eventTypeCache, logger: logger);

            act.Should().Throw<ArgumentNullException>().
                And.ParamName.Should().Be("eventDeserializer");
        }
        [Fact]
        public void WhenConstructedWithNullModelFactoryShouldThrowArgumentNullException()
        {
            var dbContextFactory = new Mock<IDbContextFactory>().Object;
            var eventDeserializer = new Mock<IEventDeserializer>().Object;
            IEventModelFactory eventModelFactory = null;
            var eventTypeCache = new Mock<IEventTypeCache>().Object;
            var logger = new Mock<ILogger<EntityFrameworkCoreEventStore>>().Object;

            Action act = () => new EntityFrameworkCoreEventStore(dbContextFactory: dbContextFactory, eventDeserializer: eventDeserializer, eventModelFactory: eventModelFactory, eventTypeCache: eventTypeCache, logger: logger);

            act.Should().Throw<ArgumentNullException>().
                And.ParamName.Should().Be("eventModelFactory");
        }
        [Fact]
        public void WhenConstructedWithNullEventTypeCacheShouldThrowArgumentNullException()
        {
            var dbContextFactory = new Mock<IDbContextFactory>().Object;
            var eventDeserializer = new Mock<IEventDeserializer>().Object;
            var eventModelFactory = new Mock<IEventModelFactory>().Object;
            IEventTypeCache eventTypeCache = null;
            var logger = new Mock<ILogger<EntityFrameworkCoreEventStore>>().Object;

            Action act = () => new EntityFrameworkCoreEventStore(dbContextFactory: dbContextFactory, eventDeserializer: eventDeserializer, eventModelFactory: eventModelFactory, eventTypeCache: eventTypeCache, logger: logger);

            act.Should().Throw<ArgumentNullException>().
                And.ParamName.Should().Be("eventTypeCache");
        }
        [Fact]
        public void WhenConstructedWithNullLoggerShouldThrowArgumentNullException()
        {
            var dbContextFactory = new Mock<IDbContextFactory>().Object;
            var eventDeserializer = new Mock<IEventDeserializer>().Object;
            var eventModelFactory = new Mock<IEventModelFactory>().Object;
            var eventTypeCache = new Mock<IEventTypeCache>().Object;
            ILogger<EntityFrameworkCoreEventStore> logger = null;

            Action act = () => new EntityFrameworkCoreEventStore(dbContextFactory: dbContextFactory, eventDeserializer: eventDeserializer, eventModelFactory: eventModelFactory, eventTypeCache: eventTypeCache, logger: logger);

            act.Should().Throw<ArgumentNullException>().
                And.ParamName.Should().Be("logger");
        }
        [Fact]
        public void WhenSavingNullAggregateThenShoudThrowArgumentNullException()
        {
            var dbContextFactory = new Mock<IDbContextFactory>().Object;
            var eventDeserializer = new Mock<IEventDeserializer>().Object;
            var eventModelFactory = new Mock<IEventModelFactory>().Object;
            var eventTypeCache = new Mock<IEventTypeCache>().Object;
            var logger = new Mock<ILogger<EntityFrameworkCoreEventStore>>().Object;
            var store = new EntityFrameworkCoreEventStore(dbContextFactory: dbContextFactory, eventDeserializer: eventDeserializer, eventModelFactory: eventModelFactory, eventTypeCache: eventTypeCache, logger: logger);

            FakeAggregate aggregate = null;

            Func<Task> act = async () => await store.SaveAsync<FakeAggregate, FakeAggregateState>(aggregate, 0);

            act.Should().Throw<ArgumentNullException>().
                And.ParamName.Should().Be("aggregate");
        }
        [Fact]
        public void WhenSavingAggregateWithNoUncommittedEventsThenShoudSaveNoEvents()
        {
            var dbContext = _serviceProvider.GetRequiredService<IDbContextFactory>().Create();
            var store = _serviceProvider.GetRequiredService<IEventStore>();
            var serializer = _serviceProvider.GetRequiredService<IEventSerializer>();
            var factory = _serviceProvider.GetRequiredService<IAggregateFactory>();

            var aggregate = factory.FromHistory<FakeAggregate, FakeAggregateState>(Enumerable.Empty<IEvent>());

            Func<Task> act = async () => await store.SaveAsync<FakeAggregate, FakeAggregateState>(aggregate, 0);

            var count = dbContext.Events.Count(e => e.AggregateId == aggregate.Id);
            count.Should().Be(0);
        }
        [Fact]
        public void WhenSavingAggregateWithDifferentExpectedVersionThenShoudThrowConcurrencyException()
        {
            var dbContext = _serviceProvider.GetRequiredService<IDbContextFactory>().Create();
            var store = _serviceProvider.GetRequiredService<IEventStore>();
            var serializer = _serviceProvider.GetRequiredService<IEventSerializer>();
            var factory = _serviceProvider.GetRequiredService<IAggregateFactory>();
            var id = Guid.NewGuid().ToSequentialGuid();

            var aggregate = factory.FromHistory<FakeAggregate, FakeAggregateState>(Enumerable.Empty<IEvent>());

            aggregate.FakeAction();

            Func<Task> act = async () => await store.SaveAsync<FakeAggregate, FakeAggregateState>(aggregate, 1);

            act.Should().Throw<ConcurrencyException>();
        }
        [Fact]
        public async Task WhenSavingAggregateWithSingleUncommittedEventsThenShoudSaveEvent()
        {
            var dbContext = _serviceProvider.GetRequiredService<IDbContextFactory>().Create();
            var store = _serviceProvider.GetRequiredService<IEventStore>();
            var serializer = _serviceProvider.GetRequiredService<IEventSerializer>();
            var factory = _serviceProvider.GetRequiredService<IAggregateFactory>();
            var id = Guid.NewGuid().ToSequentialGuid();

            var aggregate = factory.FromHistory<FakeAggregate, FakeAggregateState>(Enumerable.Empty<IEvent>());

            aggregate.FakeAction();

            await store.SaveAsync<FakeAggregate, FakeAggregateState>(aggregate, 0);

            var count = dbContext.Events.Count(e => e.AggregateId == aggregate.Id);
            count.Should().Be(1);
        }
        [Fact]
        public async Task WhenSavingAggregateWithMultipleUncommittedEventsThenShoudSaveEvents()
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

            await store.SaveAsync<FakeAggregate, FakeAggregateState>(aggregate, 0);

            var count = dbContext.Events.Count(e => e.AggregateId == aggregate.Id);
            count.Should().Be(3);
        }

        [Fact]
        public async Task WhenAggregateHasEventsThenGetAsyncShouldReturnExpectedEvents()
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
            aggregate.FakeAction();
            aggregate.FakeAction();

            await store.SaveAsync<FakeAggregate, FakeAggregateState>(aggregate, 0);

            var events = await store.GetEventsAsync(aggregate.Id.Value);

            events.Count().Should().Be(5);
        }
        [Fact]
        public async Task WhenAggregateHasEventsThenGetAsyncWithOffsetShouldReturnExpectedEvents()
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
            aggregate.FakeAction();
            aggregate.FakeAction();

            await store.SaveAsync<FakeAggregate, FakeAggregateState>(aggregate, 0);

            var events = await store.GetEventsAsync(aggregate.Id.Value, 3);

            events.Count().Should().Be(2);
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

            await store.SaveAsync<FakeAggregate, FakeAggregateState>(aggregate, 0);

            var total = dbContext.Events.Count();
            var page = await store.GetEventsAsync(0);

            page.PreviousOffset.Should().Be(0);
            page.Offset.Should().Be(total);
            page.Events.Count().Should().Be(total);
        }
    }
}
