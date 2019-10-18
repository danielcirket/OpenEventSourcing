using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Events;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.RabbitMQ.Extensions;
using OpenEventSourcing.RabbitMQ.Queues;
using OpenEventSourcing.Serialization.Json.Extensions;
using OpenEventSourcing.Testing.Attributes;

namespace OpenEventSourcing.RabbitMQ.Tests.Queues.Sender
{
    public class SendTests
    {
        public IServiceProvider ServiceProvider { get; }

        public SendTests()
        {
            var services = new ServiceCollection();

            services.AddLogging(o => o.AddDebug())
                    .AddOpenEventSourcing()
                    .AddRabbitMq(o =>
                    {
                        o.UseConnection("amqp://guest:guest@localhost:5672/")
                         .UseExchange("test-exchange");
                    })
                    .AddJsonSerializers();

            ServiceProvider = services.BuildServiceProvider();
        }

        [IntegrationTest]
        public void WhenSendAsyncCalledWithSingleNullEventThenShouldThrowArgumentNullException()
        {
            var sender = ServiceProvider.GetRequiredService<IQueueMessageSender>();

            Func<Task> act = async () => await sender.SendAsync((IEvent)null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("event");
        }
        [IntegrationTest]
        public void WhenSendAsyncCalledWithNullEventsThenShouldThrowArgumentNullException()
        {
            var sender = ServiceProvider.GetRequiredService<IQueueMessageSender>();

            Func<Task> act = async () => await sender.SendAsync((IEnumerable<IEvent>)null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("events");
        }
        [IntegrationTest]
        public void WhenSendAsyncCalledWithSingleEventThenShouldSendEvent()
        {
            var sender = ServiceProvider.GetRequiredService<IQueueMessageSender>();
            var @event = new SameSenderEvent();

            Func<Task> act = async () => await sender.SendAsync(@event);

            act.Should().NotThrow();
        }
        [IntegrationTest]
        public void WhenSendAsyncCalledWithMultipleEventsThenShouldSendEvents()
        {
            var sender = ServiceProvider.GetRequiredService<IQueueMessageSender>();
            var events = new[] { new SameSenderEvent(), new SameSenderEvent() };

            Func<Task> act = async () => await sender.SendAsync(events);

            act.Should().NotThrow();
        }

        private class SameSenderEvent : Event
        {
            public SameSenderEvent() : base(Guid.NewGuid(), 1)
            {
            }
        }
    }
}
