using System;
using FluentAssertions;
using Moq;
using OpenEventSourcing.Events;
using Xunit;

namespace OpenEventSourcing.EntityFrameworkCore.Tests.EventContextFactory
{
    public class ConstructorTests
    {
        [Fact]
        public void WhenConstructedWithNullEventTypeCacheThenShouldThrowArgumentNullException()
        {
            Action act = () => new DefaultEventContextFactory(eventTypeCache: null, eventDeserializer: null);

            act.Should().Throw<ArgumentNullException>().
                And.ParamName.Should().Be("eventTypeCache");
        }

        [Fact]
        public void WhenConstructedWithNullEventSerializerThenShouldThrowArgumentNullException()
        {
            var cache = Mock.Of<IEventTypeCache>();

            Action act = () => new DefaultEventContextFactory(eventTypeCache: cache, eventDeserializer: null);

            act.Should().Throw<ArgumentNullException>().
                And.ParamName.Should().Be("eventDeserializer");
        }
    }
}
