using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.RabbitMQ.Connections;
using OpenEventSourcing.RabbitMQ.Extensions;
using Xunit;

namespace OpenEventSourcing.RabbitMQ.Tests.Connections.Connection
{
    public class FinalizerTests
    {
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
                        o.UseConnection("amqp://guest:guest@localhost:5672/")
                         .UseExchange("test-exchange");
                    });

            var sp = services.BuildServiceProvider();

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
                        o.UseConnection("amqp://guest:guest@localhost:5672/")
                         .UseExchange("test-exchange");
                    });

            var sp = services.BuildServiceProvider();

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
                        o.UseConnection("amqp://guest:guest@localhost:5672/")
                         .UseExchange("test-exchange");
                    });

            var sp = services.BuildServiceProvider();

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

                await connection.ExchangeExistsAsync(name: options.Value.Exchange);

                connection.Dispose();
            };

            verify.Should().NotThrow();
        }
    }
}
