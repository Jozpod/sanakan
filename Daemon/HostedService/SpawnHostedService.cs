using Discord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Abstractions;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.Extensions;
using Sanakan.Game.Services.Abstractions;
using Sanakan.TaskQueue;
using Sanakan.TaskQueue.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Daemon.HostedService
{
    internal class SpawnHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly ISystemClock _systemClock;
        private readonly IDiscordClientAccessor _discordSocketClientAccessor;
        private readonly IOptionsMonitor<DiscordConfiguration> _discordConfiguration;
        private readonly IOptionsMonitor<ExperienceConfiguration> _experienceConfiguration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly IBlockingPriorityQueue _blockingPriorityQueue;
        private readonly ITaskManager _taskManager;
        private readonly IWaifuService _waifuService;
        private readonly ITimer _timer;
        private readonly object _syncRoot = new();
        private readonly IDictionary<ulong, Entry> _serverCounter;
        private readonly IDictionary<ulong, ulong> _userCounter;
        private bool _isRunning;

        public SpawnHostedService(
            IRandomNumberGenerator randomNumberGenerator,
            IBlockingPriorityQueue blockingPriorityQueue,
            ILogger<SpawnHostedService> logger,
            IOptionsMonitor<DiscordConfiguration> discordConfiguration,
            IOptionsMonitor<ExperienceConfiguration> experienceConfiguration,
            IDiscordClientAccessor discordSocketClientAccessor,
            ISystemClock systemClock,
            IServiceScopeFactory serviceScopeFactory,
            ITaskManager taskManager,
            IWaifuService waifuService,
            ITimer timer)
        {
            _randomNumberGenerator = randomNumberGenerator;
            _blockingPriorityQueue = blockingPriorityQueue;
            _logger = logger;
            _systemClock = systemClock;
            _serviceScopeFactory = serviceScopeFactory;
            _discordConfiguration = discordConfiguration;
            _experienceConfiguration = experienceConfiguration;
            _discordSocketClientAccessor = discordSocketClientAccessor;
            _taskManager = taskManager;
            _waifuService = waifuService;
            _timer = timer;
            _discordSocketClientAccessor.LoggedIn += LoggedIn;

            _serverCounter = new Dictionary<ulong, Entry>();
            _userCounter = new Dictionary<ulong, ulong>();
        }

        internal void OnResetCounter(object sender, TimerEventArgs e)
        {
            if (_isRunning)
            {
                return;
            }

            _isRunning = true;

            var utcNow = _systemClock.UtcNow;
            var resetInterval = TimeSpan.FromDays(1);

            lock (_syncRoot)
            {
                foreach (var entry in _serverCounter.Values)
                {
                    if (utcNow - entry.ResetOn > resetInterval)
                    {
                        entry.EventCount = 0;
                        entry.ResetOn = utcNow;
                    }
                }
            }

            _isRunning = false;
        }

        internal async Task HandleMessageAsync(IMessage message)
        {
            var userMessage = message as IUserMessage;

            if (userMessage == null)
            {
                return;
            }

            var author = userMessage.Author;

            if (author.IsBotOrWebhook())
            {
                return;
            }

            var user = userMessage.Author as IGuildUser;

            if (user == null)
            {
                return;
            }

            var guildId = user.Guild.Id;
            var guild = user.Guild;

            if (_discordConfiguration.CurrentValue.BlacklistedGuilds.Contains(guildId))
            {
                return;
            }

            using var serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var guildConfigRepository = serviceProvider.GetRequiredService<IGuildConfigRepository>();
            var guildConfig = await guildConfigRepository.GetCachedById(guildId);

            if (guildConfig == null)
            {
                return;
            }

            var noExperience = guildConfig.ChannelsWithoutExperience.Any(x => x.ChannelId == userMessage.Channel.Id);

            if (!noExperience)
            {
                HandleUserAsync(userMessage);
            }

            if (guildConfig.WaifuConfig == null)
            {
                return;
            }

            var spawnChannelId = guildConfig.WaifuConfig.SpawnChannelId;
            var trashSpawnChannelId = guildConfig.WaifuConfig.TrashSpawnChannelId;

            if (!spawnChannelId.HasValue || !trashSpawnChannelId.HasValue)
            {
                return;
            }

            var spawnChannel = await guild.GetTextChannelAsync(spawnChannelId.Value);
            var trashSpawnChannel = await guild.GetTextChannelAsync(trashSpawnChannelId.Value);

            var mention = "";
            var waifuRoleId = guildConfig.WaifuRoleId;

            if (waifuRoleId.HasValue)
            {
                var waifuRole = guild.GetRole(waifuRoleId.Value);
                mention = waifuRole.Mention;
            }

            var muteRole = guild.GetRole(guildConfig.MuteRoleId);

            await HandleGuildAsync(
                guildId,
                spawnChannel,
                trashSpawnChannel,
                guildConfig.SafariLimit,
                mention,
                noExperience,
                muteRole);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _timer.Tick += OnResetCounter;
                _timer.Start(TimeSpan.Zero, TimeSpan.FromHours(1));
                await _taskManager.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private async Task SpawnCardAsync(
            ITextChannel spawnChannel,
            ITextChannel trashChannel,
            string mention,
            IRole? muteRole)
        {
            var character = await _waifuService.GetRandomCharacterAsync();

            if (character == null)
            {
                _logger.LogInformation("Could not retrieve safari image from Shinden.");
                return;
            }

            var newCard = _waifuService.GenerateNewCard(null, character);
            newCard.Source = CardSource.Safari;
            newCard.Affection -= 1.8;
            newCard.InCage = true;

            var pokeImage = await _waifuService.GetRandomSarafiImage();
            var time = _systemClock.UtcNow.AddMinutes(5);
            var description = $"**Polowanie zakończy się o**: `{time.ToShortTimeString()}:{time.Second:00}`";
            var imageUrl = await _waifuService.SendAndGetSafariImageUrlAsync(pokeImage!, trashChannel);

            var embed = new EmbedBuilder
            {
                Color = EMType.Bot.Color(),
                Description = description,
                ImageUrl = imageUrl,
            };

            var message = await spawnChannel.SendMessageAsync(mention, embed: embed.Build());

            try
            {
                await _taskManager.Delay(TimeSpan.FromMinutes(5));

                var usersReacted = await message.GetReactionUsersAsync(Emojis.RaisedHand, 300).FlattenAsync();
                var users = usersReacted.ToList();

                IUser? winner = null;

                var cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.CancelAfter(TimeSpan.FromMinutes(1));
                var cancellationToken = cancellationTokenSource.Token;

                await Task.Run(
                    async () =>
                    {
                        using var serviceScope = _serviceScopeFactory.CreateScope();
                        var serviceProvider = serviceScope.ServiceProvider;
                        var userRepository = serviceProvider.GetRequiredService<IUserRepository>();

                        while (winner == null)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            if (!users.Any())
                            {
                                embed.Description = $"Na polowanie nie stawił się żaden łowca!";
                                await message.ModifyAsync(x => x.Embed = embed.Build());
                                return;
                            }

                            var isUserMuted = false;
                            var selected = _randomNumberGenerator.GetOneRandomFrom(users);

                            if (muteRole != null && selected is IGuildUser guildUser)
                            {
                                if (guildUser.RoleIds.Any(id => id == muteRole.Id))
                                {
                                    isUserMuted = true;
                                }
                            }

                            var databaseUser = await userRepository.GetCachedAsync(selected.Id);
                            var gameDeck = databaseUser.GameDeck;

                            if (databaseUser != null && !isUserMuted)
                            {
                                if (!databaseUser.IsBlacklisted
                                    && gameDeck.MaxNumberOfCards > gameDeck.Cards.Count)
                                {
                                    winner = selected;
                                }
                            }

                            users.Remove(selected);
                        }
                    },
                    cancellationToken);

                await message.RemoveAllReactionsAsync();

                if (winner == null)
                {
                    return;
                }

                _blockingPriorityQueue.TryEnqueue(new SafariMessage
                {
                    Winner = winner,
                    Embed = embed,
                    Card = newCard,
                    Message = message,
                    TrashChannel = trashChannel,
                    Image = pokeImage!,
                    GuildId = trashChannel?.Guild?.Id,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while running Safari", ex);
                await message.ModifyAsync(x => x.Embed = "Karta uciekła!".ToEmbedMessage(EMType.Error).Build());
                await message.RemoveAllReactionsAsync();
            }

            await message.AddReactionAsync(Emojis.RaisedHand);
        }

        private void HandleUserAsync(IMessage message)
        {
            var charactersNeeded = _experienceConfiguration.CurrentValue.CharPerPacket;
            if (charactersNeeded <= 0)
            {
                return;
            }

            var user = message.Author;
            var userId = user.Id;
            var messageLength = GetMessageRealLength(message);

            if (_userCounter.TryGetValue(userId, out var charactersTotal))
            {
                charactersTotal += messageLength;
            }
            else
            {
                charactersTotal = messageLength;
            }

            _userCounter[userId] = charactersTotal;

            if (charactersTotal > charactersNeeded)
            {
                _userCounter[userId] = 0;

                var guildUser = user as IGuildUser;

                var enqueued = _blockingPriorityQueue.TryEnqueue(new SpawnCardBundleMessage
                {
                    GuildId = guildUser?.Guild?.Id,
                    Mention = user.Mention,
                    DiscordUserId = userId,
                    MessageChannel = message.Channel,
                });

                if (!enqueued)
                {
                    _logger.LogWarning("Could not enqueue message {0}", nameof(SpawnCardBundleMessage));
                }
            }
        }

        private ulong GetMessageRealLength(IMessage message)
        {
            if (string.IsNullOrEmpty(message.Content))
            {
                return 1;
            }

            var content = message.Content;
            var emoteChars = (ulong)message.Tags.CountEmotesTextLength();
            var linkChars = (ulong)content.CountLinkTextLength();
            var nonWhiteSpaceChars = (ulong)content.Count(c => c != ' ');
            var quotedChars = (ulong)content.CountQuotedTextLength();
            var charsThatMatters = nonWhiteSpaceChars - linkChars - emoteChars - quotedChars;

            return charsThatMatters < 1ul ? 1ul : charsThatMatters;
        }

        private async Task HandleGuildAsync(
            ulong guildId,
            ITextChannel spawnChannel,
            ITextChannel trashChannel,
            ulong dailyLimit,
            string mention,
            bool noExperience,
            IRole muteRole)
        {
            if (!_discordConfiguration.CurrentValue.SafariEnabled)
            {
                return;
            }

            var effectiveLimit = 0ul;

            lock (_syncRoot)
            {
                if (_serverCounter.TryGetValue(guildId, out var limit))
                {
                    effectiveLimit = limit.EventCount;
                }
                else
                {
                    _serverCounter[guildId] = new Entry
                    {
                        ResetOn = _systemClock.UtcNow,
                        EventCount = 0,
                    };
                }

                var chance = noExperience ? 285 : 85;

                if (effectiveLimit > 0 && effectiveLimit >= dailyLimit)
                {
                    return;
                }

                if (!_randomNumberGenerator.TakeATry(chance))
                {
                    return;
                }

                _serverCounter[guildId].EventCount = effectiveLimit + 1;
            }

            await SpawnCardAsync(spawnChannel, trashChannel, mention, muteRole);
        }

        private Task LoggedIn()
        {
            _discordSocketClientAccessor.MessageReceived += HandleMessageAsync;
            return Task.CompletedTask;
        }

        private class Entry
        {
            public ulong EventCount { get; set; }

            public DateTime ResetOn { get; set; }
        }
    }
}
