﻿using System.Threading;
using System.Threading.Tasks;
using OpenEventSourcing.Events;

namespace OpenEventSourcing.Tests.Events
{
    internal class FakeEventHandler : IEventHandler<FakeEvent>
    {
        private int _calls = 0;

        public int Calls => _calls;

        public Task HandleAsync(IEventContext<FakeEvent> @event, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Interlocked.Increment(ref _calls);

            return Task.CompletedTask;
        }
    }
}
