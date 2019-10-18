using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.RabbitMQ.Exceptions;
using OpenEventSourcing.RabbitMQ.Extensions;
using OpenEventSourcing.RabbitMQ.Management;
using OpenEventSourcing.Testing.Attributes;
using RabbitMQ.Client;

namespace OpenEventSourcing.RabbitMQ.Tests.Management
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
                            .UseExchange("test-exchange");
                    });

            ServiceProvider = services.BuildServiceProvider();
        }

        [IntegrationTest]
        public void WhenCreateSubscriptionAsyncCalledWithNewSubscriptionThenShouldSucceed()
        {
            var client = ServiceProvider.GetRequiredService<IRabbitMqManagementClient>();
            var exchangeName = $"test-exchange-{Guid.NewGuid()}";
            var queueName = $"test-queue-{Guid.NewGuid()}";
            var subscriptionName = $"test-subscription-{Guid.NewGuid()}";

            Func<Task> act = async () =>
            {
                await client.CreateExchangeAsync(name: exchangeName, exchangeType: ExchangeType.Topic, durable: false);
                await client.CreateQueueAsync(name: queueName, durable: false);
                await client.CreateSubscriptionAsync(subscriptionName, queueName, exchangeName);
            };

            act.Should().NotThrow();
        }
        [IntegrationTest]
        public void WhenCreateSubscriptionAsyncCalledWithExistingSubscriptionThenShouldSucceed()
        {
            var client = ServiceProvider.GetRequiredService<IRabbitMqManagementClient>();
            var exchangeName = $"test-exchange-{Guid.NewGuid()}";
            var queueName = $"test-queue-{Guid.NewGuid()}";
            var subscriptionName = $"test-subscription-{Guid.NewGuid()}";

            Func<Task> act = async () =>
            {
                await client.CreateExchangeAsync(name: exchangeName, exchangeType: ExchangeType.Topic, durable: false);
                await client.CreateQueueAsync(name: queueName, durable: false);
                await client.CreateSubscriptionAsync(subscriptionName, queueName, exchangeName);
                await client.CreateSubscriptionAsync(subscriptionName, queueName, exchangeName);
            };

            act.Should().NotThrow();
        }
        [IntegrationTest]
        public void WhenCreateSubscriptionAsyncCalledWithNonExistentExhangeThenShouldThrowExchangeNotFoundException()
        {
            var client = ServiceProvider.GetRequiredService<IRabbitMqManagementClient>();
            var exchangeName = $"test-exchange-{Guid.NewGuid()}";
            var queueName = $"test-queue-{Guid.NewGuid()}";
            var subscriptionName = $"test-subscription-{Guid.NewGuid()}";

            Func<Task> act = async () =>
            {
                await client.CreateQueueAsync(name: queueName, durable: false);
                await client.CreateSubscriptionAsync(subscriptionName, queueName, exchangeName);
            };

            act.Should().Throw<ExchangeNotFoundException>();
        }
        [IntegrationTest]
        public void WhenCreateSubscriptionAsyncCalledWithNonExistentQueueThenShouldThrowQueueNotFoundException()
        {
            var client = ServiceProvider.GetRequiredService<IRabbitMqManagementClient>();
            var exchangeName = $"test-exchange-{Guid.NewGuid()}";
            var queueName = $"test-queue-{Guid.NewGuid()}";
            var subscriptionName = $"test-subscription-{Guid.NewGuid()}";

            Func<Task> act = async () =>
            {
                await client.CreateExchangeAsync(name: exchangeName, exchangeType: ExchangeType.Topic, durable: false);
                await client.CreateSubscriptionAsync(subscriptionName, queueName, exchangeName);
            };

            act.Should().Throw<QueueNotFoundException>();
        }
        [IntegrationTest]
        public void WhenRemoveSubscriptionAsyncCalledWithNonExistentExchangeThenShouldThrowExchangeNotFoundException()
        {
            var client = ServiceProvider.GetRequiredService<IRabbitMqManagementClient>();
            var exchangeName = $"test-exchange-{Guid.NewGuid()}";
            var queueName = $"test-queue-{Guid.NewGuid()}";
            var subscriptionName = $"test-subscription-{Guid.NewGuid()}";

            Func<Task> act = async () =>
            {
                await client.CreateQueueAsync(name: queueName, durable: false);
                await client.RemoveSubscriptionAsync(subscriptionName, queueName, exchangeName);
            };

            act.Should().Throw<ExchangeNotFoundException>();
        }
        [IntegrationTest]
        public void WhenRemoveSubscriptionAsyncCalledWithNonExistentQueueThenShouldThrowQueueNotFoundException()
        {
            var client = ServiceProvider.GetRequiredService<IRabbitMqManagementClient>();
            var exchangeName = $"test-exchange-{Guid.NewGuid()}";
            var queueName = $"test-queue-{Guid.NewGuid()}";
            var subscriptionName = $"test-subscription-{Guid.NewGuid()}";

            Func<Task> act = async () =>
            {
                await client.CreateExchangeAsync(name: exchangeName, exchangeType: ExchangeType.Topic, durable: false);
                await client.CreateSubscriptionAsync(subscriptionName, queueName, exchangeName);
            };

            act.Should().Throw<QueueNotFoundException>();
        }
    }
}
