using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using OpenEventSourcing.Events;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.RabbitMQ.Extensions;
using OpenEventSourcing.RabbitMQ.Queues;
using OpenEventSourcing.Serialization.Json.Extensions;
using Xunit;

namespace OpenEventSourcing.RabbitMQ.Tests.RabbitMqEventBus
{
    public partial class ConstructorTests
    {
        public IServiceProvider ServiceProvider { get; }

        public ConstructorTests()
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

            ServiceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public void WhenConstructedWithNullLoggerThenShouldThrowArgumentNullException()
        {
            Action act = () => new RabbitMQ.RabbitMqEventBus(logger: null, messageSender: null, queueMessageReceiver: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("logger");
        }
        [Fact]
        public void WhenConstructedWithNullMessageSenderThenShouldThrowArgumentNullException()
        {
            var logger = ServiceProvider.GetRequiredService<ILogger<RabbitMQ.RabbitMqEventBus>>();

            Action act = () => new RabbitMQ.RabbitMqEventBus(logger: logger, messageSender: null, queueMessageReceiver: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("messageSender");
        }
        [Fact]
        public void WhenConstructedWithNullOptionsThenShouldThrowArgumentNullException()
        {
            var logger = ServiceProvider.GetRequiredService<ILogger<RabbitMQ.RabbitMqEventBus>>();
            var messageSender = Mock.Of<IQueueMessageSender
                >();

            Action act = () => new RabbitMQ.RabbitMqEventBus(logger: logger, messageSender: messageSender, queueMessageReceiver: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("queueMessageReceiver");
        }
        [Fact]
        public void WhenResolvedAsEventBusPublisherThenShouldResolveInstance()
        {
            Action act = () =>
            {
                var bus = ServiceProvider.GetRequiredService<IEventBusPublisher>();
            };

            act.Should().NotThrow();
        }
        [Fact]
        public void WhenResolvedAsEventBusConsumerThenShouldResolveInstance()
        {
            Action act = () =>
            {
                var bus = ServiceProvider.GetRequiredService<IEventBusConsumer>();
            };

            act.Should().NotThrow();
        }
    }
}
