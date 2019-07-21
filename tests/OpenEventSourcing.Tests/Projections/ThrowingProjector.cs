using System;
using System.Threading;
using System.Threading.Tasks;
using OpenEventSourcing.Projections;

namespace OpenEventSourcing.Tests.Projections
{
    internal class ThrowingProjector : BackgroundProjector<NoOpProjection>
    {
        public int Calls { get; set; }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            cancellationToken.Register(() =>
            {
                Calls++;
                throw new InvalidOperationException();
            });

            cancellationToken.Register(() =>
            {
                Calls++;
            });

            return new TaskCompletionSource<object>().Task;
        }
    }
}
