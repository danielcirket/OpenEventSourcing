using System;
using System.Threading.Tasks;

namespace OpenEventSourcing.Projections
{
    public interface IProjectionWriter<TView>
        where TView : class
    {
        Task<TView> Add(Guid key, Func<TView> add);
        Task<TView> Update(Guid key, Func<TView, TView> update);
        Task<TView> Update(Guid key, Action<TView> update);

        //Task<TView> Update(Func<TView, TView> update);
        //Task<TView> Update(Action<TView> update);
        Task<TView> Remove(Guid key);
    }
}
