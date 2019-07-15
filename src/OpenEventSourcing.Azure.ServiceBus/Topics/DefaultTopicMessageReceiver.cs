using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Events;
using OpenEventSourcing.Serialization;

namespace OpenEventSourcing.Azure.ServiceBus.Topics
{
    internal sealed class DefaultTopicMessageReceiver : ITopicMessageReceiver
    {
        private readonly ILogger<DefaultTopicMessageReceiver> _logger;
        private readonly IServiceProvider _serviceProvider;

        public DefaultTopicMessageReceiver(ILogger<DefaultTopicMessageReceiver> logger,
                                           IServiceProvider serviceProvider)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public Task OnErrorAsync(ExceptionReceivedEventArgs error)
        {
            var context = error.ExceptionReceivedContext;

            var sb = new StringBuilder();
            sb.AppendLine($"Azure Message Bus handler encountered an exception: '{error.Exception.Message}'");
            sb.AppendLine($"{error.Exception}");
            sb.AppendLine($"Exception context: ");
            sb.AppendLine($"- Endpoint: {context.Endpoint}");
            sb.AppendLine($"- Entity Path: {context.EntityPath}");
            sb.AppendLine($"- Executing Action: {context.Action}");

            _logger.LogError(sb.ToString());

            return Task.CompletedTask;
        }
        public async Task RecieveAsync(ISubscriptionClient client, Message message, CancellationToken cancellationToken = default)
        {
            var eventName = message.Label;
            var eventData = Encoding.UTF8.GetString(message.Body);             

            using (var scope = _serviceProvider.CreateScope())
            {
                var eventTypeCache = scope.ServiceProvider.GetRequiredService<IEventTypeCache>();
                var eventDispatcher = scope.ServiceProvider.GetRequiredService<IEventDispatcher>();
                var eventDeserializer = scope.ServiceProvider.GetRequiredService<IEventDeserializer>();

                if (!eventTypeCache.TryGet(eventName, out var type))
                {
                    _logger.LogWarning($"Event type '{eventName}' could not be dispatched. Event type not found in event cache.");
                    return;
                }

                var @event = (IEvent)eventDeserializer.Deserialize(eventData, type);

                await eventDispatcher.DispatchAsync(@event);
            }

            await client.CompleteAsync(message.SystemProperties.LockToken);
        }
    }
}
