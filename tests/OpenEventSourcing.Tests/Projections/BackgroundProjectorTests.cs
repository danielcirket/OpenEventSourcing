using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace OpenEventSourcing.Tests.Projections
{
    public class BackgroundProjectorTests
    {
        [Fact]
        public void WhenStartedThenShouldReturnCompletedTaskIfLongRunningTaskIsIncomplete()
        {
            var tcs = new TaskCompletionSource<object>();
            var service = new TestProjector(tcs.Task);
            var task = service.StartAsync(CancellationToken.None);

            task.IsCompleted.Should().BeTrue();
            tcs.Task.IsCompleted.Should().BeFalse();

            // Complete the tcs task
            tcs.TrySetResult(null);
        }
        [Fact]
        public void WhenCancelledThenStartShouldReturnCompletedTask()
        {
            var tcs = new TaskCompletionSource<object>();

            tcs.TrySetCanceled();

            var service = new TestProjector(tcs.Task);
            var task = service.StartAsync(CancellationToken.None);

            task.IsCompleted.Should().BeTrue();
        }
        [Fact]
        public void WhenFailedThenStartShouldReturnLongRunningTask()
        {
            var tcs = new TaskCompletionSource<object>();

            tcs.TrySetException(new Exception("Failure!"));

            var service = new TestProjector(tcs.Task);

            Func<Task> act = async () => await service.StartAsync(CancellationToken.None);

            act.Should().Throw<Exception>()
               .And.Message.Should().Be("Failure!");
        }
        [Fact]
        public async Task WhenStopCalledWithoutStartingThenShouldNoop()
        {
            var tcs = new TaskCompletionSource<object>();
            var service = new TestProjector(tcs.Task);

            await service.StopAsync(CancellationToken.None);
        }
        [Fact]
        public async Task WhenStopCalledThenShouldAlsoStopBackgroundService()
        {
            var tcs = new TaskCompletionSource<object>();
            var service = new TestProjector(tcs.Task);

            await service.StartAsync(CancellationToken.None);

            service.ExecutingTask.IsCompleted.Should().BeFalse();

            await service.StopAsync(CancellationToken.None);

            service.ExecutingTask.IsCompleted.Should().BeTrue();
        }
        [Fact]
        public async Task WhenStoppedThenShouldStopEventIfTaskNeverEnds()
        {
            var service = new CancellationIgnoringProjector();

            await service.StartAsync(CancellationToken.None);

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));

            await service.StopAsync(cts.Token);
        }
        [Fact]
        public async Task WhenCancellationCallbackThrowsThenStopShouldThrow()
        {
            var service = new ThrowingProjector();

            await service.StartAsync(CancellationToken.None);

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));

            Func<Task> act = async () => await service.StopAsync(cts.Token);

            act.Should().Throw<AggregateException>();
            service.Calls.Should().Be(2);
        }
        [Fact]
        public async Task WhenStartedAndThenDisposedThenShouldTriggerCancellationToken()
        {
            var service = new WaitOnCancellationProjector();

            await service.StartAsync(CancellationToken.None);

            service.Dispose();
        }
    }
}
