using Microsoft.Extensions.DependencyInjection;
using Sanakan.TaskQueue.Messages;
using System;

namespace Sanakan.TaskQueue.MessageHandlers
{
    public static class Extensions
    {
        public static void GetMessageHandler<T>(this IServiceProvider serviceProvider, T _)
           where T : BaseMessage
        {
            serviceProvider.GetRequiredService<IMessageHandler<T>>();
        }
    }
}
