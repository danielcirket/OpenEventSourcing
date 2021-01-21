using System;
using OpenEventSourcing.Queries;

namespace OpenEventSourcing.Tests.Fakes
{
    internal class FakeQuery : Query<bool>
    {
        public FakeQuery() 
            : base(OpenEventSourcing.CorrelationId.From(Guid.NewGuid().ToString()), Actor.From("test")) { }
    }
}
