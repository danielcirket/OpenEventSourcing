using System;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Events;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.RabbitMQ.Extensions;
using OpenEventSourcing.RabbitMQ.Messages;
using OpenEventSourcing.Serialization;
using OpenEventSourcing.Serialization.Json.Extensions;
using Xunit;

namespace OpenEventSourcing.RabbitMQ.Tests.Messages.MessageFactory
{
    public class CreateMessageTests : IClassFixture<ConfigurationFixture>
    {
        public IServiceProvider ServiceProvider { get; }

        public CreateMessageTests(ConfigurationFixture fixture)
        {
            var services = new ServiceCollection();

            services.AddLogging(o => o.AddDebug())
                    .AddOpenEventSourcing()
                    .AddRabbitMq(o =>
                    {
                        o.UseConnection(fixture.Configuration.GetValue<string>("RabbitMQ:ConnectionString"))
                         .UseExchange(e =>
                         {
                             e.WithName("test-exchange");
                             e.UseExchangeType("topic");
                             e.AutoDelete();
                         });
                    })
                    .AddJsonSerializers();

#if NETCOREAPP3_0 || NETCOREAPP3_1
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
                    .And.ParamName.Should().Be("context");
            }
        }
        [Fact]
        public void WhenCreateMessageCalledWithEventThenShouldPopulateMessageIdFromEventId()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var factory = scope.ServiceProvider.GetRequiredService<IMessageFactory>();
                var @event = new FakeEvent();
                var notification = new EventNotification<FakeEvent>(streamId: @event.Subject, @event: @event, correlationId: null, causationId: null, timestamp: @event.Timestamp, userId: null);
                var result = factory.CreateMessage(notification);

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
                var notification = new EventNotification<FakeEvent>(streamId: @event.Subject, @event: @event, correlationId: null, causationId: null, timestamp: @event.Timestamp, userId: null);
                var result = factory.CreateMessage(notification);

                result.Type.Should().Be(nameof(FakeEvent));
            }
        }
        [Fact]
        public void WhenCreateMessageCalledWithEventThenShouldPopulateCorrelationIdFromEvent()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var factory = scope.ServiceProvider.GetRequiredService<IMessageFactory>();
                var @event = new FakeEvent();
                var notification = new EventNotification<FakeEvent>(streamId: @event.Subject, @event: @event, correlationId: Guid.NewGuid().ToString(), causationId: null, timestamp: @event.Timestamp, userId: null);

                var result = factory.CreateMessage(notification);

                result.CorrelationId.Should().Be(notification.CorrelationId);
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
                var notification = new EventNotification<FakeEvent>(streamId: @event.Subject, @event: @event, correlationId: Guid.NewGuid().ToString(), causationId: null, timestamp: @event.Timestamp, userId: null);
                var result = factory.CreateMessage(notification);

                result.Body.Should().Equal(body);
                result.Size.Should().Be(body.Length);
            }
        }

        private class FakeEvent : Event
        {
            public string Message { get; } = nameof(FakeEvent);
            
            public FakeEvent() 
                : base(Guid.NewGuid().ToString(), 1)
            {
            }
        }
    }
}
