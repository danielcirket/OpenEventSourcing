using System.Threading;
using System.Threading.Tasks;

namespace OpenEventSourcing.Queries
{
    public interface IQueryStore
    {
        Task SaveAsync<TQueryResult>(IQuery<TQueryResult> query, CancellationToken cancellationToken = default);
    }
}
