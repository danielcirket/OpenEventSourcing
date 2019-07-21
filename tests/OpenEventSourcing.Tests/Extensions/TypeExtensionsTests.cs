using System;
using System.Collections.Generic;
using FluentAssertions;
using OpenEventSourcing.Extensions;
using Xunit;

namespace OpenEventSourcing.Tests.Extensions
{
    public class TypeExtensionsTests
    {
        [Fact]
        public void WhenTypeIsNullThenShouldThrowArgumentNullException()
        {
            Type type = null;

            Action act = () => type.FriendlyName();

            act.Should().Throw<ArgumentNullException>();
        }

        [Theory, MemberData(nameof(GetTypesAndExpectedFormattedNames))]
        public void WhenTypeIsNotNullThenShouldReturnCorrectFriendlyName(Type type, string expected)
        {
            var name = type.FriendlyName();

            name.Should().Be(expected);
        }

        public static IEnumerable<object[]> GetTypesAndExpectedFormattedNames()
        {
            yield return new object[] { typeof(int), nameof(Int32) };
            yield return new object[] { typeof(string), nameof(String) };
            yield return new object[] { typeof(List<int>), "List<Int32>" };
            yield return new object[] { typeof(Dictionary<int, string>), "Dictionary<Int32, String>" };
            yield return new object[] { typeof(List<int?>), "List<Int32?>" };
            yield return new object[] { typeof(Dictionary<int?, string>), "Dictionary<Int32?, String>" };
            yield return new object[] { typeof(List<List<int?>>), "List<List<Int32?>>" };
            yield return new object[] { typeof(object[]), "Object[]" };
        }
    }
}
