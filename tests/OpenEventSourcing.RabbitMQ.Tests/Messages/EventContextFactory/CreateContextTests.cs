using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using OpenEventSourcing.Events;
using OpenEventSourcing.Extensions;
using OpenEventSourcing.RabbitMQ.Extensions;
using OpenEventSourcing.RabbitMQ.Messages;
using OpenEventSourcing.Serialization;
using OpenEventSourcing.Serialization.Json.Extensions;
using RabbitMQ.Client.Events;
using Xunit;
using MQ = RabbitMQ.Client;

namespace OpenEventSourcing.RabbitMQ.Tests.Messages
{
    public class CreateContextTests : IClassFixture<ConfigurationFixture>
    {
        public IServiceProvider ServiceProvider { get; }

        public CreateContextTests(ConfigurationFixture fixture)
        {
            var services = new ServiceCollection();

            services.AddLogging(o => o.AddDebug())
                    .AddOpenEventSourcing()
                    .AddRabbitMq(o =>
                    {
                        o.UseConnection(fixture.Configuration.GetValue<string>("RabbitMQ:ConnectionString"))
                         .UseExchange(e =>
                         {
                             e.WithName("test-exchange");
                             e.UseExchangeType("topic");
                             e.AutoDelete();
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
            var properties = Mock.Of<MQ.Impl.BasicProperties>();
            var @event = new CreateTestEvent();
            var causationId = Guid.NewGuid().ToString();
            var correlationId = Guid.NewGuid().ToString();
            var userId = "test-user";

            properties.Headers = new Dictionary<string, object>
            {
                { nameof(IEventContext<IEvent>.CausationId), causationId },
                { nameof(IEventContext<IEvent>.CorrelationId), correlationId },
                { nameof(IEventContext<IEvent>.UserId), userId },
            };

            var message = new ReceivedMessage(new BasicDeliverEventArgs
            {
                RoutingKey = nameof(CreateTestEvent),
                Body = Encoding.UTF8.GetBytes(serializer.Serialize(@event)),
                BasicProperties = properties,
            });
            var context = factory.CreateContext(message);

            context.CausationId.Should().Be(causationId);
            context.CorrelationId.Should().Be(correlationId);
            context.Payload.Should().BeOfType<CreateTestEvent>();
            context.Timestamp.Should().Be(@event.Timestamp);
            context.UserId.Should().Be(userId);
        }

        private class CreateTestEvent : Event
        {
            public CreateTestEvent() : base(Guid.NewGuid().ToString(), 1) { }
        }
    }
}
