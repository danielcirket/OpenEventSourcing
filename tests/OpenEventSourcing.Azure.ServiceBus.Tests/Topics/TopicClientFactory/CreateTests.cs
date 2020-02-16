using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.ServiceBus;
using Moq;
using OpenEventSourcing.Azure.ServiceBus.Management;
using OpenEventSourcing.Azure.ServiceBus.Topics;
using Xunit;

namespace OpenEventSourcing.Azure.ServiceBus.Tests.Topics.TopicClientFactory
{
    public class CreateTests
    {
        [Fact]
        public async Task WhenCreateCalledWithTopicThenShouldReturnTopicClient()
        {
            var builder = new ServiceBusConnectionStringBuilder("Endpoint=sb://fake.servicebus.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=DummyKey;;EntityPath=DummyEntityPath");
            var manager = new Mock<IServiceBusManagementClient>();

            manager.Setup(m => m.CreateTopicAsync(It.IsAny<string>(), null, null))
                   .Returns(Task.FromResult(true));

            var factory = new DefaultTopicClientFactory(managementClient: manager.Object, connectionStringBuilder: builder);

            var client = await factory.CreateAsync();

            client.Should().NotBeNull();
        }
    }
}
