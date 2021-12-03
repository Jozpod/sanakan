using Sanakan.TaskQueue.Messages;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.MessageHandlers
{
    internal abstract class BaseMessageHandler<T> : IMessageHandler<T>
        where T : BaseMessage
    {
        public Task HandleAsync(BaseMessage baseMessage) => HandleAsync((T)baseMessage);

        public abstract Task HandleAsync(T baseMessage);
    }
}
