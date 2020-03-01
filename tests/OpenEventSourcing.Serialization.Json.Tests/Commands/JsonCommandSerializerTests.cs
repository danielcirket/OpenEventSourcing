using System;
using FluentAssertions;
using Xunit;

namespace OpenEventSourcing.Serialization.Json.Tests.Commands
{
    public class JsonCommandSerializerTests
    {
        private readonly string _json = "{\"id\":\"00000000-0000-0000-0000-000000000000\",\"subject\":\"00000000-0000-0000-0000-000000000000\",\"correlationId\":\"00000000-0000-0000-0000-000000000000\",\"timestamp\":\"9999-12-31T23:59:59.9999999+00:00\",\"version\":3,\"userId\":null}";

        [Fact]
        public void WhenDataIsNullThenShouldThrowArgumentNullException()
        {
            Action act = () => new JsonCommandSerializer().Serialize<object>(null);

            act.Should().Throw<ArgumentNullException>();
        }
        [Fact]
        public void WhenDataIsNotNullThenShouldSerializeWithExpectedValue()
        {
            var serializer = new JsonCommandSerializer();

            var result = serializer.Serialize(new FakeCommand());

            result.Should().BeEquivalentTo(_json);
        }
    }
}
