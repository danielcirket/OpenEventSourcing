using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.RabbitMQ.Connections;
using OpenEventSourcing.RabbitMQ.Extensions;
using OpenEventSourcing.Testing.Attributes;
using RabbitMQ.Client.Exceptions;
using Xunit;

namespace OpenEventSourcing.RabbitMQ.Tests.Connections
{
    public class ConstructorTests
    {
        public IServiceProvider ServiceProvider { get; }

        public ConstructorTests()
        {
            var services = new ServiceCollection();
            // services.AddOpenEventSourcing()
            services.AddLogging(o => o.AddDebug());

            services.AddOpenEventSourcing()
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

        [Fact]
        public void WhenConstructedWithNullOptionsThenShouldThrowArgumentNullException()
        {
            Action act = () => new RabbitMqConnectionFactory(options: null, pool: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("options");
        }
        [Fact]
        public void WhenConstructedWithNullConnectionPoolThenShouldThrowArgumentNullException()
        {
            var options = Mock.Of<IOptions<RabbitMqOptions>>();

            Action act = () => new RabbitMqConnectionFactory(options: options, pool: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("pool");
        }
        [IntegrationTest]
        public async Task WhenCreateConnectionAsyncCalledThenShouldReturnConnection()
        {
            var factory = ServiceProvider.GetRequiredService<IRabbitMqConnectionFactory>();

            var connection = await factory.CreateConnectionAsync(cancellationToken: CancellationToken.None);

            connection.Should().NotBeNull();

            connection.Dispose();
        }
        [IntegrationTest]
        public void WhenRabbitMqNotReachableCreateConnectionAsyncShouldThrowBrokerUnreachableException()
        {
            var services = new ServiceCollection();
            // services.AddOpenEventSourcing()
            services.AddLogging(o => o.AddDebug());

            services.AddOpenEventSourcing()
                    .AddRabbitMq(o =>
                    {
                        o.UseConnection("amqp://guest:guest@localhost:1324/")
                         .UseExchange(e =>
                         {
                             e.WithName($"test-exchange-{Guid.NewGuid()}");
                             e.UseExchangeType("topic");
                         });
                    });

            var sp = services.BuildServiceProvider();

            var factory = sp.GetRequiredService<IRabbitMqConnectionFactory>();

            Func<Task<IRabbitMqConnection>> act = async () => await factory.CreateConnectionAsync(cancellationToken: CancellationToken.None);

            act.Should().Throw<BrokerUnreachableException>();
        }
    }
}
