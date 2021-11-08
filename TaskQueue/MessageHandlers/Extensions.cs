using Microsoft.Extensions.DependencyInjection;
using Sanakan.TaskQueue.Messages;
using System;

namespace Sanakan.TaskQueue.MessageHandlers
{
    public static class Extensions
    {
        public static IMessageHandler<T> GetMessageHandler<T>(this IServiceProvider serviceProvider, T _)
           where T : BaseMessage
        {
            return serviceProvider.GetRequiredService<IMessageHandler<T>>();
        }
    }
}
