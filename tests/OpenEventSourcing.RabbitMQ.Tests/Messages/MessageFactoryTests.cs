using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Events;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.RabbitMQ.Extensions;
using OpenEventSourcing.RabbitMQ.Messages;
using OpenEventSourcing.Serialization;
using OpenEventSourcing.Serialization.Json.Extensions;
using Xunit;

namespace OpenEventSourcing.RabbitMQ.Tests.Messages
{
    public class MessageFactoryTests
    {
        public IServiceProvider ServiceProvider { get; }

        public MessageFactoryTests()
        {
            var services = new ServiceCollection();

            services.AddLogging(o => o.AddDebug())
                    .AddOpenEventSourcing()
                    .AddRabbitMq(o =>
                    {
                        o.UseConnection("amqp://guest:guest@localhost:5672/")
                         .UseExchange(e =>
                         {
                             e.WithName("test-exchange");
                             e.UseExchangeType("topic");
                         });
                    })
                    .AddJsonSerializers();

#if NETCOREAPP3_0
            ServiceProvider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true }).CreateScope().ServiceProvider;
#else
            ServiceProvider = services.BuildServiceProvider(validateScopes: true);
#endif
        }

        [Fact]
        public void WhenCreateMessageCalledWithNullEventThenShouldThrowArgumentNullException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var factory = scope.ServiceProvider.GetRequiredService<IMessageFactory>();

                Action act = () => factory.CreateMessage(null);

                act.Should().Throw<ArgumentNullException>()
                    .And.ParamName.Should().Be("event");
            }
        }
        [Fact]
        public void WhenCreateMessageCalledWithEventThenShouldPopulateMessageIdFromEventId()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var factory = scope.ServiceProvider.GetRequiredService<IMessageFactory>();
                var @event = new FakeEvent();
                var result = factory.CreateMessage(@event);

                result.MessageId.Should().Be(@event.Id);
            }
        }
        [Fact]
        public void WhenCreateMessageCalledWithEventThenShouldPopulateTypeFromEventTypeName()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var factory = scope.ServiceProvider.GetRequiredService<IMessageFactory>();
                var @event = new FakeEvent();
                var result = factory.CreateMessage(@event);

                result.Type.Should().Be(nameof(FakeEvent));
            }
        }
        [Fact]
        public void WhenCreateMessageCalledWithEventThenShouldPopulateCorrelationIdFromEvent()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var factory = scope.ServiceProvider.GetRequiredService<IMessageFactory>();
                var @event = new FakeEvent(correlationId: Guid.NewGuid());

                var result = factory.CreateMessage(@event);

                result.CorrelationId.Should().Be(@event.CorrelationId);
            }
        }
        [Fact]
        public void WhenCreateMessageCalledWithEventThenShouldPopulateBodyFromEvent()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var factory = scope.ServiceProvider.GetRequiredService<IMessageFactory>();
                var serializer = scope.ServiceProvider.GetRequiredService<IEventSerializer>();

                var @event = new FakeEvent();
                var body = serializer.Serialize(@event);
                var result = factory.CreateMessage(@event);

                result.Body.Should().Equal(body);
                result.Size.Should().Be(body.Length);
            }
        }

        private class FakeEvent : Event
        {
            public string Message { get; } = nameof(FakeEvent);
            
            public FakeEvent() 
                : base(Guid.NewGuid(), 1)
            {
            }
            public FakeEvent(Guid correlationId)
                : base(Guid.NewGuid(), 1)
            {
                CorrelationId = correlationId;
            }
        }
    }
}
