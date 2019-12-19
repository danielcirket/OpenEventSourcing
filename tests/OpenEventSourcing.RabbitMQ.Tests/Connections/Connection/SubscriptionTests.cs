using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.RabbitMQ.Connections;
using OpenEventSourcing.RabbitMQ.Exceptions;
using OpenEventSourcing.RabbitMQ.Extensions;
using OpenEventSourcing.Serialization.Json.Extensions;
using OpenEventSourcing.Testing.Attributes;
using RabbitMQ.Client;

namespace OpenEventSourcing.RabbitMQ.Tests.Connections.Connection
{
    public class SubscriptionTests
    {
        public IServiceProvider ServiceProvider { get; }

        public SubscriptionTests()
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
            ServiceProvider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
#else
            ServiceProvider = services.BuildServiceProvider(validateScopes: true);
#endif
        }

        [IntegrationTest]
        public void WhenCreateSubscriptionAsyncCalledWithNewSubscriptionThenShouldSucceed()
        {
            var factory = ServiceProvider.GetRequiredService<IRabbitMqConnectionFactory>();
            var exchangeName = $"test-exchange-{Guid.NewGuid()}";
            var queueName = $"test-queue-{Guid.NewGuid()}";
            var subscriptionName = $"test-subscription-{Guid.NewGuid()}";

            Func<Task> act = async () =>
            {
                var connection = await factory.CreateConnectionAsync(CancellationToken.None);
                await connection.CreateExchangeAsync(name: exchangeName, exchangeType: ExchangeType.Topic, durable: false);
                await connection.CreateQueueAsync(name: queueName, durable: false);
                await connection.CreateSubscriptionAsync(subscriptionName, queueName, exchangeName);
            };

            act.Should().NotThrow();
        }
        [IntegrationTest]
        public void WhenCreateSubscriptionAsyncCalledWithExistingSubscriptionThenShouldSucceed()
        {
            var factory = ServiceProvider.GetRequiredService<IRabbitMqConnectionFactory>();
            var exchangeName = $"test-exchange-{Guid.NewGuid()}";
            var queueName = $"test-queue-{Guid.NewGuid()}";
            var subscriptionName = $"test-subscription-{Guid.NewGuid()}";

            Func<Task> act = async () =>
            {
                var connection = await factory.CreateConnectionAsync(CancellationToken.None);
                await connection.CreateExchangeAsync(name: exchangeName, exchangeType: ExchangeType.Topic, durable: false);
                await connection.CreateQueueAsync(name: queueName, durable: false);
                await connection.CreateSubscriptionAsync(subscriptionName, queueName, exchangeName);
                await connection.CreateSubscriptionAsync(subscriptionName, queueName, exchangeName);
            };

            act.Should().NotThrow();
        }
        [IntegrationTest]
        public void WhenCreateSubscriptionAsyncCalledWithNonExistentExhangeThenShouldThrowExchangeNotFoundException()
        {
            var factory = ServiceProvider.GetRequiredService<IRabbitMqConnectionFactory>();
            var exchangeName = $"test-exchange-{Guid.NewGuid()}";
            var queueName = $"test-queue-{Guid.NewGuid()}";
            var subscriptionName = $"test-subscription-{Guid.NewGuid()}";

            Func<Task> act = async () =>
            {
                var connection = await factory.CreateConnectionAsync(CancellationToken.None);
                await connection.CreateQueueAsync(name: queueName, durable: false);
                await connection.CreateSubscriptionAsync(subscriptionName, queueName, exchangeName);
            };

            act.Should().Throw<ExchangeNotFoundException>();
        }
        [IntegrationTest]
        public void WhenCreateSubscriptionAsyncCalledWithNonExistentQueueThenShouldThrowQueueNotFoundException()
        {
            var factory = ServiceProvider.GetRequiredService<IRabbitMqConnectionFactory>();
            var exchangeName = $"test-exchange-{Guid.NewGuid()}";
            var queueName = $"test-queue-{Guid.NewGuid()}";
            var subscriptionName = $"test-subscription-{Guid.NewGuid()}";

            Func<Task> act = async () =>
            {
                var connection = await factory.CreateConnectionAsync(CancellationToken.None);
                await connection.CreateExchangeAsync(name: exchangeName, exchangeType: ExchangeType.Topic, durable: false);
                await connection.CreateSubscriptionAsync(subscriptionName, queueName, exchangeName);
            };

            act.Should().Throw<QueueNotFoundException>();
        }
        [IntegrationTest]
        public void WhenRemoveSubscriptionAsyncCalledWithNonExistentExchangeThenShouldThrowExchangeNotFoundException()
        {
            var factory = ServiceProvider.GetRequiredService<IRabbitMqConnectionFactory>();
            var exchangeName = $"test-exchange-{Guid.NewGuid()}";
            var queueName = $"test-queue-{Guid.NewGuid()}";
            var subscriptionName = $"test-subscription-{Guid.NewGuid()}";

            Func<Task> act = async () =>
            {
                var connection = await factory.CreateConnectionAsync(CancellationToken.None);
                await connection.CreateQueueAsync(name: queueName, durable: false);
                await connection.RemoveSubscriptionAsync(subscriptionName, queueName, exchangeName);
            };

            act.Should().Throw<ExchangeNotFoundException>();
        }
        [IntegrationTest]
        public void WhenRemoveSubscriptionAsyncCalledWithNonExistentQueueThenShouldThrowQueueNotFoundException()
        {
            var factory = ServiceProvider.GetRequiredService<IRabbitMqConnectionFactory>();
            var exchangeName = $"test-exchange-{Guid.NewGuid()}";
            var queueName = $"test-queue-{Guid.NewGuid()}";
            var subscriptionName = $"test-subscription-{Guid.NewGuid()}";

            Func<Task> act = async () =>
            {
                var connection = await factory.CreateConnectionAsync(CancellationToken.None);
                await connection.CreateExchangeAsync(name: exchangeName, exchangeType: ExchangeType.Topic, durable: false);
                await connection.CreateSubscriptionAsync(subscriptionName, queueName, exchangeName);
            };

            act.Should().Throw<QueueNotFoundException>();
        }
    }
}
