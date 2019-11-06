using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using OpenEventSourcing.RabbitMQ.Messages;
using OpenEventSourcing.RabbitMQ.Queues;
using Xunit;

namespace OpenEventSourcing.RabbitMQ.Tests.Queues.Sender
{
    public class ConstructorTests
    {
        [Fact]
        public void WhenConstructedWithNullLoggerThenShouldThrowArgumentNullException()
        {
            Action act = () => new DefaultQueueMessageSender(logger: null, options: null, messageFactory: null, connectionFactory: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("logger");
        }
        [Fact]
        public void WhenConstructedWithNullOptionsThenShouldThrowArgumentNullException()
        {
            var logger = Mock.Of<ILogger<DefaultQueueMessageSender>>();

            Action act = () => new DefaultQueueMessageSender(logger: logger, options: null, messageFactory: null, connectionFactory: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("options");
        }
        [Fact]
        public void WhenConstructedWithNullMessageFactoryThenShouldThrowArgumentNullException()
        {
            var logger = Mock.Of<ILogger<DefaultQueueMessageSender>>();
            var options = Mock.Of<IOptions<RabbitMqOptions>>();

            Action act = () => new DefaultQueueMessageSender(logger: logger, options: options, messageFactory: null, connectionFactory: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("messageFactory");
        }
        [Fact]
        public void WhenConstructedWithNullConnectionFactoryThenShouldThrowArgumentNullException()
        {
            var logger = Mock.Of<ILogger<DefaultQueueMessageSender>>();
            var options = Mock.Of<IOptions<RabbitMqOptions>>();
            var messageFactory = Mock.Of<IMessageFactory>();

            Action act = () => new DefaultQueueMessageSender(logger: logger, options: options, messageFactory: messageFactory, connectionFactory: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("connectionFactory");
        }
    }
}
