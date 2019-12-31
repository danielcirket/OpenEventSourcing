using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OpenEventSourcing.Azure.ServiceBus.Topics;
using Xunit;

namespace OpenEventSourcing.Azure.ServiceBus.Tests.Topics.Sender
{
    public class ConstructorTests
    {
        [Fact]
        public void WhenConstructedWithNullLoggerThenShouldThrowArgumentNullException()
        {
            Action act = () => new DefaultTopicMessageSender(logger: null, topicClientFactory: null, messageFactory: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("logger");
        }
        [Fact]
        public void WhenConstructedWithNullTopicClientFactoryThenShouldThrowArgumentNullException()
        {
            var logger = Mock.Of<ILogger<DefaultTopicMessageSender>>();

            Action act = () => new DefaultTopicMessageSender(logger: logger, topicClientFactory: null, messageFactory: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("topicClientFactory");
        }
        [Fact]
        public void WhenConstructedWithNullMessageFactoryThenShouldThrowArgumentNullException()
        {
            var logger = Mock.Of<ILogger<DefaultTopicMessageSender>>();
            var factory = Mock.Of<ITopicClientFactory>();

            Action act = () => new DefaultTopicMessageSender(logger: logger, topicClientFactory: factory, messageFactory: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("messageFactory");
        }
    }
}
