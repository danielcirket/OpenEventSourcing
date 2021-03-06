﻿using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.RabbitMQ.Exceptions;
using OpenEventSourcing.RabbitMQ.Extensions;
using OpenEventSourcing.RabbitMQ.Management;
using OpenEventSourcing.Serialization.Json.Extensions;
using Xunit;

namespace OpenEventSourcing.RabbitMQ.Tests.Management
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
            
            ServiceProvider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
        }

        [RabbitMqTest]
        public void WhenCreateQueueAsyncCalledWithNonExistentQueueThenShouldSucceed()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IRabbitMqManagementClient>();
                var queueName = $"test-queue-{Guid.NewGuid()}";

                Func<Task> act = async () => await client.CreateQueueAsync(name: queueName, durable: false);

                act.Should().NotThrow();
            }
        }
        [RabbitMqTest]
        public void WhenCreateQueueAsyncCalledWithExistingQueueThenShouldThrowQueueAlreadyExistsException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IRabbitMqManagementClient>();
                var queueName = $"test-queue-{Guid.NewGuid()}";

                Func<Task> act = async () =>
                {
                    await client.CreateQueueAsync(name: queueName, durable: false);
                    await client.CreateQueueAsync(name: queueName, durable: false);
                };

                act.Should().Throw<QueueAlreadyExistsException>();
            }
        }
        [RabbitMqTest]
        public void WhenQueueExistsAsyncCalledWithNonExistentQueueThenShouldReturnFalse()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IRabbitMqManagementClient>();
                var queueName = $"test-queue-{Guid.NewGuid()}";

                Func<Task> verify = async () =>
                {
                    var result = await client.QueueExistsAsync(name: queueName);

                    result.Should().BeFalse();
                };

                verify.Should().NotThrow();
            }
        }
        [RabbitMqTest]
        public void WhenQueueExistsAsyncCalledWithExistingQueueThenShouldReturnTrue()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IRabbitMqManagementClient>();
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
        }
        [RabbitMqTest]
        public void WhenRemoveQueueAsyncCalledWithExistingQueueThenShouldSucceed()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IRabbitMqManagementClient>();
                var queueName = $"test-queue-{Guid.NewGuid()}";

                Func<Task> act = async () =>
                {
                    await client.CreateQueueAsync(name: queueName, durable: false);
                    await client.RemoveQueueAsync(name: queueName);
                };

                act.Should().NotThrow();
            }
        }
        [RabbitMqTest]
        public void WhenRemoveQueueAsyncCalledWithNonExistentQueueThenShouldThrowQueueNotFoundException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IRabbitMqManagementClient>();
                var queueName = $"test-queue-{Guid.NewGuid()}";

                Func<Task> act = async () => await client.RemoveQueueAsync(name: queueName);

                act.Should().Throw<QueueNotFoundException>();
            }
        }
    }
}
