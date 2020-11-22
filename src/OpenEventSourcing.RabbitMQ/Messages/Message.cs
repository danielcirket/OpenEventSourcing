using System;

namespace OpenEventSourcing.RabbitMQ.Messages
{
    public class Message
    {
        public string Type { get; set; }
        public byte[] Body { get; set; }
        public long Size => (Body != null) ? Body.Length : 0;
        public string MessageId { get; set; }
        public string CorrelationId { get; set; }
        public string CausationId { get; set; }
        public string UserId { get; set; }
    }
}
