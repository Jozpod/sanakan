using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Common
{
    internal class TaskManager : ITaskManager
    {
        public Task Delay(TimeSpan delay) => Task.Delay(delay);

        public Task Delay(TimeSpan delay, CancellationToken cancellationToken) => Task.Delay(delay, cancellationToken);
    }
}
