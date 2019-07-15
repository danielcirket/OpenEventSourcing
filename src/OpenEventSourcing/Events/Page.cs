using System;
using System.Collections.Generic;

namespace OpenEventSourcing.Events
{
    public sealed class Page
    {
        public long Offset { get; }
        public long PreviousOffset { get; }
        public IEnumerable<IEvent> Events { get; }

        public Page(long offset, long previousOffset, IEnumerable<IEvent> events)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));

            Offset = offset;
            PreviousOffset = previousOffset;
            Events = events;
        }
    }
}
