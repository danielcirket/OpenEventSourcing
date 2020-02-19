using System;
using OpenEventSourcing.Domain;
using OpenEventSourcing.Extensions;

namespace OpenEventSourcing.EntityFrameworkCore.Tests.Fakes
{
    public class FakeAggregate : Aggregate<FakeAggregateState>
    {
        public override FakeAggregateState GetState() => _state;

        public FakeAggregate(FakeAggregateState state)
            : base(state)
        {
            Id = Guid.NewGuid().ToString();
            Version = 0;

            Handles<FakeEvent>(Handle);
        }

        private void Handle(FakeEvent @event)
        {
            Id = @event.Subject;
        }

        internal void FakeAction()
        {
            Apply(new FakeEvent(Id, Version.GetValueOrDefault() + 1));
        }
    }

    public class FakeAggregateState : IAggregateState
    {
    }
}
