using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.RabbitMQ.Connections;
using OpenEventSourcing.RabbitMQ.Extensions;
using OpenEventSourcing.RabbitMQ.Management;
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
                            .UseExchange("test-exchange");
                    });

            ServiceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public void WhenConstructedWithNullConnectionFactoryThenShouldThrowArgumentNullException()
        {
            Action act = () => new RabbitMqManagementClient(connectionFactory: null, options: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("connectionFactory");
        }
        [Fact]
        public void WhenConstructedWithNullOptionsThenShouldThrowArgumentNullException()
        {
            var factory = ServiceProvider.GetRequiredService<IRabbitMqConnectionFactory>();

            Action act = () => new RabbitMqManagementClient(connectionFactory: factory, options: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("options");
        }
    }
}
