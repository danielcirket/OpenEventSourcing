using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OpenEventSourcing.EntityFrameworkCore.InMemory;
using OpenEventSourcing.EntityFrameworkCore.Tests.Fakes;
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

            Action act = () => eventModelFactory.Create(@event: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("event");
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

            var @event = new FakeEvent(subject: Guid.NewGuid().ToString(), version: 1);
            var entity = eventModelFactory.Create(@event);

            entity.Subject.Should().Be(@event.Subject);
            entity.CausationId.Should().Be(@event.CausationId);
            entity.CorrelationId.Should().Be(@event.CorrelationId);
            entity.Data.Should().Be(eventSerializer.Serialize(@event));
            entity.Id.Should().Be(@event.Id);
            entity.Name.Should().Be(nameof(FakeEvent));
            entity.Timestamp.Should().Be(@event.Timestamp);
            entity.Type.Should().Be(@event.GetType().FullName);
            entity.UserId.Should().Be(@event.UserId);
        }
    }
}
