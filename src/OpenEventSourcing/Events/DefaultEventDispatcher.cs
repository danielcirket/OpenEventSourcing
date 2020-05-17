using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Extensions;

namespace OpenEventSourcing.Events
{
    internal sealed class EventDispatcher : IEventDispatcher
    {
        private readonly ILogger<EventDispatcher> _logger;
        private readonly IServiceProvider _serviceProvider;

        public EventDispatcher(ILogger<EventDispatcher> logger,
                               IServiceProvider serviceProvider)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task DispatchAsync<TEvent>(TEvent @event) where TEvent : IEvent
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            _logger.LogInformation($"Dispatching event '{typeof(TEvent).FriendlyName()}'.");

            var handlers = _serviceProvider.GetRequiredServices<IEventHandler<TEvent>>();

            _logger.LogInformation($"Resolved {handlers.Count()} handlers for event '{typeof(TEvent).FriendlyName()}'.");

            await Task.WhenAll(handlers.Select(handler => handler.HandleAsync(@event)));
        }
        public async Task DispatchAsync(IEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            // TODO(Dan): Cache these generic methods. The Event dispatcher is scoped generally, so that would require consideration.
            var methodInfos = GetType().GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var match = methodInfos.First(info => info.Name == nameof(DispatchAsync) && info.IsGenericMethod);
            var method = match.MakeGenericMethod(@event.GetType());

            await (Task)method.Invoke(this, new[] { @event });
        }
    }
}
