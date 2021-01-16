using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenEventSourcing.Events;
using OpenEventSourcing.RabbitMQ.Connections;
using OpenEventSourcing.RabbitMQ.Messages;
using OpenEventSourcing.RabbitMQ.Subscriptions;
using OpenEventSourcing.Serialization;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OpenEventSourcing.RabbitMQ.Queues
{
    internal sealed class DefaultQueueMessageReceiver : IQueueMessageReceiver
    {
        private readonly IOptions<RabbitMqOptions> _options;
        private readonly ILogger<DefaultQueueMessageReceiver> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IRabbitMqConnectionFactory _connectionFactory;
        private readonly IEventContextFactory _eventContextFactory;
        private CancellationTokenSource _stoppingCts;
        private List<Task> _executingTasks;

        public DefaultQueueMessageReceiver(IOptions<RabbitMqOptions> options,
                                           IRabbitMqConnectionFactory connectionFactory,
                                           IEventContextFactory eventContextFactory,
                                           ILogger<DefaultQueueMessageReceiver> logger,
                                           IServiceScopeFactory serviceScopeFactory)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (connectionFactory == null)
                throw new ArgumentNullException(nameof(connectionFactory));
            if (eventContextFactory == null)
                throw new ArgumentNullException(nameof(eventContextFactory));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (serviceScopeFactory == null)
                throw new ArgumentNullException(nameof(serviceScopeFactory));

            _options = options;
            _connectionFactory = connectionFactory;
            _eventContextFactory = eventContextFactory;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _stoppingCts = new CancellationTokenSource();
        }

        public async Task OnReceiveAsync(ReceivedMessage message, CancellationToken cancellationToken)
        {
            var eventName = message.RoutingKey;

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

                await eventDispatcher.DispatchAsync(context).ConfigureAwait(false);
            }
        }
        public Task OnErrorAsync(ReceivedMessage message, Exception ex)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"RabbitMQ message handler encountered an exception: '{ex.Message}'");
            sb.AppendLine($"{ex}");
            sb.AppendLine($"Exception context: ");
            sb.AppendLine($"- Exchange: {message.Exchange}");
            //sb.AppendLine($"- Queue: {message.Queue}");
            sb.AppendLine($"- Routing Key: {message.RoutingKey}");

            _logger.LogError(sb.ToString());

            return Task.CompletedTask;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var manager = scope.ServiceProvider.GetRequiredService<ISubscriptionManager>();
                await manager.ConfigureAsync();
            }

            var tasks = new List<Task>();

            foreach (var subscription in _options.Value.Subscriptions)
                tasks.Add(ExecuteAsync(subscription, _stoppingCts.Token));

            _executingTasks = tasks;
        }
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executingTasks.Count < 1)
                return;

            try
            {
                _stoppingCts.Cancel();
            }
            finally
            {
                _executingTasks.Add(Task.Delay(Timeout.Infinite, cancellationToken));

                await Task.WhenAny(_executingTasks);
            }
        }

        private async Task ExecuteAsync(RabbitMqSubscription subscription, CancellationToken cancellationToken)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken))
            {
                var underlyingConnection = connection.UnderlyingConnection;
                var channel = underlyingConnection.CreateModel();
                var consumer = new AsyncEventingBasicConsumer(channel);
                channel.BasicConsume(consumer: consumer, queue: subscription.Name, autoAck: false);

                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var message = await ReceiveAsync(consumer);

                        try
                        {
                            await OnReceiveAsync(message, cancellationToken).ConfigureAwait(false);

                            channel.BasicAck(message.DeliveryTag, multiple: false);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error: RabbitMQ Message Receiver error, could not consume message '{message.DeliveryTag}'");

                            channel.BasicNack(message.DeliveryTag, multiple: false, requeue: true);

                            await OnErrorAsync(message, ex).ConfigureAwait(false);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Unexpected receiver error: {ex.Message}");
                        throw;
                    }
                }

                channel.BasicCancel(consumer.ConsumerTag);
            }
        }

        private Task<ReceivedMessage> ReceiveAsync(AsyncEventingBasicConsumer consumer)
        {
            var tcs = new TaskCompletionSource<ReceivedMessage>();

            AsyncEventHandler<BasicDeliverEventArgs> handler = null;

            handler += (sender, args) =>
            {
                try
                {
                    var eventName = args.RoutingKey;
                    var eventData = Encoding.UTF8.GetString(args.Body);
                    var message = new ReceivedMessage(args);

                    _logger.LogInformation($"Received message. Type: '{eventName}' | Size: '{args.Body.Length}' bytes | Data: '{eventData}'");

                    tcs.SetResult(message);

                    return Task.CompletedTask;
                }
                finally
                {
                    consumer.Received -= handler;
                }
            };

            consumer.Received += handler;

            return tcs.Task;
        }
    }
}
