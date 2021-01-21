using System;

namespace OpenEventSourcing.EntityFrameworkCore.Entities
{
    public class Event
    {
        public long SequenceNo { get; set; }
        public string Id { get; set; }
        public string StreamId { get; set; }
        public string CorrelationId { get; set; }
        public string CausationId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Data { get; set; }
        public string Actor { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}
