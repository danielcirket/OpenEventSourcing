using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyModel;
using OpenEventSourcing.EntityFrameworkCore.Entities;
using OpenEventSourcing.EntityFrameworkCore.EntityConfiguration;

namespace OpenEventSourcing.EntityFrameworkCore.DbContexts
{
    public sealed class OpenEventSourcingProjectionDbContext : DbContext
    {
        public DbSet<ProjectionState> ProjectionStates { get; set; }

        internal OpenEventSourcingProjectionDbContext(DbContextOptions<OpenEventSourcingProjectionDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ProjectionStateEntityTypeConfiguration());

            var types = DependencyContext.Default.RuntimeLibraries
                .SelectMany(library => library.GetDefaultAssemblyNames(DependencyContext.Default))
                .Select(Assembly.Load)
                .SelectMany(x => x.GetTypes())
                .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IProjectionTypeConfiguration<>)))
                .ToArray();

            foreach (var type in types)
                builder.ApplyConfiguration((dynamic)Activator.CreateInstance(type));

            base.OnModelCreating(builder);
        }
    }
}
