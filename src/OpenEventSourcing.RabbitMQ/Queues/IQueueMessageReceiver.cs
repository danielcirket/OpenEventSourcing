using System;
using System.Threading;
using System.Threading.Tasks;
using OpenEventSourcing.RabbitMQ.Messages;

namespace OpenEventSourcing.RabbitMQ.Queues
{
    public interface IQueueMessageReceiver
    {
        Task StartAsync(CancellationToken cancellationToken = default);
        Task StopAsync(CancellationToken cancellationToken = default);
        Task OnReceiveAsync(ReceivedMessage message, CancellationToken cancellationToken);
        Task OnErrorAsync(ReceivedMessage message, Exception ex);
    }
}
