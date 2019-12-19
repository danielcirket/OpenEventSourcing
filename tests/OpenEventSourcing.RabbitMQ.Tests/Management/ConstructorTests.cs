using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.RabbitMQ.Connections;
using OpenEventSourcing.RabbitMQ.Extensions;
using OpenEventSourcing.RabbitMQ.Management;
using OpenEventSourcing.RabbitMQ.Management.Api;
using OpenEventSourcing.Serialization.Json.Extensions;
using Xunit;

namespace OpenEventSourcing.RabbitMQ.Tests.Management
{
    public partial class ConstructorTests
    {
        public IServiceProvider ServiceProvider { get; }

        public ConstructorTests()
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

        [Fact]
        public void WhenConstructedWithNullConnectionFactoryThenShouldThrowArgumentNullException()
        {
            Action act = () => new RabbitMqManagementClient(connectionFactory: null, client: null, options: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("connectionFactory");
        }
        [Fact]
        public void WhenConstructedWithNullApiClientThenShouldThrowArgumentNullException()
        {
            var factory = ServiceProvider.GetRequiredService<IRabbitMqConnectionFactory>();

            Action act = () => new RabbitMqManagementClient(connectionFactory: factory, client: null, options: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("client");
        }
        [Fact]
        public void WhenConstructedWithNullOptionsThenShouldThrowArgumentNullException()
        {
            var factory = ServiceProvider.GetRequiredService<IRabbitMqConnectionFactory>();
            var client = Mock.Of<IRabbitMqManagementApiClient>();

            Action act = () => new RabbitMqManagementClient(connectionFactory: factory, client: client, options: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("options");
        }
    }
}
