using System;
using FluentAssertions;
using OpenEventSourcing.Azure.ServiceBus.Subscriptions;
using Xunit;

namespace OpenEventSourcing.Azure.ServiceBus.Tests.Subscriptions.SubscriptionClientFactory
{
    public class ConstructorTests
    {
        [Fact]
        public void WhenConstructedWithNullConnectionStringBuilderThenShouldThrowArgumentNullException()
        {
            Action act = () => new DefaultSubscriptionClientFactory(connectionStringBuilder: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("connectionStringBuilder");
        }
    }
}
