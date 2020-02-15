using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.RabbitMQ.Connections;
using OpenEventSourcing.RabbitMQ.Exceptions;
using OpenEventSourcing.RabbitMQ.Extensions;
using OpenEventSourcing.Serialization.Json.Extensions;
using OpenEventSourcing.Testing.Attributes;
using Xunit;

namespace OpenEventSourcing.RabbitMQ.Tests.Connections.Connection
{
    public class QueueTests : IClassFixture<ConfigurationFixture>
    {
        public IServiceProvider ServiceProvider { get; }

        public QueueTests(ConfigurationFixture fixture)
        {
            var services = new ServiceCollection();

            services.AddLogging(o => o.AddDebug())
                     .AddOpenEventSourcing()
                     .AddRabbitMq(o =>
                     {
                         o.UseConnection(fixture.Configuration.GetValue<string>("RabbitMQ:ConnectionString"))
                          .UseExchange(e =>
                          {
                              e.WithName("test-exchange");
                              e.UseExchangeType("topic");
                              e.AutoDelete();
                          });
                     })
                     .AddJsonSerializers();

#if NETCOREAPP3_0 || NETCOREAPP3_1
            ServiceProvider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
#else
            ServiceProvider = services.BuildServiceProvider(validateScopes: true);
#endif
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
