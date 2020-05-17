using System;
using System.Threading;
using System.Threading.Tasks;
using OpenEventSourcing.Queries;

namespace OpenEventSourcing.Tests.Fakes
{
    internal sealed class FakeDispatchableQueryHandler : IQueryHandler<FakeDispatchableQuery, bool>
    {
        private int _handled = 0;
        public int Handled => _handled;

        public Task<bool> RetrieveAsync(FakeDispatchableQuery query, CancellationToken cancellationToken = default)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            Interlocked.Increment(ref _handled);

            return Task.FromResult(true);
        }
    }
}
