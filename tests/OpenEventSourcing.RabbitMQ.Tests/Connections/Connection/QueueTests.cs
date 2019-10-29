using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.RabbitMQ.Connections;
using OpenEventSourcing.RabbitMQ.Exceptions;
using OpenEventSourcing.RabbitMQ.Extensions;
using OpenEventSourcing.RabbitMQ.Management;
using OpenEventSourcing.Testing.Attributes;

namespace OpenEventSourcing.RabbitMQ.Tests.Connections.Connection
{
    public class QueueTests
    {
        public IServiceProvider ServiceProvider { get; }

        public QueueTests()
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

        [IntegrationTest]
        public void WhenCreateQueueAsyncCalledWithNonExistentQueueThenShouldSucceed()
        {
            var factory = ServiceProvider.GetRequiredService<IRabbitMqConnectionFactory>();
            var queueName = $"test-queue-{Guid.NewGuid()}";

            Func<Task> act = async () =>
            {
                var connection = await factory.CreateConnectionAsync(CancellationToken.None);
                await connection.CreateQueueAsync(name: queueName, durable: false);
            };

            act.Should().NotThrow();

            Func<Task> verify = async () =>
            {
                var connection = await factory.CreateConnectionAsync(CancellationToken.None);
                await connection.CreateQueueAsync(name: queueName, durable: false);
            };

            verify.Should().Throw<QueueAlreadyExistsException>()
                .And.QueueName.Should().Be(queueName);
        }
        [IntegrationTest]
        public void WhenCreateQueueAsyncCalledWithExistingQueueThenShouldThrowQueueAlreadyExistsException()
        {
            var factory = ServiceProvider.GetRequiredService<IRabbitMqConnectionFactory>();
            var queueName = $"test-queue-{Guid.NewGuid()}";

            Func<Task> act = async () =>
            {
                var connection = await factory.CreateConnectionAsync(CancellationToken.None);
                await connection.CreateQueueAsync(name: queueName, durable: false);
                await connection.CreateQueueAsync(name: queueName, durable: false);
            };

            act.Should().Throw<QueueAlreadyExistsException>();
        }
        [IntegrationTest]
        public void WhenRemoveQueueAsyncCalledWithExistingQueueThenShouldSucceed()
        {
            var factory = ServiceProvider.GetRequiredService<IRabbitMqConnectionFactory>();
            var queueName = $"test-queue-{Guid.NewGuid()}";

            Func<Task> act = async () =>
            {
                var connection = await factory.CreateConnectionAsync(CancellationToken.None);
                await connection.CreateQueueAsync(name: queueName, durable: false);
                await connection.RemoveQueueAsync(name: queueName);
            };

            act.Should().NotThrow();

            Func<Task> verify = async () =>
            {
                var connection = await factory.CreateConnectionAsync(CancellationToken.None);
                await connection.RemoveQueueAsync(name: queueName);
            };

            verify.Should().Throw<QueueNotFoundException>()
                .And.QueueName.Should().Be(queueName);
        }
        [IntegrationTest]
        public void WhenRemoveQueueAsyncCalledWithNonExistentQueueThenShouldThrowQueueNotFoundException()
        {
            var factory = ServiceProvider.GetRequiredService<IRabbitMqConnectionFactory>();
            var queueName = $"test-queue-{Guid.NewGuid()}";

            Func<Task> act = async () =>
            {
                var connection = await factory.CreateConnectionAsync(CancellationToken.None);
                await connection.RemoveQueueAsync(name: queueName);
            };

            act.Should().Throw<QueueNotFoundException>();
        }
        [IntegrationTest]
        public void WhenQueueExistsAsyncCalledWithNonExistentQueueThenShouldReturnFalse()
        {
            var factory = ServiceProvider.GetRequiredService<IRabbitMqConnectionFactory>();
            var queueName = $"test-queue-{Guid.NewGuid()}";

            Func<Task> verify = async () =>
            {
                var connection = await factory.CreateConnectionAsync(CancellationToken.None);
                var result = await connection.QueueExistsAsync(name: queueName);

                result.Should().BeFalse();
            };

            verify.Should().NotThrow();
        }
        [IntegrationTest]
        public void WhenQueueExistsAsyncCalledWithExistingQueueThenShouldReturnTrue()
        {
            var factory = ServiceProvider.GetRequiredService<IRabbitMqConnectionFactory>();
            var queueName = $"test-queue-{Guid.NewGuid()}";

            Func<Task> act = async () =>
            {
                var connection = await factory.CreateConnectionAsync(CancellationToken.None);
                await connection.CreateQueueAsync(name: queueName, durable: false);
            };

            act.Should().NotThrow();

            Func<Task> verify = async () =>
            {
                var connection = await factory.CreateConnectionAsync(CancellationToken.None);
                var result = await connection.QueueExistsAsync(name: queueName);

                result.Should().BeTrue();
            };

            verify.Should().NotThrow();
        }
        [IntegrationTest]
        public void WhenCreateQueueAsyncCalledWithNonExistentQueueThenShouldSucceedAndPersistAfterConnection()
        {
            var factory = ServiceProvider.GetRequiredService<IRabbitMqConnectionFactory>();
            var queueName = $"test-queue-{Guid.NewGuid()}";

            Func<Task> act = async () =>
            {
                var connection = await factory.CreateConnectionAsync(CancellationToken.None);
                await connection.CreateQueueAsync(name: queueName, durable: false);
                connection.UnderlyingConnection.Dispose();
            };

            act.Should().NotThrow();

            Func<Task> verify = async () =>
            {
                var connection = await factory.CreateConnectionAsync(CancellationToken.None);
                await connection.CreateQueueAsync(name: queueName, durable: false);
            };

            verify.Should().Throw<QueueAlreadyExistsException>()
                .And.QueueName.Should().Be(queueName);
        }
    }
}
