using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Azure.ServiceBus.Messages;
using OpenEventSourcing.Events;
using OpenEventSourcing.Serialization;

namespace OpenEventSourcing.Azure.ServiceBus.Topics
{
    internal sealed class DefaultTopicMessageReceiver : ITopicMessageReceiver
    {
        private readonly ILogger<DefaultTopicMessageReceiver> _logger;
        private readonly IEventContextFactory _eventContextFactory;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public DefaultTopicMessageReceiver(ILogger<DefaultTopicMessageReceiver> logger,
                                           IEventContextFactory eventContextFactory,
                                           IServiceScopeFactory serviceScopeFactory)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (eventContextFactory == null)
                throw new ArgumentNullException(nameof(eventContextFactory));
            if (serviceScopeFactory == null)
                throw new ArgumentNullException(nameof(serviceScopeFactory));

            _logger = logger;
            _eventContextFactory = eventContextFactory;
            _serviceScopeFactory = serviceScopeFactory;
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
        public async Task ReceiveAsync(ISubscriptionClient client, Message message, CancellationToken cancellationToken = default)
        {
            var eventName = message.Label;            

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var eventTypeCache = scope.ServiceProvider.GetRequiredService<IEventTypeCache>();
                var eventDispatcher = scope.ServiceProvider.GetRequiredService<IEventDispatcher>();
                var eventDeserializer = scope.ServiceProvider.GetRequiredService<IEventDeserializer>();

                if (!eventTypeCache.TryGet(eventName, out var type))
                {
                    _logger.LogWarning($"Event type '{eventName}' could not be dispatched. Event type not found in event cache.");
                    return;
                }

                var context = _eventContextFactory.CreateContext(message);

                await eventDispatcher.DispatchAsync(context);
            }

            await client.CompleteAsync(message.SystemProperties.LockToken);
        }
    }
}
