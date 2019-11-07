using System;
using System.Threading.Tasks;
using OpenEventSourcing.EntityFrameworkCore.DbContexts;
using OpenEventSourcing.Projections;

namespace OpenEventSourcing.EntityFrameworkCore.Projections
{
    internal sealed class EntityFrameworkCoreProjectionWriter<TView>
        : IProjectionWriter<TView>
        where TView : class
    {
        private readonly IProjectionDbContextFactory _projectionDbContextFactory;

        public EntityFrameworkCoreProjectionWriter(IProjectionDbContextFactory projectionDbContextFactory)
        {
            if (projectionDbContextFactory == null)
                throw new ArgumentNullException(nameof(projectionDbContextFactory));

            _projectionDbContextFactory = projectionDbContextFactory;
        }

        public async Task<TView> Add(Guid key, Func<TView> add)
        {
            using (var context = _projectionDbContextFactory.Create())
            {
                var entity = add();

                await context.Set<TView>().AddAsync(entity);
                await context.SaveChangesAsync();

                return entity;
            }
        }
        public async Task<TView> Remove(Guid key)
        {
            using (var context = _projectionDbContextFactory.Create())
            {
                var entity = await context.Set<TView>().FindAsync(key);

                if (entity == null)
                    return null;

                context.Set<TView>().Remove(entity);

                await context.SaveChangesAsync();

                return entity;
            }
        }
        public async Task<TView> Update(Guid key, Func<TView, TView> update)
        {
            using (var context = _projectionDbContextFactory.Create())
            {
                var entity = await context.Set<TView>().FindAsync(key);

                if (entity == null)
                    return null;

                update(entity);

                context.Set<TView>().Update(entity);

                await context.SaveChangesAsync();

                return entity;
            }
        }
        public async Task<TView> Update(Guid key, Action<TView> update)
        {
            return await Update(key, view =>
            {
                update(view);
                return view;
            });
        }
    }
}
