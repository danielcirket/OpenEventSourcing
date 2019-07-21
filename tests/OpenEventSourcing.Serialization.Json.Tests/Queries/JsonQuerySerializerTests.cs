using System;
using FluentAssertions;
using Xunit;

namespace OpenEventSourcing.Serialization.Json.Tests.Queries
{
    public class JsonQuerySerializerTests
    {
        private readonly string _json = "{\"id\":\"00000000-0000-0000-0000-000000000000\",\"timestamp\":\"0001-01-01T00:00:00+00:00\",\"correlationId\":\"00000000-0000-0000-0000-000000000000\",\"userId\":null}";

        [Fact]
        public void WhenDataIsNullThenShouldThrowArgumentNullException()
        {
            Action act = () => new JsonQuerySerializer().Serialize<object>(null);

            act.Should().Throw<ArgumentNullException>();
        }
        [Fact]
        public void WhenDataIsNotNullThenShouldSerializeWithExpectedValue()
        {
            var serializer = new JsonQuerySerializer();

            var result = serializer.Serialize(new FakeQuery());

            result.Should().BeEquivalentTo(_json);
        }
    }
}
