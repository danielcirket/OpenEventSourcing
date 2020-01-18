using System;

namespace OpenEventSourcing.Domain
{
    public class ConcurrencyException : Exception
    {
        public ConcurrencyException(Guid aggregateId, long expectedVersion, long actualVersion)
            : base(BuildErrorMessage(aggregateId, expectedVersion, actualVersion)) { }

        private static string BuildErrorMessage(Guid aggregateId, long expectedVersion, long actualVersion)
            => $"Concurrency exception | Aggregate: {aggregateId} | Expected version: {expectedVersion} | Actual version: {actualVersion}";
    }
}
