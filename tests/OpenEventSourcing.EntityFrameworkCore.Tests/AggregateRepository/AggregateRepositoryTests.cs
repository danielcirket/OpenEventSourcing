using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OpenEventSourcing.Domain;
using OpenEventSourcing.EntityFrameworkCore.InMemory;
using OpenEventSourcing.EntityFrameworkCore.Tests.Fakes;
using OpenEventSourcing.Events;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.Serialization.Json.Extensions;
using Xunit;

namespace OpenEventSourcing.EntityFrameworkCore.Tests.AggregateRepository
{
    public class AggregateRepositoryTests
    {
        private readonly IServiceProvider _serviceProvider;

        public AggregateRepositoryTests()
        {
            var services = new ServiceCollection();

            services.AddOpenEventSourcing()
                    .AddEntityFrameworkCoreInMemory()
                    .AddJsonSerializers();

            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public void WhenConstructedWithNullDbContextFactoryShouldThrowArgumentNullException()
        {
            IEventStore eventStore = null;
            var factory = new Mock<IAggregateFactory>().Object;

            Action act = () => new EntityFrameworkCore.AggregateRepository(eventStore, factory);

            act.Should().Throw<ArgumentNullException>().
                And.ParamName.Should().Be("eventStore");
        }
        [Fact]
        public void WhenConstructedWithNullQuerySerializerShouldThrowArgumentNullException()
        {
            var eventStore = new Mock<IEventStore>().Object;
            IAggregateFactory aggregateFactory = null;

            Action act = () => new EntityFrameworkCore.AggregateRepository(eventStore, aggregateFactory);

            act.Should().Throw<ArgumentNullException>().
                And.ParamName.Should().Be("aggregateFactory");
        }
        [Fact]
        public async Task WhenNoEventsForAggregateThenGetAsyncShouldReturnNull()
        {
            var id = Guid.NewGuid();
            var repository = _serviceProvider.GetRequiredService<IAggregateRepository>();

            var aggregate = await repository.GetAsync<FakeAggregate, FakeAggregateState>(id);

            aggregate.Should().BeNull();
        }
        [Fact]
        public async Task WhenEventsForAggregateExistsThenGetAsyncShouldReturnAggregate()
        {
            var factory = _serviceProvider.GetRequiredService<IAggregateFactory>();
            var repository = _serviceProvider.GetRequiredService<IAggregateRepository>();
            var aggregate = factory.FromHistory<FakeAggregate, FakeAggregateState>(new IEvent[] { });

            aggregate.FakeAction();

            var id = aggregate.Id.Value;

            await repository.SaveAsync(aggregate);

            var result = await repository.GetAsync<FakeAggregate, FakeAggregateState>(id);

            result.Should().NotBeNull();
            result.Id.Should().Be(id);
            result.Version.Should().Be(1);
        }
        [Fact]
        public void WhenSaveAsyncCalledWithNullAggregateThenShouldThrowArgumentNullException()
        {
            var repository = _serviceProvider.GetRequiredService<IAggregateRepository>();

            Func<Task> act = async () => await repository.SaveAsync((FakeAggregate)null);

            act.Should().Throw<ArgumentNullException>();
        }
        [Fact]
        public async Task WhenSaveAsyncThenShouldSaveAggregateEvents()
        {
            var store = _serviceProvider.GetRequiredService<IEventStore>();
            var factory = _serviceProvider.GetRequiredService<IAggregateFactory>();

            var id = Guid.NewGuid();

            var repository = new EntityFrameworkCore.AggregateRepository(store, factory);
            var aggregate = factory.FromHistory<FakeAggregate, FakeAggregateState>(new IEvent[] { new FakeEvent(id, 1) });

            aggregate.FakeAction();

            await repository.SaveAsync(aggregate);

            var result = await repository.GetAsync<FakeAggregate, FakeAggregateState>(id);

            result.Id.Should().Be(id);
            result.Version.Should().Be(2);
        }
    }
}
