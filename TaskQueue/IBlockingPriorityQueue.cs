using Sanakan.TaskQueue.Messages;
using System.Collections.Generic;
using System.Threading;

namespace Sanakan.TaskQueue
{
    public interface IBlockingPriorityQueue
    {
        bool TryEnqueue(BaseMessage message);

        IAsyncEnumerable<BaseMessage> GetAsyncEnumerable(CancellationToken token = default);
    }
}
