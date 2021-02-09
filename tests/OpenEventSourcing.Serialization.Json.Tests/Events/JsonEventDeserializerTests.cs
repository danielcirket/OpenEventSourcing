using System;
using FluentAssertions;
using OpenEventSourcing.Events;
using Xunit;

namespace OpenEventSourcing.Serialization.Json.Tests.Events
{
    public class JsonEventDeserializerTests
    {
        private readonly string _json = "{\"id\":\"5eb0ea02-7dde-4730-bd61-09fcf1065e11\",\"subject\":\"fdfbd5b3-cafb-4376-a3fa-7f74c20a0188\",\"timestamp\":\"9999-12-31T23:59:59.9999999+00:00\",\"version\":3}";

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
            result.Id.Should().Be(EventId.From(Guid.Parse("5eb0ea02-7dde-4730-bd61-09fcf1065e11").ToString()));
            result.Subject.Should().Be(Guid.Parse("fdfbd5b3-cafb-4376-a3fa-7f74c20a0188").ToString());
            result.Timestamp.Should().Be(DateTimeOffset.MaxValue);
            result.Version.Should().Be(3);
        }
        [Fact]
        public void WhenDataIsNotNullThenShouldSerializeWithExpectedValueNonGeneric()
        {
            var serializer = new JsonEventDeserializer();

            var result = (FakeEvent)serializer.Deserialize(_json, typeof(FakeEvent));

            result.Should().NotBeNull();
            result.Id.Should().Be(EventId.From(Guid.Parse("5eb0ea02-7dde-4730-bd61-09fcf1065e11").ToString()));
            result.Subject.Should().Be(Guid.Parse("fdfbd5b3-cafb-4376-a3fa-7f74c20a0188").ToString());
            result.Timestamp.Should().Be(DateTimeOffset.MaxValue);
            result.Version.Should().Be(3);
        }
    }
}
