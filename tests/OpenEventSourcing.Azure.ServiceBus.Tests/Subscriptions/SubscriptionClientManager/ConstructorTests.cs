using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using OpenEventSourcing.Azure.ServiceBus.Subscriptions;
using OpenEventSourcing.Azure.ServiceBus.Topics;
using Xunit;

namespace OpenEventSourcing.Azure.ServiceBus.Tests.Subscriptions.SubscriptionClientManager
{
    public class ConstructorTests
    {
        [Fact]
        public void WhenConstructedWithNullOptionsThenShouldThrowArgumentNullException()
        {
            Action act = () => new DefaultSubscriptionClientManager(options: null, subscriptionClientFactory: null, messageReceiver: null, managementClient: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("options");
        }
        [Fact]
        public void WhenConstructedWithNullSubscriptionClientFactoryThenShouldThrowArgumentNullException()
        {
            var options = Mock.Of<IOptions<ServiceBusOptions>>();

            Action act = () => new DefaultSubscriptionClientManager(options: options, subscriptionClientFactory: null, messageReceiver: null, managementClient: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("subscriptionClientFactory");
        }
        [Fact]
        public void WhenConstructedWithNullMessageReceiverThenShouldThrowArgumentNullException()
        {
            var options = Mock.Of<IOptions<ServiceBusOptions>>();
            var subscriptionClientFactory = Mock.Of<ISubscriptionClientFactory>();

            Action act = () => new DefaultSubscriptionClientManager(options: options, subscriptionClientFactory: subscriptionClientFactory, messageReceiver: null, managementClient: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("messageReceiver");
        }
        [Fact]
        public void WhenConstructedWithNullManagementClientThenShouldThrowArgumentNullException()
        {
            var options = Mock.Of<IOptions<ServiceBusOptions>>();
            var subscriptionClientFactory = Mock.Of<ISubscriptionClientFactory>();
            var messageReceiver = Mock.Of<ITopicMessageReceiver>();

            Action act = () => new DefaultSubscriptionClientManager(options: options, subscriptionClientFactory: subscriptionClientFactory, messageReceiver: messageReceiver, managementClient: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("managementClient");
        }
    }
}
