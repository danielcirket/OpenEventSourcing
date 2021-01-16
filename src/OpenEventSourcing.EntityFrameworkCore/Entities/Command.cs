using System;

namespace OpenEventSourcing.EntityFrameworkCore.Entities
{
    public class Command
    {
        public long SequenceNo { get; set; }
        public string Id { get; set; }
        public string Subject { get; set; }
        public string CorrelationId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Data { get; set; }
        public string UserId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}
