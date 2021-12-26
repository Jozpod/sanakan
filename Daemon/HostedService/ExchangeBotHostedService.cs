using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sanakan.Common;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Abstractions.Configuration;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Daemon.HostedService
{
    [ExcludeFromCodeCoverage]
    internal class ExchangeBotHostedService : BackgroundService
    {
        private readonly IDiscordClientAccessor _discordClientAccessor;
        private readonly IIconConfiguration _iconConfiguration;
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ITaskManager _taskManager;
        private ulong _botUserId;
        private bool _reAddReaction;

        public ExchangeBotHostedService(
            IDiscordClientAccessor discordClientAccessor,
            IIconConfiguration iconConfiguration,
            IRandomNumberGenerator randomNumberGenerator,
            IServiceScopeFactory serviceScopeFactory,
            ITaskManager taskManager)
        {
            _discordClientAccessor = discordClientAccessor;
            _iconConfiguration = iconConfiguration;
            _randomNumberGenerator = randomNumberGenerator;
            _serviceScopeFactory = serviceScopeFactory;
            _taskManager = taskManager;
            _discordClientAccessor.GuildAvailable += GuildAvailable;
        }

        private Task GuildAvailable(IGuild _)
        {
            _discordClientAccessor.MessageReceived += HandleMessageAsync;
            _discordClientAccessor.ReactionAdded += ReactionAddedAsync;
            var client = _discordClientAccessor.Client;
            _botUserId = client.CurrentUser.Id;
            return Task.CompletedTask;
        }

        private async Task ReactionAddedAsync(Cacheable<IUserMessage, ulong> cachedMessage, ISocketMessageChannel messageChannel, IReaction reaction)
        {
            var message = cachedMessage.Value;

            if (message.Author.Id != _botUserId)
            {
                return;
            }

            var embeds = message.Embeds;

            if (!embeds.Any())
            {
                return;
            }

            var embed = embeds.First();

            if (!embed.Description.Contains("Wymiana"))
            {
                return;
            }

            var reactionUsers = await message.GetReactionUsersAsync(_iconConfiguration.Accept, 10)
                .FlattenAsync();

            if(!reactionUsers.Any(pr => pr.Id != _botUserId))
            {
                return;
            }

            if(_reAddReaction)
            {
                _reAddReaction = false;
                return;
            }

            _reAddReaction = true;
            await message.RemoveReactionAsync(_iconConfiguration.Accept, _botUserId);
            await message.AddReactionAsync(_iconConfiguration.Accept);
        }

        private async Task HandleMessageAsync(IMessage message)
        {
            if (message.Author.Id != _botUserId)
            {
                return;
            }

            var embeds = message.Embeds;

            if (!embeds.Any())
            {
                return;
            }

            var embed = embeds.First();

            if (string.IsNullOrEmpty(embed.Description))
            {
                return;
            }

            if (!embed.Description.Contains("Wymiana"))
            {
                return;
            }

            using var serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var userRepository = serviceProvider.GetRequiredService<IUserRepository>();

            var botUser = await userRepository.GetCachedFullUserAsync(_botUserId);

            var randomCard = _randomNumberGenerator.GetOneRandomFrom(botUser.GameDeck.Cards);

            await message.Channel.SendMessageAsync($"dodaj {randomCard.Id}");

            await _taskManager.Delay(TimeSpan.FromSeconds(2));

            await message.AddReactionAsync(_iconConfiguration.TwoEmote);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await _taskManager.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                
            }
        }
    }
}
