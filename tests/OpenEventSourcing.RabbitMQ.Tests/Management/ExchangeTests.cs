using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.RabbitMQ.Exceptions;
using OpenEventSourcing.RabbitMQ.Extensions;
using OpenEventSourcing.RabbitMQ.Management;
using OpenEventSourcing.Serialization.Json.Extensions;
using OpenEventSourcing.Testing.Attributes;
using RabbitMQ.Client;
using Xunit;

namespace OpenEventSourcing.RabbitMQ.Tests.Management
{
    public class ExchangeTests : IClassFixture<ConfigurationFixture>
    {
        public IServiceProvider ServiceProvider { get; }

        public ExchangeTests(ConfigurationFixture fixture)
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

#if NETCOREAPP3_0 || NETCOREAPP3_1
            ServiceProvider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true });
#else
            ServiceProvider = services.BuildServiceProvider(validateScopes: true);
#endif
        }

        [IntegrationTest]
        public void WhenCreateExchangeAsyncCalledWithNonExistentExchangeThenShouldSucceed()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IRabbitMqManagementClient>();
                var exchangeName = $"test-exchange-{Guid.NewGuid()}";

                Func<Task> act = async () => await client.CreateExchangeAsync(name: exchangeName, exchangeType: ExchangeType.Topic, durable: false);

                act.Should().NotThrow();
            }
        }
        [IntegrationTest]
        public void WhenCreateExchangeAsyncCalledWithExistingExchangeThenShouldThrowExchangeAlreadyExistsException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IRabbitMqManagementClient>();
                var exchangeName = $"test-exchange-{Guid.NewGuid()}";

                Func<Task> act = async () =>
                {
                    await client.CreateExchangeAsync(name: exchangeName, exchangeType: ExchangeType.Topic, durable: false);
                    await client.CreateExchangeAsync(name: exchangeName, exchangeType: ExchangeType.Topic, durable: false);
                };

                act.Should().Throw<ExchangeAlreadyExistsException>()
                    .And.ExchangeName.Should().Be(exchangeName);
            }
        }
        [IntegrationTest]
        public void WhenExchangeExistsAsyncCalledWithNonExistentExchangeThenShouldReturnFalse()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IRabbitMqManagementClient>();
                var exchangeName = $"test-exchange-{Guid.NewGuid()}";

                Func<Task> verify = async () =>
                {
                    var result = await client.ExchangeExistsAsync(name: exchangeName);

                    result.Should().BeFalse();
                };

                verify.Should().NotThrow();
            }
        }
        [IntegrationTest]
        public void WhenExchangeExistsAsyncCalledWithExistingExchangeThenShouldReturnTrue()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IRabbitMqManagementClient>();
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
        }
        [IntegrationTest]
        public void WhenRemoveExchangeAsyncCalledWithNonExistentExchangeThenShouldThrowExhangeNotFoundException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IRabbitMqManagementClient>();
                var exchangeName = $"test-exchange-{Guid.NewGuid()}";

                Func<Task> act = async () => await client.RemoveExchangeAsync(name: exchangeName);

                act.Should().Throw<ExchangeNotFoundException>()
                    .And.ExchangeName.Should().Be(exchangeName);
            }
        }
        [IntegrationTest]
        public void WhenRemoveExchangeAsyncCalledWithExistingExchangeThenShouldSucceed()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IRabbitMqManagementClient>();
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
}
