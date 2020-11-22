using System;
using FluentAssertions;
using Xunit;

namespace OpenEventSourcing.Serialization.Json.Tests.Events
{
    public class JsonEventDeserializerTests
    {
        private readonly string _json = "{\"id\":\"00000000-0000-0000-0000-000000000000\",\"aggregateId\":\"00000000-0000-0000-0000-000000000000\",\"correlationId\":null,\"causationId\":null,\"timestamp\":\"9999-12-31T23:59:59.9999999+00:00\",\"version\":3,\"userId\":\"User\"}";

        [Fact]
        public void WhenDataIsNullThenShouldThrowArgumentNullException()
        {
            Action act = () => new JsonEventDeserializer().Deserialize<object>(null);

            act.Should().Throw<ArgumentNullException>();
        }
        [Fact]
        public void WhenDataIsNullThenShouldThrowArgumentNullExceptionNonGeneric()
        {
            Action act = () => new JsonEventDeserializer().Deserialize(null, typeof(object));

            act.Should().Throw<ArgumentNullException>();
        }
        [Fact]
        public void WhenDataIsNotNullThenShouldSerializeWithExpectedValue()
        {
            var serializer = new JsonEventDeserializer();

            var result = serializer.Deserialize<FakeEvent>(_json);

            result.Should().NotBeNull();
            result.Id.Should().Be(Guid.Empty.ToString());
            result.Subject.Should().Be(Guid.Empty.ToString());
            result.CausationId.Should().BeNull();
            result.CorrelationId.Should().BeNull();
            result.Timestamp.Should().Be(DateTimeOffset.MaxValue);
            result.Version.Should().Be(3);
            result.UserId.Should().BeEquivalentTo("User");
        }
        [Fact]
        public void WhenDataIsNotNullThenShouldSerializeWithExpectedValueNonGeneric()
        {
            var serializer = new JsonEventDeserializer();

            var result = (FakeEvent)serializer.Deserialize(_json, typeof(FakeEvent));

            result.Should().NotBeNull();
            result.Id.Should().Be(Guid.Empty.ToString());
            result.Subject.Should().Be(Guid.Empty.ToString());
            result.CausationId.Should().BeNull();
            result.CorrelationId.Should().BeNull();
            result.Timestamp.Should().Be(DateTimeOffset.MaxValue);
            result.Version.Should().Be(3);
            result.UserId.Should().BeEquivalentTo("User");
        }
    }
}
