using System;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using OpenEventSourcing.RabbitMQ.Connections;
using OpenEventSourcing.RabbitMQ.Subscriptions;
using Xunit;

namespace OpenEventSourcing.RabbitMQ.Tests.Subscriptions.SubscriptionManager
{
    public class ConstructorTests
    {
        [Fact]
        public void WhenConstructedWithNullConnectionFactoryThenShouldThrowArgumentNullException()
        {
            var options = Mock.Of<IOptions<RabbitMqOptions>>();

            Action act = () => new DefaultSubscriptionManager(options: options, connectionFactory: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("connectionFactory");
        }
        [Fact]
        public void WhenConstructedWithNullOptionsThenShouldThrowArgumentNullException()
        {
            var connectionFactory = Mock.Of<IRabbitMqConnectionFactory>();

            Action act = () => new DefaultSubscriptionManager(connectionFactory: connectionFactory, options: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("options");
        }
    }
}
