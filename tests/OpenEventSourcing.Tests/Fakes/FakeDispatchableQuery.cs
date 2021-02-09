using System;
using OpenEventSourcing.Queries;

namespace OpenEventSourcing.Tests.Fakes
{
    internal class FakeDispatchableQuery : Query<bool>
    {
        public FakeDispatchableQuery() 
            : base(OpenEventSourcing.CorrelationId.From(Guid.NewGuid().ToString()), Actor.From("test")) { }
    }
}
