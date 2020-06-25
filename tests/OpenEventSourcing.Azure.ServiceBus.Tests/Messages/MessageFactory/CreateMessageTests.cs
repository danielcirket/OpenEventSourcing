using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.Azure.ServiceBus.Extensions;
using OpenEventSourcing.Serialization.Json.Extensions;
using FluentAssertions;
using Xunit;
using OpenEventSourcing.Azure.ServiceBus.Messages;
using OpenEventSourcing.Events;
using OpenEventSourcing.Serialization;

namespace OpenEventSourcing.Azure.ServiceBus.Tests.Messages.MessageFactory
{
    public class CreateMessageTests
    {
        public IServiceProvider ServiceProvider { get; }

        public CreateMessageTests()
        {
            var services = new ServiceCollection();

            services.AddLogging(o => o.AddDebug())
                    .AddOpenEventSourcing()
                    .AddAzureServiceBus(o =>
                    {
                        o.UseConnection(Environment.GetEnvironmentVariable("AZURE_SERVICE_BUS_CONNECTION_STRING") ?? "Endpoint=sb://openeventsourcing.servicebus.windows.net/;SharedAccessKeyName=DUMMY;SharedAccessKey=DUMMY")
                         .UseTopic(t =>
                         {
                             t.WithName("test-exchange");
                             t.AutoDeleteOnIdleAfter(TimeSpan.FromMinutes(5));
                         });
                    })
                    .AddJsonSerializers();

#if NETCOREAPP3_0
            ServiceProvider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true }).CreateScope().ServiceProvider;
#else
            ServiceProvider = services.BuildServiceProvider(validateScopes: true);
#endif
        }

        [ServiceBusTest]
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
        [ServiceBusTest]
        public void WhenCreateMessageCalledWithEventThenShouldPopulateMessageIdFromEventId()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var factory = scope.ServiceProvider.GetRequiredService<IMessageFactory>();
                var @event = new FakeEvent();
                var notification = new EventNotification<FakeEvent>(@event, correlationId: Guid.NewGuid(), causationId: null, timestamp: @event.Timestamp, userId: null);
                var result = factory.CreateMessage(notification);

                result.MessageId.Should().Be(@event.Id.ToString());
            }
        }
        [ServiceBusTest]
        public void WhenCreateMessageCalledWithEventThenShouldPopulateTypeFromEventTypeName()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var factory = scope.ServiceProvider.GetRequiredService<IMessageFactory>();
                var @event = new FakeEvent();
                var notification = new EventNotification<FakeEvent>(@event, correlationId: Guid.NewGuid(), causationId: null, timestamp: @event.Timestamp, userId: null);
                var result = factory.CreateMessage(notification);

                result.Label.Should().Be(nameof(FakeEvent));
            }
        }
        [ServiceBusTest]
        public void WhenCreateMessageCalledWithEventThenShouldPopulateCorrelationIdFromEvent()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var factory = scope.ServiceProvider.GetRequiredService<IMessageFactory>();
                var @event = new FakeEvent(correlationId: Guid.NewGuid());
                var notification = new EventNotification<FakeEvent>(@event, correlationId: Guid.NewGuid(), causationId: null, timestamp: @event.Timestamp, userId: null);
                var result = factory.CreateMessage(notification);

                result.CorrelationId.Should().Be(notification.CorrelationId.ToString());
            }
        }
        [ServiceBusTest]
        public void WhenCreateMessageCalledWithEventThenShouldPopulateBodyFromEvent()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var factory = scope.ServiceProvider.GetRequiredService<IMessageFactory>();
                var serializer = scope.ServiceProvider.GetRequiredService<IEventSerializer>();

                var @event = new FakeEvent();
                var notification = new EventNotification<FakeEvent>(@event, correlationId: null, causationId: null, timestamp: @event.Timestamp, userId: null);
                var body = serializer.Serialize(@event);
                var result = factory.CreateMessage(notification);

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
            }
        }
    }
}
