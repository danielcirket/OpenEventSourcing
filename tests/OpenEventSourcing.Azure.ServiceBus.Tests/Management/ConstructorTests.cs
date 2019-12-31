using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Azure.ServiceBus.Extensions;
using OpenEventSourcing.Azure.ServiceBus.Management;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.Serialization.Json.Extensions;
using Xunit;

namespace OpenEventSourcing.Azure.ServiceBus.Tests.Management
{
    public class ConstructorTests
    {
        [Fact]
        public void WhenConstructedWithNullManagementClientThenShouldThrowArgumentNullException()
        {
            Action act = () => new ServiceBusManagementClient(client: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("client");
        }
    }
}
