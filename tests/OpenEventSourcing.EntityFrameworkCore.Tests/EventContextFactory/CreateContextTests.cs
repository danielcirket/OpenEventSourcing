using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OpenEventSourcing.EntityFrameworkCore.InMemory;
using OpenEventSourcing.EntityFrameworkCore.Tests.Fakes;
using OpenEventSourcing.Events;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.Serialization;
using OpenEventSourcing.Serialization.Json.Extensions;
using Xunit;

namespace OpenEventSourcing.EntityFrameworkCore.Tests.EventContextFactory
{
    public class CreateContextTests
    {
        [Fact]
        public void WhenDbEventIsNullThenShouldThrowArgumentNullException()
        {
            var services = new ServiceCollection();

            services.AddOpenEventSourcing()
                    .AddEntityFrameworkCoreInMemory()
                    .AddJsonSerializers();

            var serviceProvider = services.BuildServiceProvider();

            var eventContextFactory = serviceProvider.GetRequiredService<IEventContextFactory>();

            Action act = () => eventContextFactory.CreateContext(dbEvent: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("dbEvent");
        }
        [Fact]
        public void WhenValidDbEventThenCreateContextShouldReturnEventContextWithMatchingEventType()
        {
            var services = new ServiceCollection();

            services.AddOpenEventSourcing()
                    .AddEntityFrameworkCoreInMemory()
                    .AddJsonSerializers();

            var serviceProvider = services.BuildServiceProvider();
            var eventContextFactory = serviceProvider.GetRequiredService<IEventContextFactory>();
            var serializer = serviceProvider.GetRequiredService<IEventSerializer>();
            var @event = new FakeEvent(Guid.NewGuid(), 1);

            var result = eventContextFactory.CreateContext(new Entities.Event
            {
                AggregateId = @event.AggregateId,
                CausationId = Guid.NewGuid(),
                CorrelationId = Guid.NewGuid(),
                Data = serializer.Serialize(@event),
                Id = Guid.NewGuid(),
                Name = nameof(FakeEvent),
                Timestamp = DateTimeOffset.UtcNow,
                Type = typeof(FakeEvent).FullName,
                UserId = "user-1",
            });

            result.Should().BeAssignableTo(typeof(IEventContext<FakeEvent>));
        }
    }
}
