using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.RabbitMQ.Exceptions;
using OpenEventSourcing.RabbitMQ.Extensions;
using OpenEventSourcing.RabbitMQ.Management;
using OpenEventSourcing.Testing.Attributes;

namespace OpenEventSourcing.RabbitMQ.Tests.Management
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
                          .UseExchange(e =>
                          {
                              e.WithName("test-exchange");
                              e.UseExchangeType("topic");
                          });
                     });

            ServiceProvider = services.BuildServiceProvider();
        }

        [IntegrationTest]
        public void WhenCreateQueueAsyncCalledWithNonExistentQueueThenShouldSucceed()
        {
            var client = ServiceProvider.GetRequiredService<IRabbitMqManagementClient>();
            var queueName = $"test-queue-{Guid.NewGuid()}";

            Func<Task> act = async () => await client.CreateQueueAsync(name: queueName, durable: false);

            act.Should().NotThrow();
        }
        [IntegrationTest]
        public void WhenCreateQueueAsyncCalledWithExistingQueueThenShouldThrowQueueAlreadyExistsException()
        {
            var client = ServiceProvider.GetRequiredService<IRabbitMqManagementClient>();
            var queueName = $"test-queue-{Guid.NewGuid()}";

            Func<Task> act = async () =>
            {
                await client.CreateQueueAsync(name: queueName, durable: false);
                await client.CreateQueueAsync(name: queueName, durable: false);
            };

            act.Should().Throw<QueueAlreadyExistsException>();
        }
        [IntegrationTest]
        public void WhenQueueExistsAsyncCalledWithNonExistentQueueThenShouldReturnFalse()
        {
            var client = ServiceProvider.GetRequiredService<IRabbitMqManagementClient>();
            var queueName = $"test-queue-{Guid.NewGuid()}";

            Func<Task> verify = async () =>
            {
                var result = await client.QueueExistsAsync(name: queueName);

                result.Should().BeFalse();
            };

            verify.Should().NotThrow();
        }
        [IntegrationTest]
        public void WhenQueueExistsAsyncCalledWithExistingQueueThenShouldReturnTrue()
        {
            var client = ServiceProvider.GetRequiredService<IRabbitMqManagementClient>();
            var queueName = $"test-queue-{Guid.NewGuid()}";

            Func<Task> act = async () =>
            {
                await client.CreateQueueAsync(name: queueName, durable: false);
            };

            act.Should().NotThrow();

            Func<Task> verify = async () =>
            {
                var result = await client.QueueExistsAsync(name: queueName);

                result.Should().BeTrue();
            };

            verify.Should().NotThrow();
        }
        [IntegrationTest]
        public void WhenRemoveQueueAsyncCalledWithExistingQueueThenShouldSucceed()
        {
            var client = ServiceProvider.GetRequiredService<IRabbitMqManagementClient>();
            var queueName = $"test-queue-{Guid.NewGuid()}";

            Func<Task> act = async () =>
            {
                await client.CreateQueueAsync(name: queueName, durable: false);
                await client.RemoveQueueAsync(name: queueName);
            };

            act.Should().NotThrow();
        }
        [IntegrationTest]
        public void WhenRemoveQueueAsyncCalledWithNonExistentQueueThenShouldThrowQueueNotFoundException()
        {
            var client = ServiceProvider.GetRequiredService<IRabbitMqManagementClient>();
            var queueName = $"test-queue-{Guid.NewGuid()}";

            Func<Task> act = async () => await client.RemoveQueueAsync(name: queueName);

            act.Should().Throw<QueueNotFoundException>();
        }
    }
}
