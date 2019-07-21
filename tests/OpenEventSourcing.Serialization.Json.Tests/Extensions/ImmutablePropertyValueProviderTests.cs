using System;
using System.Reflection;
using FluentAssertions;
using OpenEventSourcing.Serialization.Json.ValueProviders;
using Xunit;

namespace OpenEventSourcing.Serialization.Json.Tests.Extensions
{
    public class ImmutablePropertyValueProviderTests
    {
        private readonly int _refReadonlyField = 0;
        private int _field = 12;
        public ref readonly int RefReadonlyField => ref _refReadonlyField;
        public int Property { get; } = 15;
        public int PropertyWithSetter { get => _field; set => _field = value; }

        [Fact]
        public void WhenConstructorMemberInfoIsNullThenShouldThrowArgumentNullException()
        {
            Action act = () => new ImmutablePropertyValueProvider(null);

            act.Should().Throw<ArgumentNullException>();
        }
        [Fact]
        public void WhenMemberInfoIsNotPropertyInfoOrFieldInfoThenShouldThrowArgumentException()
        {
            var member = typeof(ImmutablePropertyValueProviderTests).GetMethod(nameof(WhenMemberInfoIsNotPropertyInfoOrFieldInfoThenShouldThrowArgumentException), BindingFlags.Public | BindingFlags.Instance);

            Action act = () => new ImmutablePropertyValueProvider(member);

            act.Should().Throw<ArgumentException>();
        }
        [Fact]
        public void WhenGetValueCalledWithByRefPropertyInfoThenShouldThrowInvalidOperationException()
        {
            var member = typeof(ImmutablePropertyValueProviderTests).GetProperty(nameof(RefReadonlyField), BindingFlags.Public | BindingFlags.Instance);

            var provider = new ImmutablePropertyValueProvider(member);

            Action act = () => provider.GetValue(this);

            act.Should().Throw<InvalidOperationException>();
        }
        [Fact]
        public void WhenGetValueCalledWithPropertyInfoThenShouldGetCorrectValue()
        {
            var member = typeof(ImmutablePropertyValueProviderTests).GetProperty(nameof(Property), BindingFlags.Public | BindingFlags.Instance);

            var provider = new ImmutablePropertyValueProvider(member);

            var value = provider.GetValue(this);

            value.Should().Be(Property);
        }
        [Fact]
        public void WhenGetValueCalledWithFieldInfoThenShouldGetCorrectValue()
        {
            var member = typeof(ImmutablePropertyValueProviderTests).GetField(nameof(_field), BindingFlags.NonPublic | BindingFlags.Instance);

            var provider = new ImmutablePropertyValueProvider(member);

            var value = provider.GetValue(this);

            value.Should().Be(_field);
        }
        [Fact]
        public void WhenSetValueCalledWithPropertyInfoThenShouldSetCorrectValue()
        {
            var member = typeof(ImmutablePropertyValueProviderTests).GetProperty(nameof(Property), BindingFlags.Public | BindingFlags.Instance);

            var provider = new ImmutablePropertyValueProvider(member);

            provider.SetValue(this, 123);

            Property.Should().Be(123);
        }
        [Fact]
        public void WhenSetValueCalledWithPropertyInfoNonAutoPropertyThenShouldThrowInvalidOperationException()
        {
            var member = typeof(ImmutablePropertyValueProviderTests).GetProperty(nameof(PropertyWithSetter), BindingFlags.Public | BindingFlags.Instance);

            var provider = new ImmutablePropertyValueProvider(member);

            Action act = () => provider.SetValue(this, 123);

            act.Should().Throw<InvalidOperationException>();
        }
    }
}
