using System;
using System.Linq;
using FluentAssertions;
using OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Tests.Fakes;
using OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Tests.Fixtures;
using Xunit;

namespace OpenEventSourcing.EntityFrameworkCore.SqlServer.Json.Tests
{
    public class JsonQueryTests : IClassFixture<JsonQueryFixture>
    {
        private readonly JsonQueryFixture _fixture;

        public JsonQueryTests(JsonQueryFixture fixture)
        {
            _fixture = fixture;
            _fixture.ListLoggerFactory.Clear();
        }

        private JsonQueryContext CreateContext() => _fixture.CreateContext();
        private void AssertSql(params string[] expected)
            => _fixture.TestSqlLoggerFactory.AssertBaseline(expected);


        [Fact]
        public void Roundtrip()
        {
            using (var context = CreateContext())
            {
                var entity = context.JsonEntities.Single(c => c.Id == 1);
                var customer = entity.Customer;
                var orders = customer.Orders;

                customer.Name.Should().Be("Joe");
                customer.Age.Should().Be(25);

                orders.Should().HaveCount(2);

                orders[0].Price.Should().Be(99.5m);
                orders[0].ShippingAddress.Should().Be("Some address 1");
                orders[0].ShippingDate.Should().Be(new DateTime(2019, 10, 1));

                orders[1].Price.Should().Be(23);
                orders[1].ShippingAddress.Should().Be("Some address 2");
                orders[1].ShippingDate.Should().Be(new DateTime(2019, 10, 10));
            }
        }

        [Fact]
        public void Literal()
        {
            using (var context = CreateContext())
            {
                var results = context.JsonEntities.Where(e => e.Customer == new Customer { Name = "Test customer", Age = 80 });

                results.Should().BeEmpty();
#if NETCOREAPP3_0
                AssertSql(
                    @"SELECT [j].[Id], [j].[Customer], [j].[TopLevelObjectArray], [j].[ToplevelArray]
FROM [JsonEntities] AS [j]
WHERE ([j].[Customer] = N'{""name"":""Test customer"",""age"":80,""id"":""00000000-0000-0000-0000-000000000000"",""isVip"":false,""statistics"":null,""orders"":null}') AND [j].[Customer] IS NOT NULL");
#elif NETCOREAPP3_1
                AssertSql(
                    @"SELECT [j].[Id], [j].[Customer], [j].[TopLevelObjectArray], [j].[ToplevelArray]
FROM [JsonEntities] AS [j]
WHERE [j].[Customer] = N'{""name"":""Test customer"",""age"":80,""id"":""00000000-0000-0000-0000-000000000000"",""isVip"":false,""statistics"":null,""orders"":null}'");
#else
    Assert.True(false);
#endif
            }
        }

        [Fact]
        public void Parameter()
        {
            using (var context = CreateContext())
            {
                var expected = context.JsonEntities.Find(1).Customer;
                var actual = context.JsonEntities.Single(c => c.Customer == expected).Customer;

                actual.Name.Should().Be(expected.Name);

#if NETCOREAPP3_0
                AssertSql(
                    @"@__p_0='1'

SELECT TOP(1) [j].[Id], [j].[Customer], [j].[TopLevelObjectArray], [j].[ToplevelArray]
FROM [JsonEntities] AS [j]
WHERE ([j].[Id] = @__p_0) AND @__p_0 IS NOT NULL",

                    @"@__expected_0='{""name"":""Joe"",""age"":25,""id"":""00000000-0000-0000-0000-000000000000"",""isVip"":false,""statistics"":{""text"":null,""visits"":4,""purchases"":3,""nested"":{""someProperty"":10,""someNullableProperty"":20,""intArray"":[3,4]}},""orders"":[{""price"":99.5,""shippingAddress"":""Some address 1"",""shippingDate"":""2019-10-01T00:00:00""},{""price"":23.0,""shippingAddress"":""Some address 2"",""shippingDate"":""2019-10-10T00:00:00""}]}' (Size = 4000)

SELECT TOP(2) [j].[Id], [j].[Customer], [j].[TopLevelObjectArray], [j].[ToplevelArray]
FROM [JsonEntities] AS [j]
WHERE (([j].[Customer] = @__expected_0) AND ([j].[Customer] IS NOT NULL AND @__expected_0 IS NOT NULL)) OR ([j].[Customer] IS NULL AND @__expected_0 IS NULL)");
#elif NETCOREAPP3_1
                AssertSql(
                    @"@__p_0='1'

SELECT TOP(1) [j].[Id], [j].[Customer], [j].[TopLevelObjectArray], [j].[ToplevelArray]
FROM [JsonEntities] AS [j]
WHERE [j].[Id] = @__p_0",

                    @"@__expected_0='{""name"":""Joe"",""age"":25,""id"":""00000000-0000-0000-0000-000000000000"",""isVip"":false,""statistics"":{""text"":null,""visits"":4,""purchases"":3,""nested"":{""someProperty"":10,""someNullableProperty"":20,""intArray"":[3,4]}},""orders"":[{""price"":99.5,""shippingAddress"":""Some address 1"",""shippingDate"":""2019-10-01T00:00:00""},{""price"":23.0,""shippingAddress"":""Some address 2"",""shippingDate"":""2019-10-10T00:00:00""}]}' (Size = 4000)

SELECT TOP(2) [j].[Id], [j].[Customer], [j].[TopLevelObjectArray], [j].[ToplevelArray]
FROM [JsonEntities] AS [j]
WHERE [j].[Customer] = @__expected_0");
#else
    Assert.True(false);
#endif
            }
        }

        [Fact]
        public void Text()
        {
            using (var context = CreateContext())
            {
                var customer = context.JsonEntities.Single(c => c.Customer.Name == "Joe");

                AssertSql(
                    @"SELECT TOP(2) [j].[Id], [j].[Customer], [j].[TopLevelObjectArray], [j].[ToplevelArray]
FROM [JsonEntities] AS [j]
WHERE JSON_VALUE([j].[Customer], '$.name') = N'Joe'");
            }
        }

        [Fact]
        public void Integer()
        {
            using (var context = CreateContext())
            {
                var customer = context.JsonEntities.Single(c => c.Customer.Age < 30);

                AssertSql(
                    @"SELECT TOP(2) [j].[Id], [j].[Customer], [j].[TopLevelObjectArray], [j].[ToplevelArray]
FROM [JsonEntities] AS [j]
WHERE CAST(JSON_VALUE([j].[Customer], '$.age') AS int) < 30");
            }
        }

        [Fact]
        public void UniqueIdentitifer()
        {
            using (var context = CreateContext())
            {
                var entity = context.JsonEntities.Single(c => c.Customer.Id == Guid.Empty);

                entity.Customer.Name.Should().Be("Joe");

                AssertSql(
                    @"SELECT TOP(2) [j].[Id], [j].[Customer], [j].[TopLevelObjectArray], [j].[ToplevelArray]
FROM [JsonEntities] AS [j]
WHERE CAST(JSON_VALUE([j].[Customer], '$.id') AS uniqueidentifier) = '00000000-0000-0000-0000-000000000000'");
            }
        }

        [Fact]
        public void Bool()
        {
            using (var context = CreateContext())
            {
                var entity = context.JsonEntities.Single(c => c.Customer.IsVip);

                entity.Customer.Name.Should().Be("Moe");

                AssertSql(
                    @"SELECT TOP(2) [j].[Id], [j].[Customer], [j].[TopLevelObjectArray], [j].[ToplevelArray]
FROM [JsonEntities] AS [j]
WHERE CAST(JSON_VALUE([j].[Customer], '$.isVip') AS bit) = CAST(1 AS bit)");
            }
        }

        [Fact]
        public void Nullable()
        {
            using (var context = CreateContext())
            {
                var entity = context.JsonEntities.Single(c => c.Customer.Statistics.Nested.SomeNullableProperty == 20);

                entity.Customer.Name.Should().Be("Joe");

                AssertSql(
                    @"SELECT TOP(2) [j].[Id], [j].[Customer], [j].[TopLevelObjectArray], [j].[ToplevelArray]
FROM [JsonEntities] AS [j]
WHERE CAST(JSON_VALUE([j].[Customer], '$.statistics.nested.someNullableProperty') AS int) = 20");
            }
        }

        [Fact]
        public void Nested()
        {
            using (var context = CreateContext())
            {
                var entity = context.JsonEntities.Single(c => c.Customer.Statistics.Visits == 4);

                entity.Customer.Name.Should().Be("Joe");

                AssertSql(
                    @"SELECT TOP(2) [j].[Id], [j].[Customer], [j].[TopLevelObjectArray], [j].[ToplevelArray]
FROM [JsonEntities] AS [j]
WHERE CAST(JSON_VALUE([j].[Customer], '$.statistics.visits') AS bigint) = CAST(4 AS bigint)");
            }
        }

        [Fact]
        public void NestedTwice()
        {
            using (var context = CreateContext())
            {
                var entity = context.JsonEntities.Single(c => c.Customer.Statistics.Nested.SomeProperty == 10);

                entity.Customer.Name.Should().Be("Joe");

                AssertSql(
                    @"SELECT TOP(2) [j].[Id], [j].[Customer], [j].[TopLevelObjectArray], [j].[ToplevelArray]
FROM [JsonEntities] AS [j]
WHERE CAST(JSON_VALUE([j].[Customer], '$.statistics.nested.someProperty') AS int) = 10");
            }
        }

        [Fact]
        public void ArrayOfObjectsComplexPropertyComparison()
        {
            using (var context = CreateContext())
            {
                var entity = context.JsonEntities.Single(c => c.Customer.Orders[0] == new Order { Price = 99.5m, ShippingAddress = "Some address 1", ShippingDate = new DateTime(2019, 10, 1) });

                entity.Customer.Name.Should().Be("Joe");

                AssertSql(
                    @"SELECT TOP(2) [j].[Id], [j].[Customer], [j].[TopLevelObjectArray], [j].[ToplevelArray]
FROM [JsonEntities] AS [j]
WHERE CAST(JSON_VALUE([j].[Customer], '$.orders[0].price') AS decimal(18,2)) = 99.5");
            }
        }

        [Fact]
        public void ArrayOfObjectPrimitiveProperty()
        {
            using (var context = CreateContext())
            {
                var entity = context.JsonEntities.Single(c => c.Customer.Orders[0].Price == 99.5m);

                entity.Customer.Name.Should().Be("Joe");

                AssertSql(
                    @"SELECT TOP(2) [j].[Id], [j].[Customer], [j].[TopLevelObjectArray], [j].[ToplevelArray]
FROM [JsonEntities] AS [j]
WHERE CAST(JSON_VALUE([j].[Customer], '$.orders[0].price') AS decimal(18,2)) = 99.5");
            }
        }

        [Fact]
        public void TopLevelArrayPrimitiveValue()
        {
            using (var context = CreateContext())
            {
                var entity = context.JsonEntities.Single(c => c.ToplevelArray[1] == 2);

                entity.Customer.Name.Should().Be("Joe");

#if NETCOREAPP3_0
                AssertSql(
                    @"SELECT TOP(2) [j].[Id], [j].[Customer], [j].[TopLevelObjectArray], [j].[ToplevelArray]
FROM [JsonEntities] AS [j]
WHERE (CAST(JSON_VALUE([j].[ToplevelArray], '$[1]') AS int) = 2) AND CAST(JSON_VALUE([j].[ToplevelArray], '$[1]') AS int) IS NOT NULL");
#elif NETCOREAPP3_1
                 AssertSql(
                    @"SELECT TOP(2) [j].[Id], [j].[Customer], [j].[TopLevelObjectArray], [j].[ToplevelArray]
FROM [JsonEntities] AS [j]
WHERE CAST(JSON_VALUE([j].[ToplevelArray], '$[1]') AS int) = 2");
#else
    Assert.True(false);
#endif
            }
        }

        [Fact]
        public void TopLevelArrayPrimitiveComparison()
        {
            using (var context = CreateContext())
            {
                var entity = context.JsonEntities.Single(c => c.ToplevelArray == new[] { 1, 2, 3 });

                entity.Customer.Name.Should().Be("Joe");

#if NETCOREAPP3_0
                AssertSql(
                    @"SELECT TOP(2) [j].[Id], [j].[Customer], [j].[TopLevelObjectArray], [j].[ToplevelArray]
FROM [JsonEntities] AS [j]
WHERE ([j].[ToplevelArray] = N'[1,2,3]') AND [j].[ToplevelArray] IS NOT NULL");
#elif NETCOREAPP3_1
                AssertSql(
                    @"SELECT TOP(2) [j].[Id], [j].[Customer], [j].[TopLevelObjectArray], [j].[ToplevelArray]
FROM [JsonEntities] AS [j]
WHERE [j].[ToplevelArray] = N'[1,2,3]'");
#else
    Assert.True(false);
#endif
            }
        }

        [Fact]
        public void TopLevelArrayComplexComparison()
        {
            using (var context = CreateContext())
            {
                var entity = context.JsonEntities.Single(c => c.TopLevelObjectArray == new[] { new Order { Price = 99.5m, ShippingAddress = "Some address 1", ShippingDate = new DateTime(2019, 10, 1) }, new Order { Price = 23, ShippingAddress = "Some address 2", ShippingDate = new DateTime(2019, 10, 10) } });

                entity.Customer.Name.Should().Be("Joe");

#if NETCOREAPP3_0
                AssertSql(
                    @"SELECT TOP(2) [j].[Id], [j].[Customer], [j].[TopLevelObjectArray], [j].[ToplevelArray]
FROM [JsonEntities] AS [j]
WHERE ([j].[TopLevelObjectArray] = N'[{""price"":99.5,""shippingAddress"":""Some address 1"",""shippingDate"":""2019-10-01T00:00:00""},{""price"":23.0,""shippingAddress"":""Some address 2"",""shippingDate"":""2019-10-10T00:00:00""}]') AND [j].[TopLevelObjectArray] IS NOT NULL");
#elif NETCOREAPP3_1
                AssertSql(
                    @"SELECT TOP(2) [j].[Id], [j].[Customer], [j].[TopLevelObjectArray], [j].[ToplevelArray]
FROM [JsonEntities] AS [j]
WHERE [j].[TopLevelObjectArray] = N'[{""price"":99.5,""shippingAddress"":""Some address 1"",""shippingDate"":""2019-10-01T00:00:00""},{""price"":23.0,""shippingAddress"":""Some address 2"",""shippingDate"":""2019-10-10T00:00:00""}]'");
#else
    Assert.True(false);
#endif
            }
        }



        [Fact]
        public void TopLevelArrayObjectComplexValueComparison()
        {
            using (var context = CreateContext())
            {
                var entity = context.JsonEntities.Single(c => c.TopLevelObjectArray[1] == new Order { Price = 99.5m, ShippingAddress = "Some address 1", ShippingDate = new DateTime(2019, 10, 1) });

                entity.Customer.Name.Should().Be("Joe");

                AssertSql(
                    @"SELECT TOP(2) [j].[Id], [j].[Customer], [j].[TopLevelObjectArray], [j].[ToplevelArray]
FROM [JsonEntities] AS [j]
WHERE JSON_VALUE([j].[TopLevelObjectArray], '$[1]') = N'{""price"":99.5,""shippingAddress"":""Some address 1"",""shippingDate"":""2019-10-01T00:00:00""}'");
            }
        }

        [Fact]
        public void NestedArray()
        {
            using (var context = CreateContext())
            {
                var entity = context.JsonEntities.Single(c => c.Customer.Statistics.Nested.IntArray[1] == 4);

                entity.Customer.Name.Should().Be("Joe");

                AssertSql(
                    @"SELECT TOP(2) [j].[Id], [j].[Customer], [j].[TopLevelObjectArray], [j].[ToplevelArray]
FROM [JsonEntities] AS [j]
WHERE CAST(JSON_VALUE([j].[Customer], '$.statistics.nested.intArray[1]') AS int) = 4");
            }
        }

        [Fact]
        public void ArrayIndexParameter()
        {
            using (var context = CreateContext())
            {
                var index = 1;
                var entity = context.JsonEntities.Single(c => c.Customer.Statistics.Nested.IntArray[index] == 4);

                entity.Customer.Name.Should().Be("Joe");

                false.Should().BeTrue();
            }
        }

        [Fact]
        public void ArrayLength()
        {
            using (var context = CreateContext())
            {
                var entity = context.JsonEntities.Single(c => c.Customer.Orders.Length == 2);

                entity.Customer.Name.Should().Be("Joe");

                AssertSql(
                    @"SELECT TOP(2) [j].[Id], [j].[Customer], [j].[TopLevelObjectArray], [j].[ToplevelArray]
FROM [JsonEntities] AS [j]
WHERE (SELECT COUNT(1) FROM OPENJSON([j].[Customer], '$.orders')) = 2");
            }
        }

        [Fact]
        public void TopLevelArrayLength()
        {
            using (var context = CreateContext())
            {
                var entity = context.JsonEntities.Single(c => c.ToplevelArray.Length > 0);

                entity.Customer.Name.Should().Be("Joe");

                AssertSql(
                    @"SELECT TOP(2) [j].[Id], [j].[Customer], [j].[TopLevelObjectArray], [j].[ToplevelArray]
FROM [JsonEntities] AS [j]
WHERE (SELECT COUNT(1) FROM OPENJSON([j].[ToplevelArray], '$')) > 0");
            }
        }

        [Fact]
        public void Like()
        {
            using (var context = CreateContext())
            {
                var entity = context.JsonEntities.Single(c => c.Customer.Name.StartsWith("J"));

                entity.Customer.Name.Should().Be("Joe");
#if NETCOREAPP3_0
                AssertSql(
                    @"SELECT TOP(2) [j].[Id], [j].[Customer], [j].[TopLevelObjectArray], [j].[ToplevelArray]
FROM [JsonEntities] AS [j]
WHERE JSON_VALUE([j].[Customer], '$.name') LIKE N'J%'");
#elif NETCOREAPP3_1
                AssertSql(
                                    @"SELECT TOP(2) [j].[Id], [j].[Customer], [j].[TopLevelObjectArray], [j].[ToplevelArray]
FROM [JsonEntities] AS [j]
WHERE JSON_VALUE([j].[Customer], '$.name') IS NOT NULL AND (JSON_VALUE([j].[Customer], '$.name') LIKE N'J%')");
#else
    Assert.True(false);
#endif
            }
        }


    }
}
