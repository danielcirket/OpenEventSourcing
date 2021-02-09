using System;
using FluentAssertions;
using Xunit;

namespace OpenEventSourcing.Serialization.Json.Tests.Events
{
    public class JsonEventSerializerTests
    {
        private readonly string _json = "{\"id\":\"00000000-0000-0000-0000-000000000000\",\"subject\":\"00000000-0000-0000-0000-000000000000\",\"timestamp\":\"9999-12-31T23:59:59.9999999+00:00\",\"version\":2}";

        [Fact]
        public void WhenDataIsNullThenShouldThrowArgumentNullException()
        {
            Action act = () => new JsonEventSerializer().Serialize<object>(null);

            act.Should().Throw<ArgumentNullException>();
        }
        [Fact]
        public void WhenDataIsNotNullThenShouldSerializeWithExpectedValue()
        {
            var serializer = new JsonEventSerializer();

            var result = serializer.Serialize(new FakeEvent());

            result.Should().BeEquivalentTo(_json);
        }
    }
}
