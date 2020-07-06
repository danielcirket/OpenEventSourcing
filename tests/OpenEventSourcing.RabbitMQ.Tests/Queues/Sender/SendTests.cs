using System;
using System.Collections.Generic;
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

                Func<Task> act = async () => await sender.SendAsync((IEvent)null);

                act.Should().Throw<ArgumentNullException>()
                    .And.ParamName.Should().Be("event");
            }
        }
        [RabbitMqTest]
        public void WhenSendAsyncCalledWithNullEventsThenShouldThrowArgumentNullException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var sender = scope.ServiceProvider.GetRequiredService<IQueueMessageSender>();

                Func<Task> act = async () => await sender.SendAsync((IEnumerable<IEvent>)null);

                act.Should().Throw<ArgumentNullException>()
                    .And.ParamName.Should().Be("events");
            }
        }
        [RabbitMqTest]
        public void WhenSendAsyncCalledWithSingleEventThenShouldSendEvent()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var sender = scope.ServiceProvider.GetRequiredService<IQueueMessageSender>();
                var @event = new SameSenderEvent();

                Func<Task> act = async () => await sender.SendAsync(@event);

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

                Func<Task> act = async () => await sender.SendAsync(events);

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
