﻿using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.DAL.Models.Analytics;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Sanakan.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using System.Collections.Concurrent;
using Sanakan.DiscordBot;
using Sanakan.Common.Configuration;
using System.Collections.Generic;
using Discord.WebSocket;
using System.Linq;
using Discord;
using Sanakan.TaskQueue;
using Sanakan.DiscordBot.Abstractions;
using Sanakan.Services.PocketWaifu;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.Extensions;
using Sanakan.ShindenApi.Models;
using Sanakan.TaskQueue.Messages;
using Sanakan.Game.Services.Abstractions;

namespace Sanakan.Web.HostedService
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
        private readonly object _syncRoot = new object();

        private IDictionary<ulong, Entry> ServerCounter;
        private IDictionary<ulong, ulong> UserCounter;

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

            ServerCounter = new Dictionary<ulong, Entry>();
            UserCounter = new Dictionary<ulong, ulong>();
        }

        internal class Entry
        {
            public ulong EventCount { get; set; }
            public DateTime ResetOn { get; set; }
        }

        private Task LoggedIn()
        {
            _discordSocketClientAccessor.MessageReceived += HandleMessageAsync;
            return Task.CompletedTask;
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

        private void OnResetCounter(object sender, TimerEventArgs e)
        {
            var utcNow = _systemClock.UtcNow;
            var resetInterval = TimeSpan.FromDays(1);

            lock (_syncRoot)
            {
                foreach (var entry in ServerCounter.Values)
                {
                    if (utcNow - entry.ResetOn > resetInterval)
                    {
                        entry.EventCount = 0;
                        entry.ResetOn = utcNow;
                    }
                }
            }
        }

        private async Task HandleGuildAsync(
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

            var guildId = spawnChannel.GuildId;
            var effectiveLimit = 0ul;

            lock (_syncRoot)
            {
                if (ServerCounter.TryGetValue(guildId, out var limit))
                {
                    effectiveLimit = limit.EventCount;
                }
                else
                {
                    ServerCounter[guildId] = new Entry
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

                ServerCounter[guildId].EventCount = effectiveLimit + 1;
            }
            
            await SpawnCardAsync(spawnChannel, trashChannel, mention, muteRole);
        }

        //private Executable GetSafariExe(
        //    EmbedBuilder embed,
        //    IUserMessage msg,
        //    Card newCard,
        //    SafariImage pokeImage,
        //    CharacterInfo character,
        //    ITextChannel trashChannel,
        //    IUser winner)
        //{
        //    return new Executable("safari", new Task<Task>(async () =>
        //    {
                
        //    }));
        //}

        private async Task SpawnCardAsync(
            ITextChannel spawnChannel,
            ITextChannel trashChannel,
            string mention,
            IRole muteRole)
        {
            var character = await _waifuService.GetRandomCharacterAsync();

            if (character == null)
            {
                _logger.LogInformation("In safari: bad shinden connection");
                return;
            }

            var newCard = _waifuService.GenerateNewCard(null, character);
            newCard.Source = CardSource.Safari;
            newCard.Affection -= 1.8;
            newCard.InCage = true;

            var pokeImage = await _waifuService.GetRandomSarafiImage();
            var time = _systemClock.UtcNow.AddMinutes(5);
            var embed = new EmbedBuilder
            {
                Color = EMType.Bot.Color(),
                Description = $"**Polowanie zakończy się o**: `{time.ToShortTimeString()}:{time.Second.ToString("00")}`",
                ImageUrl = await _waifuService.GetSafariViewAsync(pokeImage, trashChannel)
            };

            var message = await spawnChannel.SendMessageAsync(mention, embed: embed.Build());
            //RunSafari(embed, message, newCard, pokeImage, character, trashChannel, muteRole);

            try
            {
                await _taskManager.Delay(TimeSpan.FromMinutes(5));

                var usersReacted = await message.GetReactionUsersAsync(Emojis.RaisedHand, 300).FlattenAsync();
                var users = usersReacted.ToList();

                IUser? winner = null;

                var cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.CancelAfter(TimeSpan.FromMinutes(1));
                var cancellationToken = cancellationTokenSource.Token;

                await Task.Run(async () =>
                {
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

                        using var serviceScope = _serviceScopeFactory.CreateScope();
                        var serviceProvider = serviceScope.ServiceProvider;
                        var userRepository = serviceProvider.GetRequiredService<IUserRepository>();
                        var databaseUser = await userRepository.GetCachedFullUserAsync(selected.Id);
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
                }, cancellationToken);

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
                    Image = pokeImage,
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

        private void HandleUserAsync(IUserMessage message)
        {
            var charactersNeeded = _experienceConfiguration.CurrentValue.CharPerPacket;
            if (charactersNeeded <= 0)
            {
                return;
            }

            var author = message.Author;
            var messageLength = GetMessageRealLength(message);

            if (UserCounter.TryGetValue(author.Id, out var charactersTotal))
            {
                charactersTotal += messageLength;
            }
            else
            {
                charactersTotal = messageLength;
            }

            UserCounter[author.Id] = charactersTotal;

            if (charactersTotal > charactersNeeded)
            {
                UserCounter[author.Id] = 0;

                var guildUser = author as IGuildUser;
                
                _blockingPriorityQueue.TryEnqueue(new SpawnCardBundleMessage
                {
                    GuildId = guildUser?.Id,
                    Mention = author.Mention,
                    DiscordUserId = author.Id,
                    MessageChannel = message.Channel,
                });
            }
        }

        private ulong GetMessageRealLength(IUserMessage message)
        {
            if (string.IsNullOrEmpty(message.Content))
            {
                return 1;
            }

            var emoteChars = (ulong)message.Tags.CountEmotesTextLength();
            var linkChars = (ulong)message.Content.CountLinkTextLength();
            var nonWhiteSpaceChars = (ulong)message.Content.Count(c => c != ' ');
            var quotedChars = (ulong)message.Content.CountQuotedTextLength();
            var charsThatMatters = nonWhiteSpaceChars - linkChars - emoteChars - quotedChars;

            return charsThatMatters < 1ul ? 1ul : charsThatMatters;
        }

        private async Task HandleMessageAsync(IMessage message)
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
            };

            var user = userMessage.Author as IGuildUser;

            if (user == null)
            {
                return;
            };

            var guildId = user.Guild.Id;

            if (_discordConfiguration.CurrentValue.BlacklistedGuilds.Any(x => x == guildId))
            {
                return;
            }

            using var serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var guildConfigRepository = serviceProvider.GetRequiredService<IGuildConfigRepository>();
            var guildConfig = await guildConfigRepository.GetCachedGuildFullConfigAsync(guildId);

            if (guildConfig == null)
            {
                return;
            }

            var noExperience = guildConfig.ChannelsWithoutExperience.Any(x => x.Channel == userMessage.Channel.Id);

            if (!noExperience)
            {
                HandleUserAsync(userMessage);
            }

            var guild = user.Guild;
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
                spawnChannel,
                trashSpawnChannel,
                guildConfig.SafariLimit,
                mention,
                noExperience,
                muteRole);
        }
    }
}
