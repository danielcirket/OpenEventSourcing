using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Events;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.RabbitMQ.Extensions;
using OpenEventSourcing.RabbitMQ.Queues;
using OpenEventSourcing.Serialization.Json.Extensions;
using Xunit;

namespace OpenEventSourcing.RabbitMQ.Tests.Queues.Sender
{
    public class SendTests : IClassFixture<ConfigurationFixture>
    {
        public IServiceProvider ServiceProvider { get; }

        public SendTests(ConfigurationFixture fixture)
        {
            var services = new ServiceCollection();

            services.AddLogging(o => o.AddDebug())
                    .AddOpenEventSourcing()
                    .AddRabbitMq(o =>
                    {
                        o.UseConnection(fixture.Configuration.GetValue<string>("RabbitMQ:ConnectionString"))
                         .UseExchange(e =>
                         {
                             e.WithName($"test-exchange-{Guid.NewGuid()}");
                             e.UseExchangeType("topic");
                             e.AutoDelete();
                         });
                    })
                    .AddJsonSerializers();

#if NETCOREAPP3_0 || NETCOREAPP3_1
            ServiceProvider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
#else
            ServiceProvider = services.BuildServiceProvider(validateScopes: true);
#endif
        }

        [RabbitMqTest]
        public void WhenSendAsyncCalledWithSingleNullEventThenShouldThrowArgumentNullException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var sender = scope.ServiceProvider.GetRequiredService<IQueueMessageSender>();

                Func<Task> act = async () => await sender.SendAsync((IEventNotification<IEvent>)null);

                act.Should().Throw<ArgumentNullException>()
                    .And.ParamName.Should().Be("context");
            }
        }
        [RabbitMqTest]
        public void WhenSendAsyncCalledWithNullEventsThenShouldThrowArgumentNullException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var sender = scope.ServiceProvider.GetRequiredService<IQueueMessageSender>();

                Func<Task> act = async () => await sender.SendAsync((IEnumerable<IEventNotification<IEvent>>)null);

                act.Should().Throw<ArgumentNullException>()
                    .And.ParamName.Should().Be("contexts");
            }
        }
        [RabbitMqTest]
        public void WhenSendAsyncCalledWithSingleEventThenShouldSendEvent()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var sender = scope.ServiceProvider.GetRequiredService<IQueueMessageSender>();
                var @event = new SameSenderEvent();
                var notification = new EventNotification<SameSenderEvent>(@event, correlationId: null, causationId: null, timestamp: DateTimeOffset.UtcNow, userId: null);

                Func<Task> act = async () => await sender.SendAsync(notification);

                act.Should().NotThrow();
            }
        }
        [RabbitMqTest]
        public void WhenSendAsyncCalledWithMultipleEventsThenShouldSendEvents()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var sender = scope.ServiceProvider.GetRequiredService<IQueueMessageSender>();
                var events = new[] { new SameSenderEvent(), new SameSenderEvent() };
                var notifications = events.Select(@event => new EventNotification<SameSenderEvent>(@event, correlationId: null, causationId: null, timestamp: DateTimeOffset.UtcNow, userId: null));

                Func<Task> act = async () => await sender.SendAsync(notifications);

                act.Should().NotThrow();
            }
        }

        private class SameSenderEvent : Event
        {
            public SameSenderEvent() : base(Guid.NewGuid(), 1)
            {
            }
        }
    }
}
