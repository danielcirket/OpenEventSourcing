using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using OpenEventSourcing.Azure.ServiceBus.Extensions;
using OpenEventSourcing.Azure.ServiceBus.Messages;
using OpenEventSourcing.Events;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.Serialization;
using OpenEventSourcing.Serialization.Json.Extensions;
using Xunit;

namespace OpenEventSourcing.Azure.ServiceBus.Tests.Messages.EventContextFactory
{
    public class CreateContextTests : IClassFixture<ConfigurationFixture>
    {
        public IServiceProvider ServiceProvider { get; }

        public CreateContextTests(ConfigurationFixture fixture)
        {
            var services = new ServiceCollection();

            services.AddLogging(o => o.AddDebug())
                    .AddOpenEventSourcing()
                    .AddAzureServiceBus(o =>
                    {
                        o.UseConnection(Environment.GetEnvironmentVariable("AZURE_SERVICE_BUS_CONNECTION_STRING") ?? "Endpoint=sb://openeventsourcing.servicebus.windows.net/;SharedAccessKeyName=DUMMY;SharedAccessKey=DUMMY")
                         .UseTopic(t =>
                         {
                             t.WithName("test-exchange");
                             t.AutoDeleteOnIdleAfter(TimeSpan.FromMinutes(5));
                         });
                    })
                    .AddJsonSerializers();

#if NETCOREAPP3_0 || NETCOREAPP3_1
            ServiceProvider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true }).CreateScope().ServiceProvider;
#else
            ServiceProvider = services.BuildServiceProvider(validateScopes: true);
#endif
        }
        
        [Fact]
        public void WhenReceivedEventIsNullThenShouldThrowArgumentNullException()
        {
            var factory = ServiceProvider.GetRequiredService<IEventContextFactory>();

            Action act = () => factory.CreateContext(null);

            act.Should().Throw<ArgumentNullException>()
                .And
                .ParamName
                .Should().Be("message");
        }
        [Fact]
        public void WhenReceivedEventIsNotNullThenShouldReturnEventContext()
        {
            var serializer = ServiceProvider.GetRequiredService<IEventSerializer>();
            var factory = ServiceProvider.GetRequiredService<IEventContextFactory>();
            var @event = new CreateTestEvent();
            var causationId = Guid.NewGuid();
            var correlationId = Guid.NewGuid();
            var userId = "test-user";

            var message = new Message
            {
                MessageId = @event.Id.ToString(),
                Body = Encoding.UTF8.GetBytes(serializer.Serialize(@event)),
                Label = nameof(CreateTestEvent),
                CorrelationId = correlationId.ToString(),
                UserProperties =
                {
                    { nameof(IEventContext<IEvent>.CausationId), causationId.ToString() },
                    { nameof(IEventContext<IEvent>.UserId), userId },
                    { nameof(IEventContext<IEvent>.Timestamp), @event.Timestamp },
                },
            };

            var context = factory.CreateContext(message);

            context.CausationId.Should().Be(causationId);
            context.CorrelationId.Should().Be(correlationId);
            context.Payload.Should().BeOfType<CreateTestEvent>();
            context.Timestamp.Should().Be(@event.Timestamp);
            context.UserId.Should().Be(userId);
        }

        private class CreateTestEvent : Event
        {
            public CreateTestEvent() : base(Guid.NewGuid(), 1) { }
        }
    }
}
