using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using OpenEventSourcing.Events;
using OpenEventSourcing.Serialization;

namespace OpenEventSourcing.RabbitMQ.Messages
{
    internal sealed class DefaultEventContextFactory : IEventContextFactory
    {
        private readonly ConcurrentDictionary<Type, Activator<IEventContext<IEvent>>> _cache;
        private readonly IEventTypeCache _eventTypeCache;
        private readonly IEventDeserializer _eventDeserializer;

        public DefaultEventContextFactory(IEventTypeCache eventTypeCache,
                                          IEventDeserializer eventDeserializer)
        {
            if (eventTypeCache == null)
                throw new ArgumentNullException(nameof(eventTypeCache));
            if (eventDeserializer == null)
                throw new ArgumentNullException(nameof(eventDeserializer));

            _eventTypeCache = eventTypeCache;
            _eventDeserializer = eventDeserializer;
            _cache = new ConcurrentDictionary<Type, Activator<IEventContext<IEvent>>>();
        }

        public IEventContext<IEvent> CreateContext(ReceivedMessage message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (!_eventTypeCache.TryGet(message.RoutingKey, out var type))
                throw new ArgumentException($"Could not find event type for '{message.RoutingKey}'");

            var eventData = Encoding.UTF8.GetString(message.Body);
            var @event = (IEvent)_eventDeserializer.Deserialize(eventData, type);

            string streamId = null;
            CorrelationId? correlationId = null;
            CausationId? causationId = null;
            string userId = null;

            if (message.BasicProperties.Headers != null)
            {
                if (message.BasicProperties.Headers.ContainsKey(nameof(IEventContext<IEvent>.StreamId)))
                    streamId = message.BasicProperties.Headers[nameof(IEventContext<IEvent>.StreamId)]?.ToString();

                if (message.BasicProperties.Headers.ContainsKey(nameof(IEventContext<IEvent>.CorrelationId)))
                {
                    var value = message.BasicProperties.Headers[nameof(IEventContext<IEvent>.CorrelationId)]?.ToString();
                    correlationId = value != null ? CorrelationId.From(value) : (CorrelationId?)null;
                }

                if (message.BasicProperties.Headers.ContainsKey(nameof(IEventContext<IEvent>.CausationId)))
                {
                    var value = message.BasicProperties.Headers[nameof(IEventContext<IEvent>.CausationId)]?.ToString();
                    causationId = value != null ? CausationId.From(value) : (CausationId?)null;
                }

                if (message.BasicProperties.Headers.ContainsKey(nameof(IEventContext<IEvent>.UserId)))
                    userId = message.BasicProperties.Headers[nameof(IEventContext<IEvent>.UserId)]?.ToString();
            }

            if (_cache.TryGetValue(type, out var activator))
                return activator(streamId, @event, correlationId, causationId, @event.Timestamp, userId);

            activator = BuildActivator(typeof(EventContext<>).MakeGenericType(type));

            _cache.TryAdd(type, activator);

            return activator(streamId, @event, correlationId, causationId, @event.Timestamp, userId);
        }

        private Activator<IEventContext<IEvent>> BuildActivator(Type type)
        {
            var expectedParameterTypes = new Type[] { typeof(string), type.GenericTypeArguments[0], typeof(CorrelationId?), typeof(CausationId?), typeof(DateTimeOffset), typeof(string) };
            var constructor = type.GetConstructor(expectedParameterTypes);

            if (constructor == null)
                throw new InvalidOperationException($"Could not find expected constructor on type '{type.FullName}'. Expecting the following parameters: '{string.Join("' ,", expectedParameterTypes.Select(t => t.Name))}'");

            var parameters = constructor.GetParameters();
            var args = Expression.Parameter(typeof(object[]), "args");
            var argsExpressions = new Expression[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var parameterType = parameters[i].ParameterType;
                var accessor = Expression.ArrayIndex(args, Expression.Constant(i));
                var convert = Convert(accessor, parameterType);
                argsExpressions[i] = convert;
            }

            var newExpression = Expression.New(constructor, argsExpressions);
            var lambda = Expression.Lambda(typeof(Activator<IEventContext<IEvent>>), newExpression, args);

            return (Activator<IEventContext<IEvent>>)lambda.Compile();
        }

        private static Expression Convert(Expression expression, Type type)
        {
            if (type.GetTypeInfo().IsAssignableFrom(expression.Type.GetTypeInfo()))
                return expression;

            return Expression.Convert(expression, type);
        }

        private delegate T Activator<T>(params object[] args);
    }
}
