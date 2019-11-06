using System;

namespace OpenEventSourcing.RabbitMQ.Messages
{
    public class Message
    {
        public string Type { get; set; }
        public byte[] Body { get; set; }
        public long Size => (Body != null) ? Body.Length : 0;

        //public IBasicProperties Properties { get; set; }
        public Guid MessageId { get; internal set; }
        public Guid? CorrelationId { get; internal set; }
    }
}
