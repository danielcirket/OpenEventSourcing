using System;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using OpenEventSourcing.RabbitMQ.Connections;
using OpenEventSourcing.RabbitMQ.Management;
using OpenEventSourcing.RabbitMQ.Subscriptions;
using Xunit;

namespace OpenEventSourcing.RabbitMQ.Tests.Subscriptions.SubscriptionManager
{
    public class ConstructorTests
    {
        [Fact]
        public void WhenConstructedWithNullApiManagementClientThenShouldThrowArgumentNullException()
        {
            var options = Mock.Of<IOptions<RabbitMqOptions>>();

            Action act = () => new DefaultSubscriptionManager(options: options, client: null, connectionFactory: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("client");
        }
        [Fact]
        public void WhenConstructedWithNullConnectionFactoryThenShouldThrowArgumentNullException()
        {
            var options = Mock.Of<IOptions<RabbitMqOptions>>();
            var client = Mock.Of<IRabbitMqManagementClient>();

            Action act = () => new DefaultSubscriptionManager(options: options, client: client, connectionFactory: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("connectionFactory");
        }
        [Fact]
        public void WhenConstructedWithNullOptionsThenShouldThrowArgumentNullException()
        {
            var connectionFactory = Mock.Of<IRabbitMqConnectionFactory>();
            var client = Mock.Of<IRabbitMqManagementClient>();

            Action act = () => new DefaultSubscriptionManager(connectionFactory: connectionFactory, client: client, options: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("options");
        }
    }
}
