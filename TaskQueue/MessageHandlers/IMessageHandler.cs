using Sanakan.Web.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.Web.MessageHandlers
{
    public interface IMessageHandler<T>
        where T: BaseMessage
    {
        Task HandleAsync(T message);
    }
}
