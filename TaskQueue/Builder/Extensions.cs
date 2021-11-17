using Microsoft.Extensions.DependencyInjection;
using Sanakan.TaskQueue.MessageHandlers;
using Sanakan.TaskQueue.Messages;
using System;
using System.Collections.Concurrent;

namespace Sanakan.TaskQueue.Builder
{
    public static class Extensions
    {
        public static IServiceCollection AddTaskQueue(this IServiceCollection services)
        {
            services.AddSingleton<IBlockingPriorityQueue, BlockingPriorityQueue>();
            services.AddScoped<IMessageHandler<LotteryMessage>, LotteryMessageHandler>();
            services.AddScoped<IMessageHandler<DeleteUserMessage>, DeleteUserMessageHandler>();
            services.AddScoped<IMessageHandler<ConnectUserMessage>, ConnectUserMessageHandler>();
            services.AddScoped<IMessageHandler<ReplaceCharacterIdsInCardMessage>, ReplaceCharacterIdsInCardMessageHandler>();
            services.AddScoped<IMessageHandler<ToggleCardMessage>, ToggleCardMessageHandler>();
            services.AddScoped<IMessageHandler<SpawnCardBundleMessage>, SpawnCardBundleMessageHandler>();
            services.AddScoped<IMessageHandler<UpdateCardMessage>, UpdateCardMessageHandler>();
            services.AddScoped<IMessageHandler<UpdateCardPictureMessage>, UpdateCardPictureMessageHandler>();
            services.AddScoped<IMessageHandler<AddExperienceMessage>, AddExperienceMessageHandler>();
            services.AddScoped<IMessageHandler<SafariMessage>, SafariMessageHandler>();
            services.AddScoped<IMessageHandler<GiveCardsMessage>, GiveCardsMessageHandler>();
            
            return services;
        }
    }
}
