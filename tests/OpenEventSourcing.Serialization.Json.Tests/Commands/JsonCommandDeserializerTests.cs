using System;
using FluentAssertions;
using Xunit;

namespace OpenEventSourcing.Serialization.Json.Tests.Commands
{
    public class JsonCommandDeserializerTests
    {
        private readonly string _json = "{\"id\":\"00000000-0000-0000-0000-000000000000\",\"aggregateId\":\"00000000-0000-0000-0000-000000000000\",\"correlationId\":\"00000000-0000-0000-0000-000000000000\",\"timestamp\":\"9999-12-31T23:59:59.9999999+00:00\",\"version\":3,\"userId\":\"User\"}";

        [Fact]
        public void WhenDataIsNullThenShouldThrowArgumentNullException()
        {
            Action act = () => new JsonCommandDeserializer().Deserialize<object>(null);

            act.Should().Throw<ArgumentNullException>();
        }
        [Fact]
        public void WhenDataIsNullThenShouldThrowArgumentNullExceptionNonGeneric()
        {
            Action act = () => new JsonCommandDeserializer().Deserialize(null, typeof(object));

            act.Should().Throw<ArgumentNullException>();
        }
        [Fact]
        public void WhenDataIsNotNullThenShouldSerializeWithExpectedValue()
        {
            var serializer = new JsonCommandDeserializer();

            var result = serializer.Deserialize<FakeCommand>(_json);

            result.Should().NotBeNull();
            result.Id.Should().Be(Guid.Empty);
            result.Subject.Should().Be(Guid.Empty.ToString());
            result.CorrelationId.Should().Be(Guid.Empty);
            result.Timestamp.Should().Be(DateTimeOffset.MaxValue);
            result.Version.Should().Be(3);
            result.UserId.Should().BeEquivalentTo("User");
        }
        [Fact]
        public void WhenDataIsNotNullThenShouldSerializeWithExpectedValueNonGeneric()
        {
            var serializer = new JsonCommandDeserializer();

            var result = (FakeCommand)serializer.Deserialize(_json, typeof(FakeCommand));

            result.Should().NotBeNull();
            result.Id.Should().Be(Guid.Empty);
            result.Subject.Should().Be(Guid.Empty.ToString());
            result.CorrelationId.Should().Be(Guid.Empty);
            result.Timestamp.Should().Be(DateTimeOffset.MaxValue);
            result.Version.Should().Be(3);
            result.UserId.Should().BeEquivalentTo("User");
        }
    }
}
