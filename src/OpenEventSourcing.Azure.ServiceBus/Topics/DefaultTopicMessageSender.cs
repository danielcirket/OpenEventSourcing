using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using OpenEventSourcing.Azure.ServiceBus.Messages;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.Azure.ServiceBus.Topics
{
    internal sealed class DefaultTopicMessageSender : ITopicMessageSender
    {
        // NOTE(Dan): The max size is actually 256k (for teirs less than premium), but that doesn't take into account the message headers, 
        //            so we're being conservative here.
        private const int MAX_SERVICE_BUS_MESSAGE_SIZE = 192000;

        private readonly ITopicClientFactory _topicClientFactory;
        private readonly IMessageFactory _messageFactory;
        private readonly ILogger<DefaultTopicMessageSender> _logger;

        public DefaultTopicMessageSender(ITopicClientFactory topicClientFactory,
                                         IMessageFactory messageFactory,
                                         ILogger<DefaultTopicMessageSender> logger)
        {
            if (topicClientFactory == null)
                throw new ArgumentNullException(nameof(topicClientFactory));
            if (messageFactory == null)
                throw new ArgumentNullException(nameof(messageFactory));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _topicClientFactory = topicClientFactory;
            _messageFactory = messageFactory;
            _logger = logger;
        }

        public async Task SendAsync<TEvent>(TEvent @event) where TEvent : IEvent
        {
            var client = _topicClientFactory.Create();
            var message = _messageFactory.CreateMessage(@event);

            _logger.LogInformation($"Sending message 1 of 1. Type: '{message.Label}' | Size: '{message.Size}' bytes");

            await client.SendAsync(message);
        }
        public async Task SendAsync(IEnumerable<IEvent> events)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));

            var messages = events.Select(@event => _messageFactory.CreateMessage(@event));
            var batchedMessages = messages.Aggregate(new { Sum = 0L, Current = (List<Message>)null, Result = new List<List<Message>>() }, (agg, message) =>
            {
                var size = message.Size;

                if (agg.Current == null || agg.Sum + size > MAX_SERVICE_BUS_MESSAGE_SIZE)
                {
                    var current = new List<Message> { message };

                    agg.Result.Add(current);

                    return new { Sum = size, Current = current, agg.Result };
                }

                agg.Current.Add(message);

                return new { Sum = agg.Sum + size, agg.Current, agg.Result };
            }).Result;

            var topicClient = _topicClientFactory.Create();

            _logger.LogInformation($"Sending batched messages 1 of {messages.Count()}.");

            var tasks = batchedMessages.Select((batch, index) =>
            {
                _logger.LogInformation($"Sending batch {index + 1} of {batchedMessages.Count}. Message count: {batch.Count}.");

                return topicClient.SendAsync(batch);
            });

            await Task.WhenAll(tasks);

            _logger.LogInformation($"Sent batched messages 1 of {messages.Count()} in {batchedMessages.Count} batches.");
        }
        public async Task<string> SendAsync<TEvent>(TEvent @event, DateTimeOffset publishOnUtc) where TEvent : IEvent
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));
            if (publishOnUtc <= DateTimeOffset.UtcNow)
                throw new ArgumentOutOfRangeException(nameof(publishOnUtc), $"'{nameof(publishOnUtc)}' cannot be before now.");

            var message = _messageFactory.CreateMessage(@event);

            var client = _topicClientFactory.Create();

            _logger.LogInformation($"Queuing message 1 of 1. Type: '{message.Label}' | Size: '{message.Size}' bytes | Publish on: '{publishOnUtc.ToString("yyyy-MM-dd HH:mm:ss:ttt")}'");

            var enquedMessageId = await client.ScheduleMessageAsync(message, publishOnUtc);

            _logger.LogInformation($"Queued message 1 of 1. Type: '{message.Label}' | Size: '{message.Size}' bytes | Publish on: '{publishOnUtc.ToString("yyyy-MM-dd HH:mm:ss:ttt")}' | MessageId: '{enquedMessageId}'");

            return enquedMessageId.ToString();
        }
        public async Task CancelScheduledMessageAsync(string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
                throw new ArgumentException($"'{nameof(identifier)}' cannot be null or empty.", nameof(identifier));
            if (!long.TryParse(identifier, out var result))
                throw new FormatException($"Supplied {nameof(identifier)} is not in the correct format, expecting value which is a valid long.");

            var client = _topicClientFactory.Create();

            await client.CancelScheduledMessageAsync(result);
        }

    }
}
