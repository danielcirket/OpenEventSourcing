using System;

namespace OpenEventSourcing.Domain
{
    public class ConcurrencyException : Exception
    {
        public ConcurrencyException(string streamId, long expectedVersion, long actualVersion)
            : base(BuildErrorMessage(streamId, expectedVersion, actualVersion)) { }

        private static string BuildErrorMessage(string streamId, long expectedVersion, long actualVersion)
            => $"Concurrency exception | Stream: '{streamId}' | Expected version: {expectedVersion} | Actual version: {actualVersion}";
    }
}
