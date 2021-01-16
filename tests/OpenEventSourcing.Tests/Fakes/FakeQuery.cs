using System;
using OpenEventSourcing.Queries;

namespace OpenEventSourcing.Tests.Fakes
{
    internal class FakeQuery : Query<bool>
    {
        public FakeQuery() 
            : base(Guid.NewGuid().ToString(), "test") { }
    }
}
