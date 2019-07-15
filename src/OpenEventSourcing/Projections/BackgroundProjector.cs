using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenEventSourcing.Projections
{
    public abstract class BackgroundProjector<TProjection> : IProjector<TProjection>, IDisposable
        where TProjection : IProjection
    {
        private Task _executingTask;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            _executingTask = ExecuteAsync(_stoppingCts.Token);

            if (_executingTask.IsCompleted)
                return _executingTask;

            return Task.CompletedTask;
        }
        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executingTask == null)
                return;

            try
            {
                _stoppingCts.Cancel();
            }
            finally
            {
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }
        public void Dispose()
        {
            _stoppingCts.Cancel();
        }

        protected abstract Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
