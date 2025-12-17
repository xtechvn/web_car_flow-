using System.Threading.Channels;
using XTECH_FRONTEND.Model;
namespace XTECH_FRONTEND.Services.BackgroundQueue
{
    public interface IInsertQueue
    {
        bool Enqueue(InsertJob job);
        ValueTask<InsertJob> DequeueAsync(CancellationToken cancellationToken);
    }
}
