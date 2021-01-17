using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.RabbitMQ.Connections;
using OpenEventSourcing.RabbitMQ.Exceptions;
using OpenEventSourcing.RabbitMQ.Extensions;
using OpenEventSourcing.Serialization.Json.Extensions;
using RabbitMQ.Client;
using Xunit;

namespace OpenEventSourcing.RabbitMQ.Tests.Connections.Connection
{
    public class SubscriptionTests : IClassFixture<ConfigurationFixture>
    {
        public IServiceProvider ServiceProvider { get; }

        public SubscriptionTests(ConfigurationFixture fixture)
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
            
            ServiceProvider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
        }

        [RabbitMqTest]
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
        [RabbitMqTest]
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
        [RabbitMqTest]
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
        [RabbitMqTest]
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
        [RabbitMqTest]
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
        [RabbitMqTest]
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
