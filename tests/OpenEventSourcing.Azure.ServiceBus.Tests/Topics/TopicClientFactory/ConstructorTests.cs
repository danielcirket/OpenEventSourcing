using System;
using FluentAssertions;
using Moq;
using OpenEventSourcing.Azure.ServiceBus.Management;
using OpenEventSourcing.Azure.ServiceBus.Topics;
using Xunit;

namespace OpenEventSourcing.Azure.ServiceBus.Tests.Topics.TopicClientFactory
{
    public class ConstructorTests
    {
        [Fact]
        public void WhenConstructedWithNullManagementClientThenShouldThrowArgumentNullException()
        {
            Action act = () => new DefaultTopicClientFactory(managementClient: null, connectionStringBuilder: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("managementClient");
        }
        [Fact]
        public void WhenConstructedWithNullConnectionStringBuilderThenShouldThrowArgumentNullException()
        {
            var client = Mock.Of<IServiceBusManagementClient>();

            Action act = () => new DefaultTopicClientFactory(managementClient: client, connectionStringBuilder: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("connectionStringBuilder");
        }
    }
}
