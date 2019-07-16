using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using OpenEventSourcing.EntityFrameworkCore.Tests.Fakes;
using OpenEventSourcing.EntityFrameworkCore.ValueConversion;
using Xunit;

namespace OpenEventSourcing.EntityFrameworkCore.Tests.ValueConversion
{
    public class JsonValueConverterTests
    {
        private static readonly string _json = "{\"id\":null,\"name\":\"John Smith\",\"address\":{\"addressLine1\":\"1 Some Street\"}}";

        [Fact]
        public void WhenSerializedContainsExpectedValue()
        {
            var person = new Person { Name = "John Smith", Address = new Address { AddressLine1 = "1 Some Street" } };
            var converter = new JsonValueConverter<Person>();

            var serialized = (string)converter.ConvertToProvider(person);

            serialized.Should().BeEquivalentTo(_json, serialized);
        }
        [Fact]
        public void WhenDeserializedContainsExpectedValue()
        {
            var converter = new JsonValueConverter<Person>();

            var deserialized = (Person)converter.ConvertFromProvider(_json);

            deserialized.Should().NotBeNull();
            deserialized.Name.Should().Be("John Smith");
            deserialized.Address.Should().NotBeNull();
            deserialized.Address.AddressLine1.Should().Be("1 Some Street");
        }
    }
}
