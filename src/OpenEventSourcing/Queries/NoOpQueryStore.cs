using System.Threading;
using System.Threading.Tasks;

namespace OpenEventSourcing.Queries
{
    internal sealed class NoOpQueryStore : IQueryStore
    {
        public Task SaveAsync<TQueryResult>(IQuery<TQueryResult> query, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
