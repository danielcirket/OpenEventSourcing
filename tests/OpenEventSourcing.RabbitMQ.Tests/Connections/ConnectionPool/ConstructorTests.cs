using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OpenEventSourcing.RabbitMQ.Connections;
using Xunit;

namespace OpenEventSourcing.RabbitMQ.Tests.Connections.ConnectionPool
{
    public class ConstructorTests
    {
        [Fact]
        public void WhenConstructedWithNullLoggerThenShouldThrowArgumentNullException()
        {
            Action act = () => new RabbitMqConnectionPool(logger: null, options: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("logger");
        }
        [Fact]
        public void WhenConstructedWithNullOptionsThenShouldThrowArgumentNullException()
        {
            var logger = Mock.Of<ILogger<RabbitMqConnectionPool>>();

            Action act = () => new RabbitMqConnectionPool(logger: logger, options: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("options");
        }
    }
}
