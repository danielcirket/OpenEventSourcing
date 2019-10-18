using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using OpenEventSourcing.RabbitMQ.Connections;
using RabbitMQ.Client;
using Xunit;

namespace OpenEventSourcing.RabbitMQ.Tests.Connections.Connection
{
    public partial class ConstructorTests
    {
        [Fact]
        public void WhenConstructedWithNullConnectionThenShouldThrowArgumentNullException()
        {
            Action act = () => new RabbitMqConnection(ConnectionId.New(), connection: null, pool: null, options: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("connection");
        }
        [Fact]
        public void WhenConstructedWithNullPoolThenShouldThrowArgumentNullException()
        {
            var connection = Mock.Of<IConnection>();

            Action act = () => new RabbitMqConnection(ConnectionId.New(), connection: connection, pool: null, options: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("pool");
        }
        [Fact]
        public void WhenConstructedWithNullOptionsThenShouldThrowArgumentNullException()
        {
            var connection = Mock.Of<IConnection>();
            var options = Mock.Of<IOptions<RabbitMqOptions>>();
            var logger = Mock.Of<ILogger<RabbitMqConnectionPool>>();

            var pool = new RabbitMqConnectionPool(logger, options);

            Action act = () => new RabbitMqConnection(ConnectionId.New(), connection: connection, pool: pool, options: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("options");
        }
    }
}
