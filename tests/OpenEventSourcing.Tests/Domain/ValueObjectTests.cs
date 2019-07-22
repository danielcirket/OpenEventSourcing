using FluentAssertions;
using Xunit;

namespace OpenEventSourcing.Tests.Domain
{
    public class ValueObjectTests
    {
        [Fact]
        public void WhenValueObjectsAreEqualThenShouldReturnMatchingHashcodes()
        {
            var a = new FullName("FirstName", "LastName");
            var b = new FullName("FirstName", "LastName");

            var hash1 = a.GetHashCode();
            var hash2 = b.GetHashCode();

            hash1.Should().Be(hash2);
        }
        [Fact]
        public void WhenValueObjectsAreNotEqualThenShouldNotReturnMatchingHashcodes()
        {
            var a = new FullName("FirstName", "LastName");
            var b = new FullName("LastName", "LastName");

            var hash1 = a.GetHashCode();
            var hash2 = b.GetHashCode();

            hash1.Should().NotBe(hash2);
        }
        [Fact]
        public void WhenValueObjectIsNotNullComparingWithEqualOperatorToNullShouldReturnFalse()
        {
            var a = new FullName("FirstName", "LastName");
            FullName b = null;

            (a == b).Should().BeFalse();
        }
        [Fact]
        public void WhenValueObjectNotNullComparingWithEqualOperatorToValueObjectShouldReturnFalse()
        {
            FullName a = null;
            var b = new FullName("FirstName", "LastName");

            (a == b).Should().BeFalse();
        }
        [Fact]
        public void WhenValueObjectIsNotNullComparingWithNotEqualsOperatorToNullShouldReturnTrue()
        {
            var a = new FullName("FirstName", "LastName");
            FullName b = null;

            (a != b).Should().BeTrue();
        }
        [Fact]
        public void WhenValueObjectNotNullComparingWithNotEqualOperatorToValueObjectShouldReturnTrue()
        {
            FullName a = null;
            var b = new FullName("FirstName", "LastName");

            (a != b).Should().BeTrue();
        }
    }
}
