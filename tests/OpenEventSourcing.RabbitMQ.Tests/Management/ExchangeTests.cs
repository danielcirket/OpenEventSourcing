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
    public class ExchangeTests
    {
        public IServiceProvider ServiceProvider { get; }

        public ExchangeTests()
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
        public void WhenCreateExchangeAsyncCalledWithNonExistentExchangeThenShouldSucceed()
        {
            var client = ServiceProvider.GetRequiredService<IRabbitMqManagementClient>();
            var exchangeName = $"test-exchange-{Guid.NewGuid()}";

            Func<Task> act = async () => await client.CreateExchangeAsync(name: exchangeName, exchangeType: ExchangeType.Topic, durable: false);

            act.Should().NotThrow();
        }
        [IntegrationTest]
        public void WhenCreateExchangeAsyncCalledWithExistingExchangeThenShouldThrowExchangeAlreadyExistsException()
        {
            var client = ServiceProvider.GetRequiredService<IRabbitMqManagementClient>();
            var exchangeName = $"test-exchange-{Guid.NewGuid()}";

            Func<Task> act = async () =>
            {
                await client.CreateExchangeAsync(name: exchangeName, exchangeType: ExchangeType.Topic, durable: false);
                await client.CreateExchangeAsync(name: exchangeName, exchangeType: ExchangeType.Topic, durable: false);
            };

            act.Should().Throw<ExchangeAlreadyExistsException>()
                .And.ExchangeName.Should().Be(exchangeName);
        }
        [IntegrationTest]
        public void WhenExchangeExistsAsyncCalledWithNonExistentExchangeThenShouldReturnFalse()
        {
            var client = ServiceProvider.GetRequiredService<IRabbitMqManagementClient>();
            var exchangeName = $"test-exchange-{Guid.NewGuid()}";

            Func<Task> verify = async () =>
            {
                var result = await client.ExchangeExistsAsync(name: exchangeName);

                result.Should().BeFalse();
            };

            verify.Should().NotThrow();
        }
        [IntegrationTest]
        public void WhenExchangeExistsAsyncCalledWithExistingExchangeThenShouldReturnTrue()
        {
            var client = ServiceProvider.GetRequiredService<IRabbitMqManagementClient>();
            var exchangeName = $"test-exchange-{Guid.NewGuid()}";

            Func<Task> act = async () =>
            {
                await client.CreateExchangeAsync(name: exchangeName, exchangeType: ExchangeType.Topic, durable: false);
            };

            act.Should().NotThrow();

            Func<Task> verify = async () =>
            {
                var result = await client.ExchangeExistsAsync(name: exchangeName);

                result.Should().BeTrue();
            };

            verify.Should().NotThrow();
        }
        [IntegrationTest]
        public void WhenRemoveExchangeAsyncCalledWithNonExistentExchangeThenShouldThrowExhangeNotFoundException()
        {
            var client = ServiceProvider.GetRequiredService<IRabbitMqManagementClient>();
            var exchangeName = $"test-exchange-{Guid.NewGuid()}";

            Func<Task> act = async () => await client.RemoveExchangeAsync(name: exchangeName);

            act.Should().Throw<ExchangeNotFoundException>()
                .And.ExchangeName.Should().Be(exchangeName);
        }
        [IntegrationTest]
        public void WhenRemoveExchangeAsyncCalledWithExistingExchangeThenShouldSucceed()
        {
            var client = ServiceProvider.GetRequiredService<IRabbitMqManagementClient>();
            var exchangeName = $"test-exchange-{Guid.NewGuid()}";

            Func<Task> act = async () =>
            {
                await client.CreateExchangeAsync(name: exchangeName, exchangeType: ExchangeType.Topic, durable: false);
                await client.RemoveExchangeAsync(name: exchangeName);
            };

            act.Should().NotThrow();
        }
    }
}
