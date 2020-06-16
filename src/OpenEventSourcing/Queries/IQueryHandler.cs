using System.Threading;
using System.Threading.Tasks;

namespace OpenEventSourcing.Queries
{
    public interface IQueryHandler<TRequest, TResult>
        where TRequest : IQuery<TResult>
    {
        Task<TResult> RetrieveAsync(TRequest query, CancellationToken cancellationToken = default);
    }
}
