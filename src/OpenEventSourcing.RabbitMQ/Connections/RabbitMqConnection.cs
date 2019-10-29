using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OpenEventSourcing.RabbitMQ.Exceptions;
using OpenEventSourcing.RabbitMQ.Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace OpenEventSourcing.RabbitMQ.Connections
{
    internal sealed class RabbitMqConnection : IRabbitMqConnection
    {
        private readonly RabbitMqConnectionPool _pool;
        private readonly IOptions<RabbitMqOptions> _options;
        private readonly ConcurrentQueue<IModel> _channels;
        
        private bool _disposed = false;

        public string ConnectionId { get; }
        public bool IsOpen => UnderlyingConnection.IsOpen;
        public IConnection UnderlyingConnection { get; }

        public RabbitMqConnection(string id, IConnection connection,
                                  RabbitMqConnectionPool pool,
                                  IOptions<RabbitMqOptions> options)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _options = options;
            _channels = new ConcurrentQueue<IModel>();
            _pool = pool;
            ConnectionId = id;
            UnderlyingConnection = connection;
        }

        public async Task PublishAsync(Message message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            await Task.Yield();

            var channel = CreateChannel();
            var properties = channel.CreateBasicProperties();

            properties.CorrelationId = message.CorrelationId.ToString();
            properties.MessageId = message.MessageId.ToString();
            properties.Persistent = true;
            properties.Type = message.Type;

            channel.BasicPublish(exchange: _options.Value.Exchange, mandatory: true, routingKey: message.Type, basicProperties: properties, body: message.Body);

            ReturnChannel(channel);
        }
        public async Task PublishAsync(IEnumerable<Message> messages)
        {
            if (messages == null)
                throw new ArgumentNullException(nameof(messages));

            await Task.Yield();

            var channel = CreateChannel();
            var batch = channel.CreateBasicPublishBatch();

            foreach (var message in messages)
            { 
                var properties = channel.CreateBasicProperties();

                properties.CorrelationId = message.CorrelationId.ToString();
                properties.MessageId = message.MessageId.ToString();
                properties.Persistent = true;
                properties.Type = message.Type;

                batch.Add(exchange: _options.Value.Exchange, mandatory: true, routingKey: message.Type, properties: properties, body: message.Body);
            }

            batch.Publish();

            ReturnChannel(channel);
        }
        public async Task CreateExchangeAsync(string name, string exchangeType, bool durable = true, bool autoDelete = false)
        {
            await Task.Yield();

            var channel = CreateChannel();

            if (await ExchangeExistsAsync(name))
                throw new ExchangeAlreadyExistsException(name);

            channel.ExchangeDeclare(name, exchangeType, durable, autoDelete, null);

            ReturnChannel(channel);
        }
        public async Task CreateQueueAsync(string name, bool durable = true, bool autoDelete = false)
        {
            await Task.Yield();

            var channel = CreateChannel();

            if (await QueueExistsAsync(name))
                throw new QueueAlreadyExistsException(name);

            channel.QueueDeclare(name, durable: durable, autoDelete: autoDelete, exclusive: false);

            ReturnChannel(channel);
        }
        public async Task CreateSubscriptionAsync(string routingKey, string queue, string exchange)
        {
            await Task.Yield();

            var channel = CreateChannel();

            try
            {
                channel.ExchangeDeclarePassive(exchange);
            }
            catch (OperationInterruptedException)
            {
                // NOTE(Dan): If we get here is means that the exhange doesn't exist.
                throw new ExchangeNotFoundException(queue);
            }

            try
            {
                channel.QueueDeclarePassive(queue);
            }
            catch (OperationInterruptedException)
            {
                // NOTE(Dan): If we get here is means that the queue doesn't exist.
                throw new QueueNotFoundException(queue);
            }

            channel.QueueBind(queue, exchange, routingKey, null);

            ReturnChannel(channel);
        }
        public async Task RemoveExchangeAsync(string name)
        {
            await Task.Yield();

            var channel = CreateChannel();

            try
            {
                channel.ExchangeDeclarePassive(name);
            }
            catch (OperationInterruptedException)
            {
                // NOTE(Dan): If we get here is means that the exhange doesn't exist, so we want to create it.
                //            However we now need to create a new channel, as our current one will be closed.
                ReturnChannel(channel);
                throw new ExchangeNotFoundException(name);
            }

            channel.ExchangeDelete(name);
        }
        public async Task RemoveQueueAsync(string name)
        {
            await Task.Yield();

            var channel = CreateChannel();

            try
            {
                channel.QueueDeclarePassive(name);
            }
            catch (OperationInterruptedException)
            {
                // NOTE(Dan): If we get here is means that the queue doesn't exist, so we can't remove it.
                //            However we now need to create a new channel, as our current one will be closed.
                ReturnChannel(channel);
                throw new QueueNotFoundException(name);
            }

            channel.QueueDelete(name);

            ReturnChannel(channel);
        }
        public async Task RemoveSubscriptionAsync(string filter, string queue, string exchange)
        {
            await Task.Yield();

            var channel = CreateChannel();

            try
            {
                channel.ExchangeDeclarePassive(exchange);
            }
            catch (OperationInterruptedException)
            {
                // NOTE(Dan): If we get here is means that the exhange doesn't exist.
                throw new ExchangeNotFoundException(queue);
            }

            try
            {
                channel.QueueDeclarePassive(queue);
            }
            catch (OperationInterruptedException)
            {
                // NOTE(Dan): If we get here is means that the queue doesn't exist.
                throw new QueueNotFoundException(queue);
            }

            channel.QueueUnbind(queue, exchange, filter);

            ReturnChannel(channel);
        }

        public async Task<bool> ExchangeExistsAsync(string name)
        {
            await Task.Yield();

            var channel = CreateChannel();

            try
            {
                channel.ExchangeDeclarePassive(name);

                return true;
            }
            catch (OperationInterruptedException)
            {
                return false;
            }
            finally
            {
                ReturnChannel(channel);
            }
        }
        public async Task<bool> QueueExistsAsync(string name)
        {
            await Task.Yield();

            var channel = CreateChannel();

            try
            {
                channel.QueueDeclarePassive(name);

                return true;
            }
            catch (OperationInterruptedException)
            {
                return false;
            }
            finally
            {
                ReturnChannel(channel);
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            _pool.ReturnConnection(this, reRegisterForFinalization: false);
        }
        public void Reset()
        {
            _disposed = false;
        }

        private IModel CreateChannel()
        {
            if (_channels.IsEmpty)
                return UnderlyingConnection.CreateModel();
            
            if (!_channels.TryDequeue(out var model))
                return UnderlyingConnection.CreateModel();

            if (model == null || !model.IsOpen)
                return UnderlyingConnection.CreateModel();

            return model;
        }
        private void ReturnChannel(IModel channel)
        {
            if (channel.IsOpen)
            {
                _channels.Enqueue(channel);
                return;
            }

            channel.Dispose();
        }

        ~RabbitMqConnection()
        {
            _pool?.ReturnConnection(this, reRegisterForFinalization: true);
        }

        // TODO(Dan): Deal with the underlying connection events: `ConnectionShutdown`, `CallbackException` and `ConnectionBlocked`
        //            
    }
}
