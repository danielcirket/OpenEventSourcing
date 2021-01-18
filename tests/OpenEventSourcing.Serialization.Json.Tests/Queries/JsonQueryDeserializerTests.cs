using System;
using FluentAssertions;
using OpenEventSourcing.Queries;
using Xunit;

namespace OpenEventSourcing.Serialization.Json.Tests.Queries
{
    public class JsonQueryDeserializerTests
    {
        private readonly string _json = "{\"id\":\"00000000-0000-0000-0000-000000000000\",\"correlationId\":\"00000000-0000-0000-0000-000000000000\",\"timestamp\":\"9999-12-31T23:59:59.9999999+00:00\",\"userId\":\"User\"}";

        [Fact]
        public void WhenDataIsNullThenShouldThrowArgumentNullException()
        {
            Action act = () => new JsonQueryDeserializer().Deserialize<object>(null);

            act.Should().Throw<ArgumentNullException>();
        }
        [Fact]
        public void WhenDataIsNullThenShouldThrowArgumentNullExceptionNonGeneric()
        {
            Action act = () => new JsonQueryDeserializer().Deserialize(null, typeof(object));

            act.Should().Throw<ArgumentNullException>();
        }
        [Fact]
        public void WhenDataIsNotNullThenShouldSerializeWithExpectedValue()
        {
            var serializer = new JsonQueryDeserializer();

            var result = serializer.Deserialize<FakeQuery>(_json);

            result.Should().NotBeNull();
            result.Id.Should().Be(QueryId.From(Guid.Empty.ToString()));
            result.CorrelationId.Should().Be(CorrelationId.From(Guid.Empty.ToString()));
            result.Timestamp.Should().Be(DateTimeOffset.MaxValue);
            result.UserId.Should().BeEquivalentTo("User");
        }
        [Fact]
        public void WhenDataIsNotNullThenShouldSerializeWithExpectedValueNonGeneric()
        {
            var serializer = new JsonQueryDeserializer();

            var result = (FakeQuery)serializer.Deserialize(_json, typeof(FakeQuery));

            result.Should().NotBeNull();
            result.Id.Should().Be(QueryId.From(Guid.Empty.ToString()));
            result.CorrelationId.Should().Be(CorrelationId.From(Guid.Empty.ToString()));
            result.Timestamp.Should().Be(DateTimeOffset.MaxValue);
            result.UserId.Should().BeEquivalentTo("User");
        }
    }
}
