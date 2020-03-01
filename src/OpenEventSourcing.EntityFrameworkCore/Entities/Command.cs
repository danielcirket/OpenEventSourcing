using System;

namespace OpenEventSourcing.EntityFrameworkCore.Entities
{
    public class Command
    {
        public long SequenceNo { get; set; }
        public Guid Id { get; set; }
        public string Subject { get; set; }
        public Guid CorrelationId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Data { get; set; }
        public string UserId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}
