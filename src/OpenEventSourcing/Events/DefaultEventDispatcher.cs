using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Extensions;

namespace OpenEventSourcing.Events
{
    internal sealed class DefaultEventDispatcher : IEventDispatcher
    {
        private readonly ILogger<DefaultEventDispatcher> _logger;
        private readonly IServiceProvider _serviceProvider;

        public DefaultEventDispatcher(ILogger<DefaultEventDispatcher> logger,
                               IServiceProvider serviceProvider)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task DispatchAsync<TEvent>(IEventContext<TEvent> context, CancellationToken cancellationToken = default) where TEvent : IEvent
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation($"Dispatching event '{typeof(TEvent).FriendlyName()}'.");

            var handlers = _serviceProvider.GetRequiredServices<IEventHandler<TEvent>>();

            _logger.LogInformation($"Resolved {handlers.Count()} handlers for event '{typeof(TEvent).FriendlyName()}'.");

            await Task.WhenAll(handlers.Select(handler => handler.HandleAsync(context, cancellationToken)));
        }
        public async Task DispatchAsync(IEventContext<IEvent> context, CancellationToken cancellationToken = default)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            // TODO(Dan): Cache these generic methods. The Event dispatcher is scoped generally, so that would require consideration.
            var methodInfos = GetType().GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var match = methodInfos.First(info => info.Name == nameof(DispatchAsync) && info.IsGenericMethod);
            var method = match.MakeGenericMethod(context.Payload.GetType());

            await (Task)method.Invoke(this, new object[] { context, cancellationToken });
        }
    }
}
