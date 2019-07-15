using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OpenEventSourcing.EntityFrameworkCore.ChangeTracking;
using OpenEventSourcing.EntityFrameworkCore.Tests.Fakes;
using Xunit;

namespace OpenEventSourcing.EntityFrameworkCore.Tests.ChangeTracking
{
    public class JsonValueComparerTests
    {
        [Fact]
        public void WhenPropertyChangesWithJsonComparisonAndDoesNotImplementIEquatableThenShouldBeCorrectlyMarkedAsModified()
        {
            using (var context = new InMemoryDbContext())
            {
                var person = new Person { Name = "John Smith", Address = new Address { AddressLine1 = "1 Some Street" } };

                context.People.Add(person);
                context.SaveChanges();

                person = context.People.Find(person.Id);

                context.Entry(person).State.Should().Be(EntityState.Unchanged);

                person.Address.AddressLine1 = "2 Some Other Street";

                context.ChangeTracker.DetectChanges();

                context.Entry(person).State.Should().Be(EntityState.Modified);
                context.Entry(person).Property(p => p.Address).IsModified.Should().BeTrue();
            }
        }
        [Fact]
        public void WhenPropertyChangesWithJsonComparisonAndDoesNotImplementIEquatableAndChangedValueMatchesOriginalThenShouldBeCorrectlyMarkedAUnmodified()
        {
            using (var context = new InMemoryDbContext())
            {
                var person = new Person { Name = "John Smith", Address = new Address { AddressLine1 = "1 Some Street" } };

                context.People.Add(person);
                context.SaveChanges();

                person = context.People.Find(person.Id);

                context.Entry(person).State.Should().Be(EntityState.Unchanged);

                person.Address.AddressLine1 = "1 Some Street";

                context.ChangeTracker.DetectChanges();

                context.Entry(person).State.Should().Be(EntityState.Unchanged);
                context.Entry(person).Property(p => p.Address).IsModified.Should().BeFalse();
            }
        }

        [Fact]
        public void WhenPropertyChangesWithJsonComparisonAndImplementsIEquatableThenShouldBeCorrectlyMarkedAsModified()
        {
            using (var context = new InMemoryDbContext())
            {
                var customer = new Customer { Name = "John Smith", Address = new EquatableAddress { AddressLine1 = "1 Some Street" } };

                context.Customers.Add(customer);
                context.SaveChanges();

                customer = context.Customers.Find(customer.Id);

                context.Entry(customer).State.Should().Be(EntityState.Unchanged);

                customer.Address.AddressLine1 = "2 Some Other Street";

                context.ChangeTracker.DetectChanges();

                context.Entry(customer).State.Should().Be(EntityState.Modified);
                context.Entry(customer).Property(p => p.Address).IsModified.Should().BeTrue();
            }
        }
        [Fact]
        public void WhenPropertyChangesWithJsonComparisonAndDoesImplementIEquatableAndChangedValueMatchesOriginalThenShouldBeCorrectlyMarkedAUnmodified()
        {
            using (var context = new InMemoryDbContext())
            {
                var customer = new Customer { Name = "John Smith", Address = new EquatableAddress { AddressLine1 = "1 Some Street" } };

                context.Customers.Add(customer);
                context.SaveChanges();

                customer = context.Customers.Find(customer.Id);

                context.Entry(customer).State.Should().Be(EntityState.Unchanged);

                customer.Address.AddressLine1 = "1 Some Street";

                context.ChangeTracker.DetectChanges();

                context.Entry(customer).State.Should().Be(EntityState.Unchanged);
                context.Entry(customer).Property(p => p.Address).IsModified.Should().BeFalse();
            }
        }

        [Fact]
        public void WhenModelIsNullThenGetHashCodeShouldReturnZero()
        {
            var comparer = new JsonValueComparer<object>();

            var code = comparer.GetHashCode(null);

            code.Should().Be(0);
        }
        [Fact]
        public void WhenModelIsNonNullThenGetHashCodeShouldExpectedHashCode()
        {
            var comparer = new JsonValueComparer<object>();
            var @object = new object();
            var expected = JsonConvert.SerializeObject(@object, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None, MissingMemberHandling = MissingMemberHandling.Ignore, ContractResolver = new CamelCasePropertyNamesContractResolver() }).GetHashCode();

            var code = comparer.GetHashCode(new object());

            code.Should().Be(expected);
        }
        [Fact]
        public void WhenComparingNonNullTypesThenEqualsShouldReturnExpectedResult()
        {
            GenericEqualsCompareTest<byte>(1, 2);
            GenericEqualsCompareTest<ushort>(1, 2);
            GenericEqualsCompareTest<uint>(1, 2);
            GenericEqualsCompareTest<ulong>(1, 2);
            GenericEqualsCompareTest<sbyte>(1, 2);
            GenericEqualsCompareTest<short>(1, 2);
            GenericEqualsCompareTest(1, 2);
            GenericEqualsCompareTest<long>(1, 2);
            GenericEqualsCompareTest<float>(1, 2);
            GenericEqualsCompareTest<double>(1, 2);
            GenericEqualsCompareTest<decimal>(1, 2);
            GenericEqualsCompareTest('A', 'B');
            GenericEqualsCompareTest<object>(1, "A");
        }

        private void GenericEqualsCompareTest<T>(T a, T b)
        {
            var comparer = new JsonValueComparer<T>();
            var equals = comparer.EqualsExpression.Compile();

            equals(a, a).Should().BeTrue();
            equals(b, b).Should().BeTrue();
            equals(a, b).Should().BeFalse();
            equals(b, a).Should().BeFalse();
        }
    }
}
