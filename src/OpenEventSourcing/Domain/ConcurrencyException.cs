using System;

namespace OpenEventSourcing.Domain
{
    public class ConcurrencyException : Exception
    {
        public ConcurrencyException(Guid aggregateId, int expectedVersion, int actualVersion)
            : base(BuildErrorMessage(aggregateId, expectedVersion, actualVersion)) { }

        private static string BuildErrorMessage(Guid aggregateId, int expectedVersion, int actualVersion)
            => $"Concurrency exception | Aggregate: {aggregateId} | Expected version: {expectedVersion} | Actual version: {actualVersion}";
    }
}
