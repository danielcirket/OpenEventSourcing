using System;
using System.Linq;
using System.Net.Http;
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
                         .UseExchange(e =>
                         {
                             e.WithName("test-exchange");
                             e.UseExchangeType("topic");
                         });
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
        [IntegrationTest]
        public void WhenRetrieveSubscriptionsCalledWhenManagementApiNotConfiguredThenShouldThrowInvalidOperationException()
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
                await client.RetrieveSubscriptionsAsync(queueName);
            };

            act.Should().Throw<InvalidOperationException>();
        }
        [IntegrationTest]
        public void WhenRetrieveSubscriptionsCalledWhenManagementApiCorrectlyConfiguredThenShouldReturnExpectedSubscriptions()
        {
            var services = new ServiceCollection();
            var exchangeName = $"test-exchange-{Guid.NewGuid()}";
            var exchangeType = ExchangeType.Topic;
            var queueName = $"test-queue-{Guid.NewGuid()}";
            var subscriptionName = $"test-subscription-{Guid.NewGuid()}";

            services.AddLogging(o => o.AddDebug())
                    .AddOpenEventSourcing()
                    .AddRabbitMq(o =>
                    {
                        o.UseConnection("amqp://guest:guest@localhost:5672/")
                         .UseExchange(e =>
                         {
                             e.WithName(exchangeName);
                             e.UseExchangeType(exchangeType);
                         });

                        o.UseManagementApi(m => 
                        {
                            m.WithEndpoint("http://localhost:15672/")
                             .WithCredentials("guest", "guest");
                        });
                    });

            var sp = services.BuildServiceProvider();
            var client = sp.GetRequiredService<IRabbitMqManagementClient>();

            Func<Task> act = async () =>
            {
                await client.CreateExchangeAsync(name: exchangeName, exchangeType: exchangeType, durable: false);
                await client.CreateQueueAsync(name: queueName, durable: false);
                await client.CreateSubscriptionAsync(subscriptionName, queueName, exchangeName);

                var results = await client.RetrieveSubscriptionsAsync(queueName);

                results.Should().HaveCount(1);
                results.Single().Exchange.Should().Be(exchangeName);
                results.Single().Queue.Should().Be(queueName);
                results.Single().RoutingKey.Should().Be(subscriptionName);
            };

            act.Should().NotThrow();
        }
        [IntegrationTest]
        public void WhenRetrieveSubscriptionsCalledWhenManagementApiNotAvailableThenShouldThrow()
        {
            var services = new ServiceCollection();
            var exchangeName = $"test-exchange-{Guid.NewGuid()}";
            var queueName = $"test-queue-{Guid.NewGuid()}";
            var subscriptionName = $"test-subscription-{Guid.NewGuid()}";

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

                        o.UseManagementApi(m => {
                            m.WithEndpoint("http://localhost:12345/")
                             .WithCredentials("guest", "guest");
                        });
                    });

            var sp = services.BuildServiceProvider();
            var client = sp.GetRequiredService<IRabbitMqManagementClient>();

            Func<Task> act = async () =>
            {
                await client.CreateExchangeAsync(name: exchangeName, exchangeType: ExchangeType.Topic, durable: false);
                await client.CreateQueueAsync(name: queueName, durable: false);
                await client.CreateSubscriptionAsync(subscriptionName, queueName, exchangeName);

                var results = await client.RetrieveSubscriptionsAsync(queueName);
            };

            act.Should().Throw<Exception>();
        }
        [IntegrationTest]
        public void WhenRetrieveSubscriptionsCalledWhenManagementApiConfiguredWithIncorrectCredentialsThenShouldReturnExpectedSubscriptions()
        {
            var services = new ServiceCollection();
            var exchangeName = $"test-exchange-{Guid.NewGuid()}";
            var exchangeType = ExchangeType.Topic;
            var queueName = $"test-queue-{Guid.NewGuid()}";
            var subscriptionName = $"test-subscription-{Guid.NewGuid()}";

            services.AddLogging(o => o.AddDebug())
                    .AddOpenEventSourcing()
                    .AddRabbitMq(o =>
                    {
                        o.UseConnection("amqp://guest:guest@localhost:5672/")
                         .UseExchange(e =>
                         {
                             e.WithName(exchangeName);
                             e.UseExchangeType(exchangeType);
                         });

                        o.UseManagementApi(m => {
                            m.WithEndpoint("http://localhost:15672/");
                        });
                    });

            var sp = services.BuildServiceProvider();
            var client = sp.GetRequiredService<IRabbitMqManagementClient>();

            Func<Task> act = async () =>
            {
                await client.CreateExchangeAsync(name: exchangeName, exchangeType: exchangeType, durable: false);
                await client.CreateQueueAsync(name: queueName, durable: false);
                await client.CreateSubscriptionAsync(subscriptionName, queueName, exchangeName);

                var results = await client.RetrieveSubscriptionsAsync(queueName);
            };

            act.Should().Throw<HttpRequestException>()
                .And.Message.Should().Contain("401");
        }
    }
}
