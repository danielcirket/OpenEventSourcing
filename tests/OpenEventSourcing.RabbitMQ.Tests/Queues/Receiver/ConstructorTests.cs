using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using OpenEventSourcing.RabbitMQ.Connections;
using OpenEventSourcing.RabbitMQ.Queues;
using OpenEventSourcing.RabbitMQ.Subscriptions;
using Xunit;

namespace OpenEventSourcing.RabbitMQ.Tests.Queues.Receiver
{
    public class ConstructorTests
    {
        [Fact]
        public void WhenConstructedWithNullOptionsThenShouldThrowArgumentNullException()
        {
            Action act = () => new DefaultQueueMessageReceiver(options: null, logger: null, connectionFactory: null, serviceScopeFactory: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("options");
        }
        [Fact]
        public void WhenConstructedWithNullLoggerThenShouldThrowArgumentNullException()
        {
            var options = Mock.Of<IOptions<RabbitMqOptions>>();
            var connectionFactory = Mock.Of<IRabbitMqConnectionFactory>();

            Action act = () => new DefaultQueueMessageReceiver(logger: null, options: options, connectionFactory: connectionFactory, serviceScopeFactory: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("logger");
        }
        [Fact]
        public void WhenConstructedWithNullConnectionFactoryThenShouldThrowArgumentNullException()
        {
            var logger = Mock.Of<ILogger<DefaultQueueMessageReceiver>>();
            var options = Mock.Of<IOptions<RabbitMqOptions>>();

            Action act = () => new DefaultQueueMessageReceiver(logger: logger, options: options, connectionFactory: null, serviceScopeFactory: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("connectionFactory");
        }
        [Fact]
        public void WhenConstructedWithNullServiceScopeFactoryThenShouldThrowArgumentNullException()
        {
            var logger = Mock.Of<ILogger<DefaultQueueMessageReceiver>>();
            var options = Mock.Of<IOptions<RabbitMqOptions>>();
            var connectionFactory = Mock.Of<IRabbitMqConnectionFactory>();

            Action act = () => new DefaultQueueMessageReceiver(logger: logger, options: options, connectionFactory: connectionFactory, serviceScopeFactory: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("serviceScopeFactory");
        }
    }
}
