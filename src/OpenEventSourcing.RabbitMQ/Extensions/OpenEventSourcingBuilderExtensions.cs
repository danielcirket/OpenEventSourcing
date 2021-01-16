using System;
using Microsoft.Extensions.DependencyInjection;
using OpenEventSourcing.Events;
using OpenEventSourcing.RabbitMQ.Connections;
using OpenEventSourcing.RabbitMQ.Management;
using OpenEventSourcing.RabbitMQ.Management.Api;
using OpenEventSourcing.RabbitMQ.Messages;
using OpenEventSourcing.RabbitMQ.Queues;
using OpenEventSourcing.RabbitMQ.Subscriptions;

namespace OpenEventSourcing.RabbitMQ.Extensions
{
    public static class OpenEventSourcingBuilderExtensions
    {
        public static IOpenEventSourcingBuilder AddRabbitMq(this IOpenEventSourcingBuilder builder, Action<RabbitMqOptions> optionsAction)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));


            builder.Services.Configure(optionsAction);

            builder.Services.AddScoped<IMessageFactory, DefaultMessageFactory>();
            builder.Services.AddScoped<IEventBusPublisher, RabbitMqEventBus>();
            builder.Services.AddScoped<IEventBusConsumer, RabbitMqEventBus>();
            builder.Services.AddSingleton<RabbitMqConnectionPool>();
            builder.Services.AddSingleton<IRabbitMqConnectionFactory, RabbitMqConnectionFactory>();
            builder.Services.AddScoped<IQueueMessageSender, DefaultQueueMessageSender>();
            builder.Services.AddSingleton<IQueueMessageReceiver, DefaultQueueMessageReceiver>();
            builder.Services.AddScoped<ISubscriptionManager, DefaultSubscriptionManager>();
            builder.Services.AddScoped<IRabbitMqManagementClient, RabbitMqManagementClient>();
            builder.Services.AddHttpClient<IRabbitMqManagementApiClient, RabbitMqManagementApiClient>();
            builder.Services.AddSingleton<IEventContextFactory, DefaultEventContextFactory>();

            return builder;
        }
    }
}
