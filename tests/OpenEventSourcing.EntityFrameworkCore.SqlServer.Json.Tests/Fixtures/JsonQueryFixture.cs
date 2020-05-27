using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenEventSourcing.EntityFrameworkCore.Extensions;
using OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Extensions;
using OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Tests.Fakes;

namespace OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Tests.Fixtures
{
    public class JsonQueryFixture : SharedStoreFixtureBase<JsonQueryContext>
    {
        protected override string StoreName => "JsonQueryTest";
        protected override ITestStoreFactory TestStoreFactory => SqlServerJsonTestStoreFactory.Instance;
        public TestSqlLoggerFactory TestSqlLoggerFactory => (TestSqlLoggerFactory)ListLoggerFactory;
        protected override void Seed(JsonQueryContext context) => context.Seed();
    }

    public class JsonQueryContext : PoolableDbContext
    {
        public DbSet<JsonEntity> JsonEntities { get; set; }

        public JsonQueryContext(DbContextOptions<JsonQueryContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<JsonEntity>().Property(p => p.Customer)
                   .HasJsonValueConversion();

            builder.Entity<JsonEntity>().Property(p => p.ToplevelArray)
                   .HasJsonValueConversion();

            builder.Entity<JsonEntity>().Property(p => p.TopLevelObjectArray)
                   .HasJsonValueConversion();
        }

        public void Seed()
        {
            JsonEntities.AddRange(new[]
            {
                new JsonEntity
                {
                    Customer = new Customer
                    {
                        Id = Guid.Empty,
                        Name = "Joe",
                        Age = 25,
                        IsVip = false,
                        Statistics = new Statistics
                        {
                            Visits = 4,
                            Purchases = 3,
                            Nested = new NestedStatistics
                            {
                                SomeProperty = 10,
                                SomeNullableProperty = 20,
                                IntArray = new[]
                                {
                                    3,
                                    4
                                }
                            }
                        },
                        Orders = new[]
                        {
                            new Order
                            {
                                Price = 99.5m,
                                ShippingAddress = "Some address 1",
                                ShippingDate = new DateTime(2019, 10, 1)
                            },
                            new Order
                            {
                                Price = 23,
                                ShippingAddress = "Some address 2",
                                ShippingDate = new DateTime(2019, 10, 10)
                            }
                        }
                    },
                    ToplevelArray = new []
                    {
                        1,
                        2,
                        3,
                    },
                    TopLevelObjectArray = new []
                    {
                        new Order
                        {
                            Price = 99.5m,
                            ShippingAddress = "Some address 1",
                            ShippingDate = new DateTime(2019, 10, 1)
                        },
                        new Order
                        {
                            Price = 23,
                            ShippingAddress = "Some address 2",
                            ShippingDate = new DateTime(2019, 10, 10)
                        },
                    }
                },
                new JsonEntity
                {
                    Customer = new Customer
                    {
                        Id = Guid.Parse("3272b593-bfe2-4ecf-81ae-4242b0632465"),
                        Name = "Moe",
                        Age = 35,
                        IsVip = true,
                        Statistics = new Statistics
                        {
                            Visits = 20,
                            Purchases = 25,
                            Nested = new NestedStatistics
                            {
                                SomeProperty = 20,
                                SomeNullableProperty = null,
                                IntArray = new[] { 5, 6 }
                            }
                        },
                        Orders = new[]
                        {
                            new Order
                            {
                                Price = 5,
                                ShippingAddress = "Moe's address",
                                ShippingDate = new DateTime(2019, 11, 3)
                            }
                        }
                    },
                }
            });

            SaveChanges();
        }
    }

    public class SqlServerJsonTestStoreFactory : RelationalTestStoreFactory
    {
        public static SqlServerJsonTestStoreFactory Instance { get; } = new SqlServerJsonTestStoreFactory();

        public override IServiceCollection AddProviderServices(IServiceCollection serviceCollection)
            => serviceCollection.AddEntityFrameworkCoreSqlServerJson();

        public override TestStore Create(string storeName)
            => SqlServerJsonTestStore.GetOrCreate(storeName);

        public override TestStore GetOrCreate(string storeName)
            => SqlServerJsonTestStore.GetOrCreate(storeName);
    }

    public class SqlServerJsonTestStore : RelationalTestStore
    {
        public static SqlServerJsonTestStore GetOrCreate(string name)
            => new SqlServerJsonTestStore(name);

        public static SqlServerJsonTestStore GetOrCreateInitialized(string name)
            => new SqlServerJsonTestStore(name).InitializeSqlServer(null, (Func<DbContext>)null, null);

        public static SqlServerJsonTestStore Create(string name, bool shared = false)
            => new SqlServerJsonTestStore(name, shared);

        public static SqlServerJsonTestStore CreateInitialized(string name, bool useFileName = false, bool? multipleActiveResultSets = null)
            => new SqlServerJsonTestStore(name, shared: false)
                .InitializeSqlServer(null, (Func<DbContext>)null, null);

        public SqlServerJsonTestStore(string name, bool shared = true)
            : base(name, shared)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .AddUserSecrets(typeof(JsonQueryFixture).Assembly, optional: true)
                .AddEnvironmentVariables(prefix: "OPENEVENTSOURCING_")
                .Build();

            Name = name;
            Connection = new SqlConnection(configuration.GetValue<string>("SqlServer:ConnectionString"));
            // TODO(Dan): Connection?
            // TODO(Dan): Connection string?
        }

        public SqlServerJsonTestStore InitializeSqlServer(
            IServiceProvider serviceProvider, Func<DbContext> createContext, Action<DbContext> seed)
            => (SqlServerJsonTestStore)Initialize(serviceProvider, createContext, seed);

        public SqlServerJsonTestStore InitializeSqlServer(
            IServiceProvider serviceProvider, Func<SqlServerJsonTestStore, DbContext> createContext, Action<DbContext> seed)
            => InitializeSqlServer(serviceProvider, () => createContext(this), seed);

        protected override void Initialize(Func<DbContext> createContext, Action<DbContext> seed, Action<DbContext> clean)
        {
            using (var context = createContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                seed?.Invoke(context);
            }
        }
        public override DbContextOptionsBuilder AddProviderOptions(DbContextOptionsBuilder builder)
        {
            return builder.UseSqlServer(Connection)
                          .EnableJsonSupport(o => o.UseCamelCase());
        }

        public override void Clean(DbContext context)
            => context.Database.EnsureDeleted();


    }
}
