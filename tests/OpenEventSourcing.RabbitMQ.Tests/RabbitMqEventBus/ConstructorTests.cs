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
#if NETCOREAPP3_0 || NETCOREAPP3_1
            ServiceProvider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
#else
            ServiceProvider = services.BuildServiceProvider(validateScopes: true);
#endif
        }

        [Fact]
        public void WhenConstructedWithNullLoggerThenShouldThrowArgumentNullException()
        {
            Action act = () => new RabbitMQ.RabbitMqEventBus(logger: null, serviceScopeFactory: null, queueMessageReceiver: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("logger");
        }
        [Fact]
        public void WhenConstructedWithNullServiceScopeFactoryThenShouldThrowArgumentNullException()
        {
            var logger = ServiceProvider.GetRequiredService<ILogger<RabbitMQ.RabbitMqEventBus>>();

            Action act = () => new RabbitMQ.RabbitMqEventBus(logger: logger, serviceScopeFactory: null, queueMessageReceiver: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("serviceScopeFactory");
        }
        [Fact]
        public void WhenConstructedWithNullOptionsThenShouldThrowArgumentNullException()
        {
            var logger = ServiceProvider.GetRequiredService<ILogger<RabbitMQ.RabbitMqEventBus>>();
            var scopeFactory = Mock.Of<IServiceScopeFactory>();

            Action act = () => new RabbitMQ.RabbitMqEventBus(logger: logger, serviceScopeFactory: scopeFactory, queueMessageReceiver: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("queueMessageReceiver");
        }
        [Fact]
        public void WhenResolvedAsEventBusPublisherThenShouldResolveInstance()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                Action act = () =>
                {
                    var bus = scope.ServiceProvider.GetRequiredService<IEventBusPublisher>();
                };

                act.Should().NotThrow();
            }
        }
        [Fact]
        public void WhenResolvedAsEventBusConsumerThenShouldResolveInstance()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                Action act = () =>
                {
                    var bus = scope.ServiceProvider.GetRequiredService<IEventBusConsumer>();
                };

                act.Should().NotThrow();
            }
        }
    }
}
