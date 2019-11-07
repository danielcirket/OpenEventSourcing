using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OpenEventSourcing.RabbitMQ.Messages
{
    public class ReceivedMessage
    {
        public ReceivedMessage(BasicDeliverEventArgs args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            BasicProperties = args.BasicProperties;
            Body = args.Body;
            ConsumerTag = args.ConsumerTag;
            DeliveryTag = args.DeliveryTag;
            Exchange = args.Exchange;
            Redelivered = args.Redelivered;
            RoutingKey = args.RoutingKey;
        }
        /// <summary>
        /// The content header of the message.
        /// </summary>   
        public IBasicProperties BasicProperties { get; }
        /// <summary>
        /// The message body.
        /// </summary> 
        public byte[] Body { get; }
        /// <summary>
        /// The consumer tag of the consumer that the message was delivered to.
        /// </summary>
        public string ConsumerTag { get; }
        /// <summary>
        /// The delivery tag for this delivery. See IModel.BasicAck.
        /// </summary>
        public ulong DeliveryTag { get; }
        /// <summary>
        /// The exchange the message was originally published to.
        /// </summary>
        public string Exchange { get; }
        /// <summary>
        /// The AMQP "redelivered" flag.
        /// </summary>
        public bool Redelivered { get; }
        /// <summary>
        /// The routing key used when the message was originally published.
        /// </summary>
        public string RoutingKey { get; }
    }
}
