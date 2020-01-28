using System;
using System.Reflection;
using System.Threading;
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
using OpenEventSourcing.Events;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.Serialization.Json.Extensions;
using OpenEventSourcing.Testing.Attributes;
using Xunit;

namespace OpenEventSourcing.Azure.ServiceBus.Tests.Management
{
    public class RuleTests : ServiceBusSpecification, IDisposable
    {
        private readonly string _topicName = $"test-topic-{Guid.NewGuid()}";
        private readonly string _subscriptionName = $"test-sub-{Guid.NewGuid()}";
        private readonly string _ruleName = $"test-rule-{Guid.NewGuid()}";

        public RuleTests(ConfigurationFixture fixture) : base(fixture) { }

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
        public void WhenCreateRuleAsyncCalledWithNullRuleThenShouldThrowArgumentException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> verify = async () =>
                {
                    await client.CreateRuleAsync(ruleName: null, subscriptionName: $"test-sub-{Guid.NewGuid()}", topicName: _topicName);
                };

                verify.Should().Throw<ArgumentException>()
                    .And.ParamName.Should().Be("ruleName");
            }
        }
        [Fact]
        public void WhenCreateRuleAsyncCalledWithNullSubscriptionThenShouldThrowArgumentException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> verify = async () =>
                {
                    await client.CreateRuleAsync(ruleName: $"test-rule-{Guid.NewGuid()}", subscriptionName: null, topicName: _topicName);
                };

                verify.Should().Throw<ArgumentException>()
                    .And.ParamName.Should().Be("subscriptionName");
            }
        }
        [Fact]
        public void WhenCreateRuleAsyncCalledWithNullTopicThenShouldThrowArgumentException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> verify = async () =>
                {
                    await client.CreateRuleAsync(ruleName: $"test-rule-{Guid.NewGuid()}", subscriptionName: $"test-sub-{Guid.NewGuid()}", topicName: null);
                };

                verify.Should().Throw<ArgumentException>()
                    .And.ParamName.Should().Be("topicName");
            }
        }
        [ServiceBusTest]
        public void WhenCreateRuleAsyncCalledWithNonExistentTopicThenShouldThrowTopicNotFoundException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> verify = async () =>
                {
                    await client.CreateRuleAsync(ruleName: $"test-rule-{Guid.NewGuid()}", subscriptionName: $"test-sub-{Guid.NewGuid()}", topicName: _topicName);
                };

                verify.Should().Throw<TopicNotFoundException>()
                    .And.TopicName.Should().Be(_topicName);
            }
        }
        [ServiceBusTest]
        public void WhenCreateRuleAsyncCalledWithNonExistentSubscriptionThenShouldThrowSubscriptionNotFoundException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> setup = async () =>
                {
                    await client.CreateTopicAsync(_topicName);
                };

                setup.Should().NotThrow();

                Func<Task> verify = async () =>
                {
                    await client.CreateRuleAsync(ruleName: $"test-rule-{Guid.NewGuid()}", subscriptionName: _subscriptionName, topicName: _topicName);
                };

                verify.Should().Throw<SubscriptionNotFoundException>()
                    .And.SubscriptionName.Should().Be(_subscriptionName);
            }
        }
        [ServiceBusTest]
        public void WhenCreateRuleAsyncCalledWithNonExistentRuleThenShouldSucceed()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> setup = async () =>
                {
                    await client.CreateTopicAsync(_topicName);
                    await client.CreateSubscriptionAsync(_subscriptionName, _topicName);
                    await client.CreateRuleAsync(ruleName: _ruleName, subscriptionName: _subscriptionName, topicName: _topicName);
                };

                setup.Should().NotThrow();

                Func<Task> verify = async () =>
                {
                    var exists = await client.RuleExistsAsync(ruleName: _ruleName, subscriptionName: _subscriptionName, topicName: _topicName);

                    exists.Should().BeTrue();
                };

                verify.Should().NotThrow();
            }
        }
        [ServiceBusTest]
        public void WhenCreateRuleAsyncCalledWithExistingRuleThenShouldThrowRuleAlreadyExistsException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> setup = async () =>
                {
                    await client.CreateTopicAsync(_topicName);
                    await client.CreateSubscriptionAsync(_subscriptionName, _topicName);
                    await client.CreateRuleAsync(ruleName: _ruleName, subscriptionName: _subscriptionName, topicName: _topicName);
                };

                setup.Should().NotThrow();

                Func<Task> verify = async () =>
                {
                    await client.CreateRuleAsync(ruleName: _ruleName, subscriptionName: _subscriptionName, topicName: _topicName);
                };

                verify.Should().Throw<RuleAlreadyExistsException>()
                    .And.RuleName.Should().Be(_ruleName);
            }
        }
        [Fact]
        public void WhenRemoveRuleAsyncCalledWithNullRuleThenShouldThrowArgumentException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> verify = async () =>
                {
                    await client.RemoveRuleAsync(ruleName: null, subscriptionName: $"test-sub-{Guid.NewGuid()}", topicName: _topicName);
                };

                verify.Should().Throw<ArgumentException>()
                    .And.ParamName.Should().Be("ruleName");
            }
        }
        [Fact]
        public void WhenRemoveRuleAsyncCalledWithNullSubscriptionThenShouldThrowArgumentException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> verify = async () =>
                {
                    await client.RemoveRuleAsync(ruleName: $"test-rule-{Guid.NewGuid()}", subscriptionName: null, topicName: _topicName);
                };

                verify.Should().Throw<ArgumentException>()
                    .And.ParamName.Should().Be("subscriptionName");
            }
        }
        [Fact]
        public void WhenRemoveRuleAsyncCalledWithNullTopicThenShouldThrowArgumentException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> verify = async () =>
                {
                    await client.RemoveRuleAsync(ruleName: $"test-rule-{Guid.NewGuid()}", subscriptionName: $"test-sub-{Guid.NewGuid()}", topicName: null);
                };

                verify.Should().Throw<ArgumentException>()
                    .And.ParamName.Should().Be("topicName");
            }
        }
        [ServiceBusTest]
        public void WhenRemoveRuleAsyncCalledWithNonExistentTopicThenShouldThrowTopicNotFoundException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> act = async () =>
                {
                    await client.RemoveRuleAsync(ruleName:_ruleName, subscriptionName: _subscriptionName, topicName: _topicName);
                };

                act.Should().Throw<TopicNotFoundException>()
                    .And.TopicName.Should().Be(_topicName);
            }
        }
        [ServiceBusTest]
        public void WhenRemoveRuleAsyncCalledWithNonExistentSubscriptionThenShouldThrowSubscriptionNotFoundException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> setup = async () =>
                {
                    await client.CreateTopicAsync(_topicName);
                };

                setup.Should().NotThrow();

                Func<Task> act = async () =>
                {
                    await client.RemoveRuleAsync(ruleName: _ruleName, subscriptionName: _subscriptionName, topicName: _topicName);
                };

                act.Should().Throw<SubscriptionNotFoundException>()
                    .And.SubscriptionName.Should().Be(_subscriptionName);
            }
        }
        [ServiceBusTest]
        public void WhenRemoveRuleAsyncCalledWithNonExistentRuleThenShouldSucceed()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> setup = async () =>
                {
                    await client.CreateTopicAsync(_topicName);
                    await client.CreateSubscriptionAsync(_subscriptionName, _topicName);
                    await client.CreateRuleAsync(ruleName: _ruleName, subscriptionName: _subscriptionName, topicName: _topicName);
                };

                setup.Should().NotThrow();

                Func<Task> verify = async () =>
                {
                    var exists = await client.RuleExistsAsync(ruleName: _ruleName, subscriptionName: _subscriptionName, topicName: _topicName);

                    exists.Should().BeTrue();

                    await client.RemoveRuleAsync(ruleName: _ruleName, subscriptionName: _subscriptionName, topicName: _topicName);

                    exists = await client.RuleExistsAsync(ruleName: _ruleName, subscriptionName: _subscriptionName, topicName: _topicName);

                    exists.Should().BeFalse();
                };

                verify.Should().NotThrow();
            }
        }
        [Fact]
        public void WhenRetrieveRulesAsyncCalledWithNullSubscriptionThenShouldThrowArgumentException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> verify = async () =>
                {
                    await client.RetrieveRulesAsync(subscriptionName: null, topicName: _topicName);
                };

                verify.Should().Throw<ArgumentException>()
                    .And.ParamName.Should().Be("subscriptionName");
            }
        }
        [Fact]
        public void WhenRetrieveRulesAsyncCalledWithNullTopicThenShouldThrowArgumentException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> verify = async () =>
                {
                    await client.RetrieveRulesAsync(subscriptionName: $"test-sub-{Guid.NewGuid()}", topicName: null);
                };

                verify.Should().Throw<ArgumentException>()
                    .And.ParamName.Should().Be("topicName");
            }
        }
        [ServiceBusTest]
        public void WhenRetrieveRulesAsyncCalledWithSubscriptionWhichHasSingleRuleThenShouldReturnExpectedRule()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> setup = async () =>
                {
                    await client.CreateTopicAsync(_topicName);
                    await client.CreateSubscriptionAsync(_subscriptionName, _topicName);
                    await client.RemoveRuleAsync(RuleDescription.DefaultRuleName, _subscriptionName, _topicName);
                    await client.CreateRuleAsync(nameof(SampleEvent), _subscriptionName, _topicName);
                };

                setup.Should().NotThrow();

                Func<Task> verify = async () =>
                {
                    var rules = await client.RetrieveRulesAsync(subscriptionName: _subscriptionName, topicName: _topicName);

                    rules.Should().HaveCount(1);
                    rules.Should().OnlyContain(r => r.Rule == nameof(SampleEvent));
                };

                verify.Should().NotThrow();
            }
        }
        [ServiceBusTest]
        public void WhenRetrieveRulesAsyncCalledWithSubscriptionWhichHasMultipleRulesThenShouldReturnExpectedRules()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var ruleCount = 50;
                var client = scope.ServiceProvider.GetRequiredService<IServiceBusManagementClient>();

                Func<Task> setup = async () =>
                {
                    await client.CreateTopicAsync(_topicName);
                    await client.CreateSubscriptionAsync(_subscriptionName, _topicName);
                    await client.RemoveRuleAsync(RuleDescription.DefaultRuleName, _subscriptionName, _topicName);

                    for (var i = 0; i < ruleCount; i++)
                    {
                        await client.CreateRuleAsync($"{nameof(SampleEvent)}_{i}", _subscriptionName, _topicName);
                    }
                };

                setup.Should().NotThrow();

                Func<Task> verify = async () =>
                { 
                    var rules = await client.RetrieveRulesAsync(subscriptionName: _subscriptionName, topicName: _topicName);

                    rules.Should().HaveCount(ruleCount);
                    rules.Should().OnlyContain(r => r.Rule.StartsWith($"{nameof(SampleEvent)}_"));
                };

                verify.Should().NotThrow();
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

        private class SampleEvent : Event
        {
            public SampleEvent() : base(Guid.NewGuid(), 1)
            {
            }
        }
        private class SampleEventHandler : IEventHandler<SampleEvent>
        {
            private static int _received = 0;
            private static DateTimeOffset? _receivedTime;

            public static int Received => _received;
            public static DateTimeOffset? ReceivedAt => _receivedTime;

            public Task HandleAsync(SampleEvent @event)
            {
                Interlocked.Increment(ref _received);

                _receivedTime = DateTimeOffset.UtcNow;

                return Task.CompletedTask;
            }
        }
    }
}
