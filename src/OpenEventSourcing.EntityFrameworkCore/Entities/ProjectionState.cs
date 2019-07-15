using System;

namespace OpenEventSourcing.EntityFrameworkCore.Entities
{
    public class ProjectionState
    {
        public string Name { get; set; }
        public long Position { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset? LastModifiedDate { get; set; }
    }
}
