using System;
using FluentAssertions;
using Xunit;

namespace OpenEventSourcing.EntityFrameworkCore.Tests.EventModelFactory
{
    public class ConstructorTests
    {
        [Fact]
        public void WhenConstructedWithNullEventSerializerThenShouldThrowArgumentNullException()
        {
            Action act = () => new DefaultEventModelFactory(eventSerializer: null);

            act.Should().Throw<ArgumentNullException>().
                And.ParamName.Should().Be("eventSerializer");
        }
    }
}
