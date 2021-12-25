using Microsoft.Extensions.DependencyInjection;
using Sanakan.TaskQueue.MessageHandlers;
using Sanakan.TaskQueue.Messages;
using System.Diagnostics.CodeAnalysis;

namespace Sanakan.TaskQueue.Builder
{
    [ExcludeFromCodeCoverage]
    public static class Extensions
    {
        public static IServiceCollection AddTaskQueue(this IServiceCollection services)
        {
            services.AddSingleton<IBlockingPriorityQueue, BlockingPriorityQueue>();
            services.AddScoped<IMessageHandler<SessionMessage>, SessionMessageHandler>();
            services.AddScoped<IMessageHandler<AddExperienceMessage>, AddExperienceMessageHandler>();
            services.AddScoped<IMessageHandler<ConnectUserMessage>, ConnectUserMessageHandler>();
            services.AddScoped<IMessageHandler<CommandMessage>, CommandMessageHandler>();
            services.AddScoped<IMessageHandler<DeleteUserMessage>, DeleteUserMessageHandler>();
            services.AddScoped<IMessageHandler<GiveCardsMessage>, GiveCardsMessageHandler>();
            services.AddScoped<IMessageHandler<GiveBoosterPackMessage>, GiveBoosterPackMessageHandler>();
            services.AddScoped<IMessageHandler<LotteryMessage>, LotteryMessageHandler>();
            services.AddScoped<IMessageHandler<ReplaceCharacterIdsInCardMessage>, ReplaceCharacterIdsInCardMessageHandler>();
            services.AddScoped<IMessageHandler<SafariMessage>, SafariMessageHandler>();
            services.AddScoped<IMessageHandler<SpawnCardBundleMessage>, SpawnCardBundleMessageHandler>();
            services.AddScoped<IMessageHandler<ToggleCardMessage>, ToggleCardMessageHandler>();
            services.AddScoped<IMessageHandler<TransferTCMessage>, TransferTCMessageHandler>();
            services.AddScoped<IMessageHandler<UpdateCardMessage>, UpdateCardMessageHandler>();
            services.AddScoped<IMessageHandler<UpdateCardPictureMessage>, UpdateCardPictureMessageHandler>();

            return services;
        }
    }
}
