using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.RabbitMQ.Connections;
using OpenEventSourcing.RabbitMQ.Extensions;
using OpenEventSourcing.Serialization.Json.Extensions;
using Xunit;

namespace OpenEventSourcing.RabbitMQ.Tests.Connections.Connection
{
    public class FinalizerTests : IClassFixture<ConfigurationFixture>
    {
        private readonly IConfiguration _configuration;

        public FinalizerTests(ConfigurationFixture fixture)
        {
            if (fixture == null)
                throw new ArgumentNullException(nameof(fixture));

            _configuration = fixture.Configuration;
        }

        [Fact]
        public void WhenConnectionFinalizedThenOpenConnectionShouldBeReturnedToPool()
        {
            void CreateConnection(RabbitMqConnectionPool p)
            {
                p.GetConnection();
            }

            var services = new ServiceCollection();

            services.AddLogging(o => o.AddDebug())
                    .AddOpenEventSourcing()
                    .AddRabbitMq(o =>
                    {
                        o.UseConnection(_configuration.GetValue<string>("RabbitMQ:ConnectionString"))
                         .UseExchange(e =>
                         {
                             e.WithName("test-exchange");
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
            
            CreateConnection(pool);

            pool.AvailableConnections.Should().Be(0);

            GC.Collect(0, GCCollectionMode.Forced, blocking: true);
            GC.WaitForPendingFinalizers();

            pool.AvailableConnections.Should().Be(1);
        }
        [Fact]
        public void WhenConnectionFinalizedThenClosedConnectionShouldNotBeReturnedToPool()
        {
            void CreateConnection(RabbitMqConnectionPool p)
            {
                var connection = p.GetConnection();
                connection.UnderlyingConnection.Close();
            }

            var services = new ServiceCollection();

            services.AddLogging(o => o.AddDebug())
                    .AddOpenEventSourcing()
                    .AddRabbitMq(o =>
                    {
                        o.UseConnection(_configuration.GetValue<string>("RabbitMQ:ConnectionString"))
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

            CreateConnection(pool);

            pool.AvailableConnections.Should().Be(0);

            GC.Collect(0, GCCollectionMode.Forced, blocking: true);
            GC.WaitForPendingFinalizers();

            pool.AvailableConnections.Should().Be(0);
        }
        [Fact]
        public void WhenConnectionFinalizedThenOpenConnectionShouldBeReturnedToPoolAndBeInUseableState()
        {
            string connectionId = null;

            void CreateConnection(RabbitMqConnectionPool p)
            {
                var connection = p.GetConnection();
                connectionId = connection.ConnectionId;
            }

            var services = new ServiceCollection();

            services.AddLogging(o => o.AddDebug())
                    .AddOpenEventSourcing()
                    .AddRabbitMq(o =>
                    {
                        o.UseConnection(_configuration.GetValue<string>("RabbitMQ:ConnectionString"))
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
            var options = sp.GetRequiredService<IOptions<RabbitMqOptions>>();

            CreateConnection(pool);

            pool.AvailableConnections.Should().Be(0);

            GC.Collect(0, GCCollectionMode.Forced, blocking: true);
            GC.WaitForPendingFinalizers();

            pool.AvailableConnections.Should().Be(1);

            Func<Task> verify = async () =>
            {
                var connection = pool.GetConnection();

                connection.ConnectionId.Should().Be(connectionId);

                await connection.ExchangeExistsAsync(name: options.Value.Exchange.Name);

                connection.Dispose();
            };

            verify.Should().NotThrow();
        }
    }
}
