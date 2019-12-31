using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OpenEventSourcing.Azure.ServiceBus.Topics;
using Xunit;

namespace OpenEventSourcing.Azure.ServiceBus.Tests.Topics.Receiver
{
    public class ConstructorTests
    {
        [Fact]
        public void WhenConstructedWithNullLoggerThenShouldThrowArgumentNullException()
        {
            Action act = () => new DefaultTopicMessageReceiver(logger: null, serviceScopeFactory: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("logger");
        }
        [Fact]
        public void WhenConstructedWithNullTopicClientFactoryThenShouldThrowArgumentNullException()
        {
            var logger = Mock.Of<ILogger<DefaultTopicMessageReceiver>>();

            Action act = () => new DefaultTopicMessageReceiver(logger: logger, serviceScopeFactory: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("serviceScopeFactory");
        }
    }
}
