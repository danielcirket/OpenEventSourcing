using System;

namespace OpenEventSourcing.RabbitMQ.Messages
{
    public class Message
    {
        public string Type { get; set; }
        public byte[] Body { get; set; }
        public long Size => (Body != null) ? Body.Length : 0;
        public Guid MessageId { get; set; }
        public Guid? CorrelationId { get; set; }
        public Guid? CausationId { get; set; }
        public string UserId { get; set; }
    }
}
