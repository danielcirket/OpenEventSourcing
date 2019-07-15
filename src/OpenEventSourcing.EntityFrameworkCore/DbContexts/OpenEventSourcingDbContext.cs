using Microsoft.EntityFrameworkCore;
using OpenEventSourcing.EntityFrameworkCore.Entities;
using OpenEventSourcing.EntityFrameworkCore.EntityConfiguration;

namespace OpenEventSourcing.EntityFrameworkCore.DbContexts
{
    public sealed class OpenEventSourcingDbContext : DbContext
    {
        public DbSet<Command> Commands { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Query> Queries { get; set; }

        internal OpenEventSourcingDbContext(DbContextOptions<OpenEventSourcingDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new CommandEntityTypeConfiguration());
            builder.ApplyConfiguration(new EventEntityTypeConfiguration());
            builder.ApplyConfiguration(new QueryEntityTypeConfiguration());
        }
    }
}
