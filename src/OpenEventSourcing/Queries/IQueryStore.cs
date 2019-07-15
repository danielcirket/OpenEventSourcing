using System.Threading.Tasks;

namespace OpenEventSourcing.Queries
{
    public interface IQueryStore
    {
        Task SaveAsync<TQuery, TQueryResult>(TQuery query)
            where TQuery : class, IQuery<TQueryResult>;
    }
}
