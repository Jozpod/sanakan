using Sanakan.TaskQueue.Messages;
using System.Threading.Tasks;

namespace Sanakan.Web.MessageHandlers
{
    public interface IMessageHandler<T>
        where T: BaseMessage
    {
        Task HandleAsync(T message);
    }
}
