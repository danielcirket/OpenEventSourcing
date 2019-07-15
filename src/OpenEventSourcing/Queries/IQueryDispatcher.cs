using System.Threading.Tasks;

namespace OpenEventSourcing.Queries
{
    public interface IQueryDispatcher
    {
        Task<TQueryResult> DispatchAsync<TQuery, TQueryResult>(TQuery query)
            where TQuery : class, IQuery<TQueryResult>;
    }
}
