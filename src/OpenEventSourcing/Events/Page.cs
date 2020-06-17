using System;
using System.Collections.Generic;

namespace OpenEventSourcing.Events
{
    public sealed class Page
    {
        public long Offset { get; }
        public long PreviousOffset { get; }
        public IEnumerable<IEventContext<IEvent>> Events { get; }

        public Page(long offset, long previousOffset, IEnumerable<IEventContext<IEvent>> events)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));

            Offset = offset;
            PreviousOffset = previousOffset;
            Events = events;
        }
    }
}
