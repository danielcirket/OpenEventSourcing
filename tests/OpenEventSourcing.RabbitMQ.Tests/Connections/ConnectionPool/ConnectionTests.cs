using System;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.RabbitMQ.Connections;
using OpenEventSourcing.RabbitMQ.Extensions;
using OpenEventSourcing.Serialization.Json.Extensions;
using RabbitMQ.Client.Exceptions;
using Xunit;

namespace OpenEventSourcing.RabbitMQ.Tests.Connections.ConnectionPool
{
    public class ConnectionTests : IClassFixture<ConfigurationFixture>
    {
        public IServiceProvider ServiceProvider { get; }

        public ConnectionTests(ConfigurationFixture fixture)
        {
            var services = new ServiceCollection();
            // services.AddOpenEventSourcing()
            services.AddLogging(o => o.AddDebug());

            services.AddOpenEventSourcing()
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
            ServiceProvider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
#else
            ServiceProvider = services.BuildServiceProvider(validateScopes: true);
#endif
        }

        [RabbitMqTest]
        public void WhenGetConnectionCalledThenShouldReturnConnection()
        {
            var pool = ServiceProvider.GetRequiredService<RabbitMqConnectionPool>();

            var connection = pool.GetConnection();

            connection.Should().NotBeNull();

            connection.Dispose();
        }
        [RabbitMqTest]
        public void WhenGetConnectionCalledAndConnectionDisposedThenPoolShouldHaveSingleAvailableConnection()
        {
            var pool = ServiceProvider.GetRequiredService<RabbitMqConnectionPool>();

            var connection = pool.GetConnection();

            connection.Dispose();

            pool.AvailableConnections.Should().Be(1);
        }
        [RabbitMqTest]
        public void WhenGetConnectionCalledMultipleTimesWithoutConnectionsReturnedThenShouldReturnMultipleConnections()
        {
            var numberOfConnections = 5;

            var pool = ServiceProvider.GetRequiredService<RabbitMqConnectionPool>();
            var connections = Enumerable.Range(0, numberOfConnections).Select(i => pool.GetConnection()).ToList();

            pool.AvailableConnections.Should().Be(0);

            foreach (var connection in connections)
                connection.Dispose();

            pool.AvailableConnections.Should().Be(numberOfConnections);
        }
        [RabbitMqTest]
        public void WhenRabbitMqNotReachableGetConnectionShouldThrowBrokerUnreachableException()
        {
            var services = new ServiceCollection();

            services.AddLogging(o => o.AddDebug());

            services.AddOpenEventSourcing()
                    .AddRabbitMq(o =>
                    {
                        o.UseConnection("amqp://guest:guest@localhost:1324/")
                         .UseExchange(e =>
                         {
                             e.WithName($"test-exchange-{Guid.NewGuid()}");
                             e.UseExchangeType("topic");
                             e.AutoDelete();
                         });
                    })
                    .AddJsonSerializers();

#if NETCOREAPP3_0 || NETCOREAPP3_1
            var sp = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
#else
            var sp = services.BuildServiceProvider(validateScopes: true);
#endif

            var pool = sp.GetRequiredService<RabbitMqConnectionPool>();

            Action act = () => pool.GetConnection();

            act.Should().Throw<BrokerUnreachableException>();
        }
    }
}
