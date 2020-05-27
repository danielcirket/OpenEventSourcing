using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using OpenEventSourcing.EntityFrameworkCore.Extensions;

namespace OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Tests.Fakes
{
    internal sealed class FakeDbContext : DbContext
    {
        public DbSet<Person> People { get; set; }
        public DbSet<Customer> Customers { get; set; }

        public FakeDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Person>().Property(p => p.Address)
                   .HasColumnType("json")
                   .HasJsonValueConversion();

            builder.Entity<Customer>().Property(p => p.Statistics)
                   .HasColumnType("json")
                   .HasJsonValueConversion();

            builder.Entity<Customer>().Property(p => p.Orders)
                   .HasColumnType("json")
                   .HasJsonValueConversion();
        }
    }
}
