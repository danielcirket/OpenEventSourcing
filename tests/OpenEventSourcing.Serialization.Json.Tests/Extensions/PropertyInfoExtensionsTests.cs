using System;
using System.Reflection;
using FluentAssertions;
using OpenEventSourcing.Serialization.Json.Extensions;
using Xunit;

namespace OpenEventSourcing.Serialization.Json.Tests.Extensions
{
    public class PropertyInfoExtensionsTests
    {
        private object HasNoSetter { get; }
        private object HasSetter { get; set; }

        [Fact]
        public void WhenPropertyInfoIsNullThenShouldThrowArgumentNullException()
        {
            PropertyInfo info = null;

            Action act = () => info.HasSetter();

            act.Should().Throw<ArgumentNullException>();
        }
        [Fact]
        public void WhenPropertyHasSetterThenShouldReturnTrue()
        {
            var info = typeof(PropertyInfoExtensionsTests).GetProperty(nameof(HasSetter), BindingFlags.NonPublic | BindingFlags.Instance);

            var result = info.HasSetter();

            result.Should().BeTrue();
        }
        [Fact]
        public void WhenPropertyHasNoSetterThenShouldReturnFalse()
        {
            var info = typeof(PropertyInfoExtensionsTests).GetProperty(nameof(HasNoSetter), BindingFlags.NonPublic | BindingFlags.Instance);

            var result = info.HasSetter();

            result.Should().BeFalse();
        }
    }
}
