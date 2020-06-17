using System;
using FluentAssertions;
using OpenEventSourcing.Azure.ServiceBus.Messages;
using Xunit;

namespace OpenEventSourcing.Azure.ServiceBus.Tests.Messages.MessageFactory
{
    public class ConstructorTests
    {
        [Fact]
        public void WhenConstructedWithNullEventSerializerThenShouldThrowArgumentNullException()
        {
            Action act = () => new DefaultMessageFactory(eventSerializer: null);

            act.Should().Throw<ArgumentNullException>()
                .And.ParamName.Should().Be("eventSerializer");
        }
    }
}
