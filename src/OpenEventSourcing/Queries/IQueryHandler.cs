using System.Threading.Tasks;

namespace OpenEventSourcing.Queries
{
    public interface IQueryHandler<TRequest, TResult>
        where TRequest : IQuery<TResult>
        where TResult : IQueryResult
    {
        Task<TResult> RetrieveAsync(TRequest query);
    }
}
