using Microsoft.Extensions.DependencyInjection;
using Sanakan.TaskQueue.Messages;
using System;
using System.Collections.Concurrent;

namespace Sanakan.TaskQueue.Builder
{
    public static class Extensions
    {
        public static IServiceCollection AddTaskQueue(this IServiceCollection services)
        {
            services.AddSingleton<IProducerConsumerCollection<BaseMessage>, BlockingPriorityQueue>();
            return services;
        }
    }
}
