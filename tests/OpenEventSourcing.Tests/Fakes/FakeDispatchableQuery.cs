using System;
using OpenEventSourcing.Queries;

namespace OpenEventSourcing.Tests.Fakes
{
    internal class FakeDispatchableQuery : Query<bool>
    {
        public FakeDispatchableQuery() 
            : base(Guid.NewGuid(), "test") { }
    }
}
