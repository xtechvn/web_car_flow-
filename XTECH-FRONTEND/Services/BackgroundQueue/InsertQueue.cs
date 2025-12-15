using System.Threading.Channels;
using XTECH_FRONTEND.Model;
namespace XTECH_FRONTEND.Services.BackgroundQueue
{
    public class InsertQueue : IInsertQueue
    {
        private readonly Channel<InsertJob> _queue;

        public InsertQueue()
        {
            _queue = Channel.CreateUnbounded<InsertJob>();
        }

        public bool Enqueue(InsertJob job)
        {
            return _queue.Writer.TryWrite(job);
        }

        public ValueTask<InsertJob> DequeueAsync(CancellationToken cancellationToken)
            => _queue.Reader.ReadAsync(cancellationToken);
    }
}
