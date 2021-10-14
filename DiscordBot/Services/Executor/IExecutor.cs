using System;
using System.Threading.Tasks;

namespace Sanakan.Services.Executor
{
    public interface IExecutor
    {
        Task RunWorker();
        Task<bool> TryAdd(IExecutable task, TimeSpan timeout);
    }
}
