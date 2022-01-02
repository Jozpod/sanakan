using Sanakan.TaskQueue.Messages;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.MessageHandlers
{
    public interface IMessageHandler
    {
        Task HandleAsync(BaseMessage message);
    }

    public interface IMessageHandler<in T> : IMessageHandler
        where T : BaseMessage
    {
        Task HandleAsync(T message);
    }
}
