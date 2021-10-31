using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
