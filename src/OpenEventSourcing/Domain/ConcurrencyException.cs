using System;

namespace OpenEventSourcing.Domain
{
    public class ConcurrencyException : Exception
    {
        public ConcurrencyException(string subject, long expectedVersion, long actualVersion)
            : base(BuildErrorMessage(subject, expectedVersion, actualVersion)) { }

        private static string BuildErrorMessage(string subject, long expectedVersion, long actualVersion)
            => $"Concurrency exception | Aggregate: '{subject}' | Expected version: '{expectedVersion}' | Actual version: '{actualVersion}'";
    }
}
