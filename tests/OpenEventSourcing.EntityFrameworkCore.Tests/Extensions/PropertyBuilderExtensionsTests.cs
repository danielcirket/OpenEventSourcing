using System;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using OpenEventSourcing.EntityFrameworkCore.Extensions;
using OpenEventSourcing.EntityFrameworkCore.Tests.Fakes;
using OpenEventSourcing.EntityFrameworkCore.ValueConversion;
using Xunit;

namespace OpenEventSourcing.EntityFrameworkCore.Tests.Extensions
{
    public class PropertyBuilderExtensionsTests
    {
        [Fact]
        public void WhenPropertyBuilderIsNullThenHasJsonValueConversionShouldThrowArgumentNullException()
        {
            PropertyBuilder<Address> builder = null;
            Action act = () => builder.HasJsonValueConversion();

            act.Should().Throw<ArgumentNullException>();
        }
        [Fact]
        public void WhenPropertyBuilderIsNonNullThenHasJsonValueConversionShouldApplyValueConverter()
        {
            var builder = new ModelBuilder(new ConventionSet());
            var property = builder.Entity<Person>().Property(x => x.Address);

            property.HasJsonValueConversion();

            var model = builder.Model;
            var modelType = model.FindEntityType(typeof(Person));
            var modelProperty = modelType.FindProperty(nameof(Person.Address));

            var comparer = modelProperty.GetValueConverter();

            comparer.Should().BeOfType<JsonValueConverter<Address>>();
        }
    }
}
