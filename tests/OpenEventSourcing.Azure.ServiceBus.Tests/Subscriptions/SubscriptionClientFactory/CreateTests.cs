using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Microsoft.Azure.ServiceBus;
using Moq;
using OpenEventSourcing.Azure.ServiceBus.Subscriptions;
using Xunit;

namespace OpenEventSourcing.Azure.ServiceBus.Tests.Subscriptions.SubscriptionClientFactory
{
    public class CreateTests
    {
        [Fact]
        public void WhenCreateCalledWithNullSubscriptionThenShouldThrowArgumentNullException()
        {
            var builder = Mock.Of<ServiceBusConnectionStringBuilder>();
            var factory = new DefaultSubscriptionClientFactory(connectionStringBuilder: builder);

            Action act = () => factory.Create(subscription: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("subscription");
        }
        [Fact]
        public void WhenCreateCalledWithSubscriptionThenShouldReturnSubscriptionClient()
        {
            var builder = new ServiceBusConnectionStringBuilder("Endpoint=sb://fake.servicebus.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=DummyKey;;EntityPath=DummyEntityPath");
            var factory = new DefaultSubscriptionClientFactory(connectionStringBuilder: builder);

            var client = factory.Create(subscription: new ServiceBusSubscription { Name = "test-subscription" });

            client.Should().NotBeNull();
        }
    }
}
