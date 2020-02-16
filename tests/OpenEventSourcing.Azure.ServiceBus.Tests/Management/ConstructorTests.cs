using System;
using FluentAssertions;
using Microsoft.Azure.ServiceBus.Management;
using OpenEventSourcing.Azure.ServiceBus.Management;
using Xunit;

namespace OpenEventSourcing.Azure.ServiceBus.Tests.Management
{
    public class ConstructorTests
    {
        [Fact]
        public void WhenConstructedWithNullManagementClientThenShouldThrowArgumentNullException()
        {
            Action act = () => new ServiceBusManagementClient(client: null, options: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("client");
        }
        [Fact]
        public void WhenConstructedWithNullOptionstThenShouldThrowArgumentNullException()
        {
            var client = new ManagementClient("Endpoint=sb://openeventsourcing.servicebus.windows.net/;SharedAccessKeyName=DUMMY;SharedAccessKey=DUMMY");

            Action act = () => new ServiceBusManagementClient(client: client, options: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("options");
        }
    }
}
