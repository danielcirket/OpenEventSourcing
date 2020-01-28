using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Azure.ServiceBus.Exceptions;
using OpenEventSourcing.Azure.ServiceBus.Extensions;
using OpenEventSourcing.Azure.ServiceBus.Management;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.Serialization.Json.Extensions;
using Xunit;

namespace OpenEventSourcing.Azure.ServiceBus.Tests.Management
{
    public class SubscriptionTests : ServiceBusSpecification, IDisposable
    {
        private readonly string _topicName = $"test-topic-{Guid.NewGuid()}";

        public SubscriptionTests(ConfigurationFixture fixture) : base(fixture) { }

        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(o => o.AddDebug())
                    .AddOpenEventSourcing()
                    .AddAzureServiceBus(o =>
                    {
                         o.UseConnection(Configuration.GetValue<string>("Azure:ServiceBus:ConnectionString"))
                          .UseTopic(e =>
                          {
                              e.WithName(_topicName);
                          });
                    })
                    .AddJsonSerializers();
        }

        [Fact]
        public void WhenCreateSubscriptionAsyncCalledWithNullTopicNameThenShouldThrowArgumentException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> verify = async () =>
                {
                    await client.CreateSubscriptionAsync(subscriptionName: $"test-sub-{Guid.NewGuid()}", topicName: null);
                };

                verify.Should().Throw<ArgumentException>()
                    .And.ParamName.Should().Be("topicName");
            }
        }
        [Fact]
        public void WhenCreateSubscriptionAsyncCalledWithNullSubscriptionNameThenShouldThrowArgumentException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> verify = async () =>
                {
                    await client.CreateSubscriptionAsync(subscriptionName: null, topicName: _topicName);
                };

                verify.Should().Throw<ArgumentException>()
                    .And.ParamName.Should().Be("subscriptionName");
            }
        }
        [ServiceBusTest]
        public void WhenCreateSubscriptionAsyncCalledWithNonExistentSubscriptionThenShouldSucceed()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();
                var subscriptionName = $"test-sub-{Guid.NewGuid()}";

                Func<Task> act = async () =>
                {
                    await client.CreateTopicAsync(topicName: _topicName);
                    await client.CreateSubscriptionAsync(subscriptionName: subscriptionName, topicName: _topicName);
                };

                act.Should().NotThrow();
            }
        }
        [ServiceBusTest]
        public void WhenCreateSubscriptionAsyncCalledWithNonExistentTopicThenShouldThrowTopicNotFoundException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();
                var topicName = $"test-topic-{Guid.NewGuid()}";
                var subscriptionName = $"test-sub-{Guid.NewGuid()}";

                Func<Task> act = async () => await client.CreateSubscriptionAsync(subscriptionName: subscriptionName, topicName: topicName);

                act.Should().Throw<TopicNotFoundException>()
                    .And.TopicName.Should().Be(topicName);
            }
        }
        [ServiceBusTest]
        public void WhenCreateSubscriptionAsyncCalledWithExistingSubscriptionThenShouldThrowSubscriptionAlreadyExistsException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();
                var subscriptionName = $"test-sub-{Guid.NewGuid()}";

                Func<Task> act = async () =>
                {
                    await client.CreateTopicAsync(topicName: _topicName);
                    await client.CreateSubscriptionAsync(subscriptionName: subscriptionName, topicName: _topicName);
                    await client.CreateSubscriptionAsync(subscriptionName: subscriptionName, topicName: _topicName);
                };

                act.Should().Throw<SubscriptionAlreadyExistsException>()
                    .And.SubscriptionName.Should().Be(subscriptionName);
            }
        }
        [ServiceBusTest]
        public void WhenSubscriptionExistsAsyncCalledWithExistingSubscriptionThenShouldReturnTrue()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();
                var subscriptionName = $"test-sub-{Guid.NewGuid()}";

                Func<Task> act = async () =>
                {
                    await client.CreateTopicAsync(topicName: _topicName);
                    await client.CreateSubscriptionAsync(subscriptionName: subscriptionName, topicName: _topicName);

                    var result = await client.SubscriptionExistsAsync(subscriptionName: subscriptionName, topicName: _topicName);

                    result.Should().BeTrue();
                };

                act.Should().NotThrow();
            }
        }
        [ServiceBusTest]
        public void WhenSubscriptionExistsAsyncCalledWithNonExistingTopicThenShouldReturnFalse()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();
                var subscriptionName = $"test-sub-{Guid.NewGuid()}";

                Func<Task> act = async () =>
                {
                    //await client.CreateTopicAsync(topicName: _topicName);

                    var result = await client.SubscriptionExistsAsync(subscriptionName: subscriptionName, topicName: _topicName);

                    result.Should().BeFalse();
                };

                act.Should().NotThrow();
            }
        }
        [ServiceBusTest]
        public void WhenSubscriptionExistsAsyncCalledWithNonExistingSubscriptionThenShouldReturnFalse()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();
                var subscriptionName = $"test-sub-{Guid.NewGuid()}";

                Func<Task> act = async () =>
                {
                    await client.CreateTopicAsync(topicName: _topicName);

                    var result = await client.SubscriptionExistsAsync(subscriptionName: subscriptionName, topicName: _topicName);

                    result.Should().BeFalse();
                };

                act.Should().NotThrow();
            }
        }
        [ServiceBusTest]
        public void WhenRemoveSubscriptionAsyncCalledWithNonExistentTopicThenShouldThrowTopicNotFoundException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();
                var subscriptionName = $"test-sub-{Guid.NewGuid()}";

                Func<Task> act = async () =>
                {
                    await client.RemoveSubscriptionAsync(subscriptionName: subscriptionName, topicName: _topicName);
                };

                act.Should().Throw<TopicNotFoundException>()
                    .And.TopicName.Should().Be(_topicName);
            }
        }
        [ServiceBusTest]
        public void WhenRemoveSubscriptionAsyncCalledWithNonExistentSubscriptionThenShouldThrowSubscriptionNotFoundException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();
                var subscriptionName = $"test-sub-{Guid.NewGuid()}";

                Func<Task> act = async () =>
                {
                    await client.CreateTopicAsync(topicName: _topicName);
                    await client.RemoveSubscriptionAsync(subscriptionName: subscriptionName, topicName: _topicName);
                };

                act.Should().Throw<SubscriptionNotFoundException>()
                    .And.SubscriptionName.Should().Be(subscriptionName);
            }
        }
        [ServiceBusTest]
        public void WhenRemoveSubscriptionAsyncCalledWithExistingSubscriptionThenShouldSucceed()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();
                var subscriptionName = $"test-sub-{Guid.NewGuid()}";

                Func<Task> act = async () =>
                {
                    await client.CreateTopicAsync(topicName: _topicName);
                    await client.CreateSubscriptionAsync(subscriptionName: subscriptionName, topicName: _topicName);
                    await client.RemoveSubscriptionAsync(subscriptionName: subscriptionName, topicName: _topicName);
                };

                act.Should().NotThrow();
            }
        }

        public void Dispose()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var builder = scope.ServiceProvider.GetRequiredService<ServiceBusConnectionStringBuilder>();
                var managementClient = new ManagementClient(builder);

                if (managementClient.TopicExistsAsync(_topicName).GetAwaiter().GetResult())
                    managementClient.DeleteTopicAsync(_topicName).GetAwaiter().GetResult();
            }
        }
    }
}
