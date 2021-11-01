using Sanakan.TaskQueue.Messages;
using System.Collections.Generic;
using System.Threading;

namespace Sanakan.TaskQueue
{
    public interface IBlockingPriorityQueue
    {
        bool TryEnqueue(BaseMessage message);
        IEnumerable<BaseMessage> GetEnumerable(CancellationToken token = default);
    }
}
