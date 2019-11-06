using Microsoft.EntityFrameworkCore;
using OpenEventSourcing.EntityFrameworkCore.Extensions;
using OpenEventSourcing.EntityFrameworkCore.Tests.Fakes;

namespace OpenEventSourcing.EntityFrameworkCore.Tests.ChangeTracking
{
    internal class InMemoryDbContext : DbContext
    {
        public DbSet<Person> People { get; set; }
        public DbSet<Customer> Customers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
            builder.UseInMemoryDatabase("Test");
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Person>(e =>
            {
                e.Property(p => p.Address).HasJsonValueConversion();
            });

            builder.Entity<Customer>(e =>
            {
                e.Property(p => p.Address).HasJsonValueConversion();
            });
        }
    }
}
