using DiscordBot.Services.Executor;
using System;
using System.Threading.Tasks;

namespace Sanakan.Services.Executor
{
    public interface IExecutable
    {
        string GetName();
        Priority GetPriority();
        Task<Task<bool>> ExecuteAsync(IServiceProvider provider);
    }
}
