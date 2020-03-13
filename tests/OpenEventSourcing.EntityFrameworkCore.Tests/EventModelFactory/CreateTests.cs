using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OpenEventSourcing.Commands;
using OpenEventSourcing.EntityFrameworkCore.InMemory;
using OpenEventSourcing.EntityFrameworkCore.Tests.Fakes;
using OpenEventSourcing.Events;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.Serialization;
using OpenEventSourcing.Serialization.Json.Extensions;
using Xunit;

namespace OpenEventSourcing.EntityFrameworkCore.Tests.EventModelFactory
{
    public class CreateTests
    {
        [Fact]
        public void WhenCreateCalledWithNullEventThenShouldThrowArgumentNullException()
        {
            var services = new ServiceCollection();

            services.AddOpenEventSourcing()
                    .AddEntityFrameworkCoreInMemory()
                    .AddJsonSerializers();

            var serviceProvider = services.BuildServiceProvider();

            var eventModelFactory = serviceProvider.GetRequiredService<IEventModelFactory>();

            Action act = () => eventModelFactory.Create(@event: null, causation: (ICommand)null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("event");
        }
        [Fact]
        public void WhenCreateCalledWithNullCausationCommandThenShouldThrowArgumentNullException()
        {
            var services = new ServiceCollection();

            services.AddOpenEventSourcing()
                    .AddEntityFrameworkCoreInMemory()
                    .AddJsonSerializers();

            var serviceProvider = services.BuildServiceProvider();

            var eventModelFactory = serviceProvider.GetRequiredService<IEventModelFactory>();

            Action act = () => eventModelFactory.Create(@event: new FakeEvent(Guid.NewGuid(), 1), causation: (ICommand)null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("causation");
        }
        [Fact]
        public void WhenCreateCalledWithNullCausationEventThenShouldThrowArgumentNullException()
        {
            var services = new ServiceCollection();

            services.AddOpenEventSourcing()
                    .AddEntityFrameworkCoreInMemory()
                    .AddJsonSerializers();

            var serviceProvider = services.BuildServiceProvider();

            var eventModelFactory = serviceProvider.GetRequiredService<IEventModelFactory>();

            Action act = () => eventModelFactory.Create(@event: new FakeEvent(Guid.NewGuid(), 1), causation: (IIntegrationEvent)null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("causation");
        }
        [Fact]
        public void WhenCreateCalledWithEventThenShouldReturnEntityWithExpectedPropertiesPopulated()
        {
            var services = new ServiceCollection();

            services.AddOpenEventSourcing()
                    .AddEntityFrameworkCoreInMemory()
                    .AddJsonSerializers();

            var serviceProvider = services.BuildServiceProvider();
            var eventModelFactory = serviceProvider.GetRequiredService<IEventModelFactory>();
            var eventSerializer = serviceProvider.GetRequiredService<IEventSerializer>();

            var @event = new FakeEvent(aggregateId: Guid.NewGuid(), version: 1);
            var causation = new FakeCommand();
            var entity = eventModelFactory.Create(@event, causation);

            entity.AggregateId.Should().Be(@event.AggregateId);
            entity.CausationId.Should().Be(causation.Id);
            entity.CorrelationId.Should().Be(causation.CorrelationId);
            entity.Data.Should().Be(eventSerializer.Serialize(@event));
            entity.Id.Should().Be(@event.Id);
            entity.Name.Should().Be(nameof(FakeEvent));
            entity.Timestamp.Should().Be(@event.Timestamp);
            entity.Type.Should().Be(@event.GetType().FullName);
            entity.UserId.Should().Be(causation.UserId);
        }
    }
}
