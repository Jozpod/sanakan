using Microsoft.Extensions.Hosting;
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
using DiscordBot.Services.PocketWaifu.Abstractions;
using Sanakan.DiscordBot.Abstractions;
using Sanakan.Services.PocketWaifu;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.Extensions;
using Sanakan.ShindenApi.Models;
using Sanakan.TaskQueue.Messages;

namespace Sanakan.Web.HostedService
{
    public class SpawnHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly ISystemClock _systemClock;
        private readonly IDiscordSocketClientAccessor _discordSocketClientAccessor;
        private readonly IOptionsMonitor<DiscordConfiguration> _discordConfiguration;
        private readonly IOptionsMonitor<ExperienceConfiguration> _experienceConfiguration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IGuildConfigRepository _guildConfigRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly IBlockingPriorityQueue _blockingPriorityQueue;
        private readonly ITimer _timer;
        private readonly ITaskManager _taskManager;
        private readonly IWaifuService _waifu;

        private Dictionary<ulong, long> ServerCounter;
        private Dictionary<ulong, long> UserCounter;

        public SpawnHostedService(
            IGuildConfigRepository guildConfigRepository,
            IUserRepository userRepository,
            IRandomNumberGenerator randomNumberGenerator,
            IBlockingPriorityQueue blockingPriorityQueue,
            ILogger<SpawnHostedService> logger,
            IOptionsMonitor<DiscordConfiguration> discordConfiguration,
            IOptionsMonitor<ExperienceConfiguration> experienceConfiguration,
            IDiscordSocketClientAccessor discordSocketClientAccessor,
            ISystemClock systemClock,
            IServiceScopeFactory serviceScopeFactory,
            ITaskManager taskManager)
        {
            _guildConfigRepository = guildConfigRepository;
            _userRepository = userRepository;
            _randomNumberGenerator = randomNumberGenerator;
            _blockingPriorityQueue = blockingPriorityQueue;
            _logger = logger;
            _systemClock = systemClock;
            _serviceScopeFactory = serviceScopeFactory;
            _discordConfiguration = discordConfiguration;
            _experienceConfiguration = experienceConfiguration;
            _discordSocketClientAccessor = discordSocketClientAccessor;
            _taskManager = taskManager;
            _discordSocketClientAccessor.Initialized += OnInitialized;

            ServerCounter = new Dictionary<ulong, long>();
            UserCounter = new Dictionary<ulong, long>();
        }

        private Task OnInitialized()
        {
            _discordSocketClientAccessor.Client.MessageReceived += HandleMessageAsync;
            return Task.CompletedTask;
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

        private async void HandleGuildAsync(
            ITextChannel spawnChannel,
            ITextChannel trashChannel,
            long daily,
            string mention,
            bool noExp,
            SocketRole muteRole)
        {
            if (!ServerCounter.Any(x => x.Key == spawnChannel.GuildId))
            {
                ServerCounter.Add(spawnChannel.GuildId, 0);
                return;
            }

            if (ServerCounter[spawnChannel.GuildId] == 0)
            {
                await _taskManager.Delay(TimeSpan.FromDays(1));
                ServerCounter[spawnChannel.GuildId] = 0;
            }

            int chance = noExp ? 285 : 85;

            if (daily > 0 && ServerCounter[spawnChannel.GuildId] >= daily)
            {
                return;
            }

            if (!_discordConfiguration.CurrentValue.SafariEnabled)
            {
                return;
            }

            if (!_randomNumberGenerator.TakeATry(chance))
            {
                return;
            }

            ServerCounter[spawnChannel.GuildId] += 1;
            _ = Task.Run(async () =>
            {
                await SpawnCardAsync(spawnChannel, trashChannel, mention, muteRole);
            });
        }

        private void RunSafari(
            EmbedBuilder embed,
            IUserMessage msg,
            Card newCard,
            SafariImage pokeImage,
            CharacterInfo character,
            ITextChannel trashChannel,
            SocketRole mutedRole)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await _taskManager.Delay(TimeSpan.FromMinutes(5));

                    var usersReacted = await msg.GetReactionUsersAsync(Emojis.RaisedHand, 300).FlattenAsync();
                    var users = usersReacted.ToList();

                    IUser winner = null;
                    var watch = Stopwatch.StartNew();
                    while (winner == null)
                    {
                        if (watch.ElapsedMilliseconds > 60000)
                        {
                            throw new Exception("Timeout");
                        }

                        if (users.Count < 1)
                        {
                            embed.Description = $"Na polowanie nie stawił się żaden łowca!";
                            await msg.ModifyAsync(x => x.Embed = embed.Build());
                            return;
                        }

                        bool muted = false;
                        var selected = _randomNumberGenerator.GetOneRandomFrom(users);
                        if (mutedRole != null && selected is SocketGuildUser su)
                        {
                            if (su.Roles.Any(x => x.Id == mutedRole.Id))
                                muted = true;
                        }

                        var dUser = await _userRepository.GetCachedFullUserAsync(selected.Id);

                        if (dUser != null && !muted)
                        {
                            if (!dUser.IsBlacklisted && dUser.GameDeck.MaxNumberOfCards > dUser.GameDeck.Cards.Count)
                            {
                                winner = selected;
                            }
                        }
                        users.Remove(selected);
                    }

                    //var exe = GetSafariExe(embed, msg, newCard, pokeImage, character, trashChannel, winner);
                    //await _executor.TryAdd(exe, TimeSpan.FromSeconds(1));

                    await msg.RemoveAllReactionsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"In Safari", ex);
                    await msg.ModifyAsync(x => x.Embed = "Karta uciekła!".ToEmbedMessage(EMType.Error).Build());
                    await msg.RemoveAllReactionsAsync();
                }
            });
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
        //        var botUser = await _userRepository.GetUserOrCreateAsync(winner.Id);

        //        newCard.FirstIdOwner = winner.Id;
        //        newCard.Affection += botUser.GameDeck.AffectionFromKarma();
        //        botUser.GameDeck.RemoveCharacterFromWishList(newCard.CharacterId);

        //        botUser.GameDeck.Cards.Add(newCard);
        //        await _userRepository.SaveChangesAsync();

        //        _cacheManager.ExpireTag(new string[] { $"user-{botUser.Id}", "users" });

        //        var record = new UserAnalytics
        //        {
        //            Value = 1,
        //            UserId = winner.Id,
        //            MeasureDate = _systemClock.UtcNow,
        //            GuildId = trashChannel?.Guild?.Id ?? 0,
        //            Type = UserAnalyticsEventType.Card
        //        };

        //        _userAnalyticsRepository.Add(record);
        //        await _userAnalyticsRepository.SaveChangesAsync();

        //        _ = Task.Run(async () =>
        //        {
        //            try
        //            {
        //                embed.ImageUrl = await _waifu.GetSafariViewAsync(pokeImage, newCard, trashChannel);
        //                embed.Description = $"{winner.Mention} zdobył na polowaniu i wsadził do klatki:\n"
        //                                + $"{newCard.GetString(false, false, true)}\n({newCard.Title})";
        //                await msg.ModifyAsync(x => x.Embed = embed.Build());

        //                var privEmb = new EmbedBuilder()
        //                {
        //                    Color = EMType.Info.Color(),
        //                    Description = $"Na [polowaniu]({msg.GetJumpUrl()}) zdobyłeś: {newCard.GetString(false, false, true)}"
        //                };

        //                var priv = await winner.GetOrCreateDMChannelAsync();
        //                if (priv != null) await priv.SendMessageAsync("", false, privEmb.Build());
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogInformation($"In Safari: {ex}");
        //            }
        //        });
        //    }));
        //}

        private async Task SpawnCardAsync(ITextChannel spawnChannel, ITextChannel trashChannel, string mention, SocketRole muteRole)
        {
            var character = await _waifu.GetRandomCharacterAsync();

            if (character == null)
            {
                _logger.LogInformation("In Satafi: bad shinden connection");
                return;
            }

            var newCard = _waifu.GenerateNewCard(null, character);
            newCard.Source = CardSource.Safari;
            newCard.Affection -= 1.8;
            newCard.InCage = true;

            var pokeImage = await _waifu.GetRandomSarafiImage();
            var time = _systemClock.UtcNow.AddMinutes(5);
            var embed = new EmbedBuilder
            {
                Color = EMType.Bot.Color(),
                Description = $"**Polowanie zakończy się o**: `{time.ToShortTimeString()}:{time.Second.ToString("00")}`",
                ImageUrl = await _waifu.GetSafariViewAsync(pokeImage, trashChannel)
            };

            var msg = await spawnChannel.SendMessageAsync(mention, embed: embed.Build());
            RunSafari(embed, msg, newCard, pokeImage, character, trashChannel, muteRole);
            await msg.AddReactionAsync(Emojis.RaisedHand);
        }

        private void HandleUserAsync(SocketUserMessage message)
        {
            var author = message.Author;
            if (!UserCounter.Any(x => x.Key == author.Id))
            {
                UserCounter.Add(author.Id, GetMessageRealLength(message));
                return;
            }

            var charNeeded = _experienceConfiguration.CurrentValue.CharPerPacket;
            if (charNeeded <= 0)
            {
                charNeeded = 3250;
            }

            UserCounter[author.Id] += GetMessageRealLength(message);
            if (UserCounter[author.Id] > charNeeded)
            {
                UserCounter[author.Id] = 0;
                var guildUser = author as SocketGuildUser;
                _blockingPriorityQueue.TryEnqueue(new SpawnCardBundleMessage
                {
                    GuildId = guildUser?.Id,
                    Mention = author.Mention,
                    DiscordUserId = author.Id,
                    MessageChannel = message.Channel,
                });
            }
        }

        //private void SpawnUserPacket(SocketUser user, ISocketMessageChannel channel)
        //{
        //    var exe = new Executable($"packet u{user.Id}", new Task<Task>(async () =>
        //    {
        //       
        //    }));

        //    _executor.TryAdd(exe, TimeSpan.FromSeconds(1));
        //}

        private long GetMessageRealLength(SocketUserMessage message)
        {
            if (string.IsNullOrEmpty(message.Content))
            {
                return 1;
            }

            int emoteChars = message.Tags.CountEmotesTextLength();
            int linkChars = message.Content.CountLinkTextLength();
            int nonWhiteSpaceChars = message.Content.Count(c => c != ' ');
            int quotedChars = message.Content.CountQuotedTextLength();
            long charsThatMatters = nonWhiteSpaceChars - linkChars - emoteChars - quotedChars;
            return charsThatMatters < 1 ? 1 : charsThatMatters;
        }

        private async Task HandleMessageAsync(SocketMessage message)
        {
            var userMessage = message as SocketUserMessage;

            if (userMessage == null)
            {
                return;
            }

            var author = userMessage.Author;

            if (author.IsBot || author.IsWebhook)
            {
                return;
            };

            var user = userMessage.Author as SocketGuildUser;

            if (user == null)
            {
                return;
            };

            var guildId = user.Guild.Id;

            if (_discordConfiguration.CurrentValue.BlacklistedGuilds.Any(x => x == guildId))
            {
                return;
            }

            var config = await _guildConfigRepository.GetCachedGuildFullConfigAsync(guildId);
            if (config == null)
            {
                return;
            }

            var noExp = config.ChannelsWithoutExp.Any(x => x.Channel == userMessage.Channel.Id);

            if (!noExp)
            {
                HandleUserAsync(userMessage);
            }

            var sch = user.Guild.GetTextChannel(config.WaifuConfig.SpawnChannelId.Value);
            var tch = user.Guild.GetTextChannel(config.WaifuConfig.TrashSpawnChannelId.Value);
            if (sch != null && tch != null)
            {
                string mention = "";
                var wRole = user.Guild.GetRole(config.WaifuRoleId);
                if (wRole != null) mention = wRole.Mention;

                var muteRole = user.Guild.GetRole(config.MuteRoleId);

                HandleGuildAsync(sch, tch, config.SafariLimit, mention, noExp, muteRole);
            }
        }
    }
}
