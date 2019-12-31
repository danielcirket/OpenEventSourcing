using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Events;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.Azure.ServiceBus.Extensions;
using OpenEventSourcing.Serialization.Json.Extensions;
using OpenEventSourcing.Azure.ServiceBus.Topics;
using System.Threading.Tasks;
using FluentAssertions;
using OpenEventSourcing.Testing.Attributes;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;

namespace OpenEventSourcing.Azure.ServiceBus.Tests.Topics.Sender
{
    public class SendTests : IDisposable
    {
        public IServiceProvider ServiceProvider { get; }

        public SendTests()
        {
            var services = new ServiceCollection();

            services.AddLogging(o => o.AddDebug())
                    .AddOpenEventSourcing()
                    .AddAzureServiceBus(o =>
                    {
                        o.UseConnection(Environment.GetEnvironmentVariable("AZURE_SERVICE_BUS_CONNECTION_STRING") ?? "Endpoint=sb://openeventsourcing.servicebus.windows.net/;SharedAccessKeyName=DUMMY;SharedAccessKey=DUMMY")
                         .UseTopic(e =>
                         {
                             e.WithName($"test-topic-{Guid.NewGuid()}");
                         });
                    })
                    .AddJsonSerializers();

#if NETCOREAPP3_0
            ServiceProvider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
#else
            ServiceProvider = services.BuildServiceProvider(validateScopes: true);
#endif
        }

        [ServiceBusTest]
        public void WhenSendAsyncCalledWithSingleNullEventThenShouldThrowArgumentNullException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var sender = scope.ServiceProvider.GetRequiredService<ITopicMessageSender>();

                Func<Task> act = async () => await sender.SendAsync((IEvent)null);

                act.Should().Throw<ArgumentNullException>()
                    .And.ParamName.Should().Be("event");
            }
        }
        [ServiceBusTest]
        public void WhenSendAsyncCalledWithNullEventsThenShouldThrowArgumentNullException()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var sender = scope.ServiceProvider.GetRequiredService<ITopicMessageSender>();

                Func<Task> act = async () => await sender.SendAsync((IEnumerable<IEvent>)null);

                act.Should().Throw<ArgumentNullException>()
                    .And.ParamName.Should().Be("events");
            }
        }
        [ServiceBusTest]
        public void WhenSendAsyncCalledWithSingleEventThenShouldSendEvent()
        {
            using (var scope = ServiceProvider.CreateScope())
            { 
                var sender = scope.ServiceProvider.GetRequiredService<ITopicMessageSender>();
                var @event = new SameSenderEvent();

                Func<Task> act = async () => await sender.SendAsync(@event);

                act.Should().NotThrow();
            }
        }
        [ServiceBusTest]
        public void WhenSendAsyncCalledWithMultipleEventsThenShouldSendEvents()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var sender = scope.ServiceProvider.GetRequiredService<ITopicMessageSender>();
                var events = new[] { new SameSenderEvent(), new SameSenderEvent() };

                Func<Task> act = async () => await sender.SendAsync(events);

                act.Should().NotThrow();
            }
        }

        public void Dispose()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var builder = scope.ServiceProvider.GetRequiredService<ServiceBusConnectionStringBuilder>();
                var managementClient = new ManagementClient(builder);

                if (managementClient.TopicExistsAsync(builder.EntityPath).GetAwaiter().GetResult())
                    managementClient.DeleteTopicAsync(builder.EntityPath).GetAwaiter().GetResult();
            }
        }

        private class SameSenderEvent : Event
        {
            public SameSenderEvent() : base(Guid.NewGuid(), 1)
            {
            }
        }
    }
}
