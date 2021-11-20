using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sanakan.DiscordBot;
using Sanakan.Common.Configuration;
using Sanakan.TaskQueue.Messages;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.Extensions;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.TaskQueue;
using Sanakan.DAL;

namespace Sanakan.Web.HostedService
{
    internal class DiscordBotHostedService : BackgroundService
    {
        private readonly IBlockingPriorityQueue _blockingPriorityQueue;
        private readonly IDiscordClientAccessor _discordSocketClientAccessor;
        private readonly ICommandHandler _commandHandler;
        private readonly ILogger _logger;
        private readonly IFileSystem _fileSystem;
        private readonly IOptionsMonitor<DiscordConfiguration> _discordConfiguration;
        private readonly IOptionsMonitor<ExperienceConfiguration> _experienceConfiguration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ISystemClock _systemClock;
        private readonly ITaskManager _taskManager;
        private readonly IDatabaseFacade _databaseFacade;
        private const double experienceSaveThreshold = 5;
        private IDictionary<ulong, UserStat> _userStatsMap;
        private readonly TimeSpan _halfAnHour;

        private class UserStat
        {
            public DateTime? SavedOn { get; set; }
            public double Experience { get; set; }
            public ulong CharacterCount { get; set; }
            public ulong CommandsCount { get; set; }
            public ulong MessagesCount { get; set; }
        }

        public DiscordBotHostedService(
            IFileSystem fileSystem,
            IBlockingPriorityQueue blockingPriorityQueue,
            ILogger<DiscordBotHostedService> logger,
            IServiceScopeFactory serviceScopeFactory,
            IDiscordClientAccessor discordSocketClientAccessor,
            IOptionsMonitor<DiscordConfiguration> discordConfiguration,
            IOptionsMonitor<ExperienceConfiguration> experienceConfiguration,
            ISystemClock systemClock,
            ICommandHandler commandHandler,
            ITaskManager taskManager,
            IDatabaseFacade databaseFacade)
        {
            _fileSystem = fileSystem;
            _blockingPriorityQueue = blockingPriorityQueue;
            _logger = logger;
            _discordSocketClientAccessor = discordSocketClientAccessor;
            _serviceScopeFactory = serviceScopeFactory;
            _discordConfiguration = discordConfiguration;
            _experienceConfiguration = experienceConfiguration;
            _systemClock = systemClock;
            _commandHandler = commandHandler;
            _taskManager = taskManager;
            _databaseFacade = databaseFacade;
            _halfAnHour = TimeSpan.FromMinutes(30);
            _userStatsMap = new Dictionary<ulong, UserStat>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                stoppingToken.ThrowIfCancellationRequested();

                await _databaseFacade.EnsureCreatedAsync(stoppingToken);

                stoppingToken.ThrowIfCancellationRequested();

                _fileSystem.CreateDirectory(Paths.CardsMiniatures);
                _fileSystem.CreateDirectory(Paths.CardsInProfiles);
                _fileSystem.CreateDirectory(Paths.SavedData);
                _fileSystem.CreateDirectory(Paths.Profiles);
                stoppingToken.ThrowIfCancellationRequested();

                _discordSocketClientAccessor.Log += onLog;
                _discordSocketClientAccessor.Disconnected += DisconnectedAsync;
                _discordSocketClientAccessor.LeftGuild += BotLeftGuildAsync;
                _discordSocketClientAccessor.UserJoined += UserJoinedAsync;
                _discordSocketClientAccessor.UserLeft += UserLeftAsync;
                _discordSocketClientAccessor.MessageReceived += HandleMessageAsync;
                _discordSocketClientAccessor.MessageDeleted += HandleDeletedMessageAsync;
                _discordSocketClientAccessor.MessageUpdated += HandleUpdatedMessageAsync;
                stoppingToken.ThrowIfCancellationRequested();

                var configuration = _discordConfiguration.CurrentValue;
                await _discordSocketClientAccessor.LoginAsync(TokenType.Bot, configuration.BotToken);
                await _discordSocketClientAccessor.SetGameAsync($"{configuration.Prefix}pomoc");
                await _discordSocketClientAccessor.Client.StartAsync();
                stoppingToken.ThrowIfCancellationRequested();
                
                await _commandHandler.InitializeAsync();
                
                stoppingToken.ThrowIfCancellationRequested();

                await _taskManager.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
            }
            catch (InvalidOperationException)
            {
                _logger.LogInformation("The discord client stopped");
            }
            catch(Exception ex)
            {
                _logger.LogInformation("Error occurred while running discord bot daemon", ex);
            }
        }

        private async Task HandleMessageAsync(IMessage message)
        {
            var user = message.Author;

            if (user.IsBotOrWebhook())
            {
                return;
            }

            var guildUser = user as IGuildUser;

            if (guildUser == null)
            {
                return;
            }

            var guild = guildUser.Guild;

            if (_discordConfiguration.CurrentValue
                .BlacklistedGuilds.Any(x => x == guild.Id))
            {
                return;
            }

            var countMessages = true;
            var calculateExperience = true;

            using var serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var guildConfigRepository = serviceProvider.GetRequiredService<IGuildConfigRepository>();
            var userRepository = serviceProvider.GetRequiredService<IUserRepository>();

            var config = await guildConfigRepository.GetCachedGuildFullConfigAsync(guild.Id);
            if (config != null)
            {
                var role = config.UserRoleId.HasValue ? guild.GetRole(config.UserRoleId.Value) : null;
                if (role != null)
                {
                    if (!guildUser.RoleIds.Contains(role.Id))
                    {
                        return;
                    }
                }

                if (config.ChannelsWithoutExperience.Any(x => x.Channel == message.Channel.Id))
                {
                    calculateExperience = false;
                }

                if (config.IgnoredChannels.Any(x => x.Channel == message.Channel.Id))
                {
                    countMessages = false;
                }
            }

            var userId = user.Id;

            if (!_userStatsMap.TryGetValue(userId, out var userExperienceStat))
            {
                if (!await userRepository.ExistsByDiscordIdAsync(user.Id))
                {
                    var databseUser = new User(userId, _systemClock.StartOfMonth);
                    userRepository.Add(databseUser);
                    await userRepository.SaveChangesAsync();
                    userExperienceStat = new UserStat();
                    _userStatsMap[userId] = userExperienceStat;
                }
            }

            if (countMessages)
            {
                var isCommand = _discordConfiguration.CurrentValue.IsCommand(message.Content);
                userExperienceStat.MessagesCount++;

                if(isCommand)
                {
                    userExperienceStat.CommandsCount++;
                }
            }

            CalculateExperienceAndCreateTask(
                userExperienceStat,
                guildUser,
                message,
                calculateExperience);
        }

        private double GetExpPointBasedOnCharCount(double charCount)
        {
            var experienceConfiguration = _experienceConfiguration.CurrentValue;
            var charPerPoint = experienceConfiguration.CharPerPoint;
            var min = experienceConfiguration.MinPerMessage;
            var max = experienceConfiguration.MaxPerMessage;

            var experience = charCount / charPerPoint;

            if (experience < min)
            {
                return min;
            }

            if (experience > max)
            {
                return max;
            }

            return experience;
        }

        private void CalculateExperienceAndCreateTask(
            UserStat userExperienceStat,
            IGuildUser user,
            IMessage message,
            bool calculateExperience)
        {
            var content = message.Content;
            var emoteChars = message.Tags.CountEmotesTextLength();
            var linkChars = content.CountLinkTextLength();
            var nonWhiteSpaceChars = content.Count(character => !char.IsWhiteSpace(character));
            var quotedChars = content.CountQuotedTextLength();
            var charsThatMatters = nonWhiteSpaceChars - linkChars - emoteChars - quotedChars;

            var effectiveCharacters = (ulong)(charsThatMatters < 1 ? 1 : charsThatMatters);
            userExperienceStat.CharacterCount += effectiveCharacters;
            var experience = GetExpPointBasedOnCharCount(charsThatMatters);

            if (!calculateExperience)
            {
                experience = 0;
            }

            if(userExperienceStat.Experience == 0)
            {
                userExperienceStat.Experience = experience;
                return;
            }

            var utcNow = _systemClock.UtcNow;

            userExperienceStat.Experience += experience;
            
            if(!userExperienceStat.SavedOn.HasValue)
            {
                userExperienceStat.SavedOn = utcNow;
            }

            var halfAnHourElapsed = (utcNow - userExperienceStat.SavedOn.Value) > _halfAnHour;

            if (userExperienceStat.Experience < experienceSaveThreshold && !halfAnHourElapsed)
            {
                return;
            }

            var effectiveExperience = (long)Math.Floor(userExperienceStat.Experience);

            if (effectiveExperience < 1)
            {
                return;
            }

            userExperienceStat.Experience -= effectiveExperience;
            userExperienceStat.SavedOn = utcNow;

            _blockingPriorityQueue.TryEnqueue(new AddExperienceMessage
            {
                Experience = effectiveExperience,
                DiscordUserId = user.Id,
                CommandCount = userExperienceStat.CharacterCount,
                CharacterCount = userExperienceStat.CharacterCount,
                MessageCount = userExperienceStat.MessagesCount,
                CalculateExperience = calculateExperience,
                GuildId = user.Guild.Id,
                User = user,
                Channel = message.Channel,
            });

            userExperienceStat.CharacterCount = 0;
            userExperienceStat.MessagesCount = 0;
            userExperienceStat.CommandsCount = 0;
        }

        private async Task DisconnectedAsync(Exception ex)
        {
            _logger.LogError("Discord client disconnected.", ex);

            var configuration = _discordConfiguration.CurrentValue;

            if (!configuration.RestartWhenDisconnected)
            {
                return;
            }

            var reconnectTimeSpan = TimeSpan.FromSeconds(40);

            _logger.LogDebug("Reconnecting after {0}", reconnectTimeSpan);

            await _taskManager.Delay(reconnectTimeSpan);

            var client = _discordSocketClientAccessor.Client;

            if (client.ConnectionState == ConnectionState.Connected)
            {
                _logger.LogDebug("Already connected");
                return;
            }

            await client.StartAsync();
            _logger.LogInformation("Reconnected!");

            return;
        }

        private async Task BotLeftGuildAsync(IGuild guild)
        {
            using var serviceScope = _serviceScopeFactory.CreateScope();
            var guildConfigRepository = serviceScope.ServiceProvider.GetRequiredService<IGuildConfigRepository>();
            var timeStatusRepository = serviceScope.ServiceProvider.GetRequiredService<ITimeStatusRepository>();
            var penaltyInfoRepository = serviceScope.ServiceProvider.GetRequiredService<IPenaltyInfoRepository>();

            var gConfig = await guildConfigRepository.GetGuildConfigOrCreateAsync(guild.Id);
            guildConfigRepository.Remove(gConfig);

            var stats = await timeStatusRepository.GetByGuildIdAsync(guild.Id);
            timeStatusRepository.RemoveRange(stats);

            await timeStatusRepository.SaveChangesAsync();

            var mutes = await penaltyInfoRepository.GetByGuildIdAsync(guild.Id);
            penaltyInfoRepository.RemoveRange(mutes);

            await penaltyInfoRepository.SaveChangesAsync();
        }

        private async Task UserJoinedAsync(IGuildUser user)
        {
            if (user.IsBotOrWebhook())
            {
                return;
            }

            var guildId = user.Guild.Id;

            if (_discordConfiguration.CurrentValue
                .BlacklistedGuilds.Any(x => x == guildId))
            {
                return;
            }

            using var serviceScope = _serviceScopeFactory.CreateScope();
            var guildConfigRepository = serviceScope.ServiceProvider.GetRequiredService<IGuildConfigRepository>();

            var guildConfig = await guildConfigRepository.GetCachedGuildFullConfigAsync(guildId);

            if (guildConfig?.WelcomeMessage == null)
            {
                return;
            }

            if (guildConfig.WelcomeMessage == "off")
            {
                return;
            }

            var content = ReplaceTags(user, guildConfig.WelcomeMessage);
            var textChannel = (IMessageChannel)await user.Guild.GetChannelAsync(guildConfig.GreetingChannelId);
            await textChannel.SendMessageAsync(content);

            if (guildConfig?.WelcomeMessagePM == null)
            {
                return;
            }

            if (guildConfig.WelcomeMessagePM == "off")
            {
                return;
            }

            try
            {
                var pw = await user.GetOrCreateDMChannelAsync();
                await pw.SendMessageAsync(ReplaceTags(user, guildConfig.WelcomeMessagePM));
                await pw.CloseAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Greeting: {ex}", ex);
            }
        }

        private async Task UserLeftAsync(IGuildUser user)
        {
            if (user.IsBotOrWebhook())
            {
                return;
            }

            var config = _discordConfiguration.CurrentValue;
            var guildId = user.Guild.Id;

            using var serviceScope = _serviceScopeFactory.CreateScope();
            var guildConfigRepository = serviceScope.ServiceProvider.GetRequiredService<IGuildConfigRepository>();

            if (!config.BlacklistedGuilds.Any(x => x == guildId))
            {
                var guildConfig = await guildConfigRepository.GetCachedGuildFullConfigAsync(guildId);
                if (guildConfig?.GoodbyeMessage == null)
                {
                    return;
                }

                if (guildConfig.GoodbyeMessage == "off")
                {
                    return;
                }

                var content = ReplaceTags(user, guildConfig.GoodbyeMessage);
                var textChannel = (IMessageChannel)await user.Guild.GetChannelAsync(guildConfig.GreetingChannelId);
                await textChannel.SendMessageAsync(content);
            }

            var client = _discordSocketClientAccessor.Client;

            _blockingPriorityQueue.TryEnqueue(new DeleteUserMessage
            {
                DiscordUserId = user.Id,
            });
        }

        private async Task HandleUpdatedMessageAsync(
            Cacheable<IMessage, ulong> oldMessage,
            IMessage newMessage,
            ISocketMessageChannel channel)
        {
            if (!oldMessage.HasValue)
            {
                return;
            }

            var user = newMessage.Author;

            if (user.IsBotOrWebhook())
            {
                return;
            }

            if (oldMessage.Value.Content.Equals(newMessage.Content, StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }

            if (newMessage.Channel is SocketGuildChannel gChannel)
            {
                if (_discordConfiguration.CurrentValue.BlacklistedGuilds.Any(x => x == gChannel.Guild.Id))
                {
                    return;
                }

                _ = Task.Run(async () =>
                {
                    await LogMessageAsync(gChannel, oldMessage.Value, newMessage);
                });
            }

            await Task.CompletedTask;
        }

        private async Task HandleDeletedMessageAsync(Cacheable<IMessage, ulong> cachedMessage, IChannel channel)
        {
            if (!cachedMessage.HasValue)
            {
                return;
            }

            var message = cachedMessage.Value;
            var user = message.Author;

            if (user.IsBotOrWebhook())
            {
                return;
            }

            if (message.Content.Length < 4 && message.Attachments.Count < 1)
            {
                return;
            }

            if (message.Channel is IGuildChannel gChannel)
            {
                if (_discordConfiguration.CurrentValue.BlacklistedGuilds.Any(x => x == gChannel.Guild.Id))
                {
                    return;
                }

                await LogMessageAsync(gChannel, message);
            }

        }

        private async Task LogMessageAsync(IGuildChannel channel, IMessage oldMessage, IMessage? newMessage = null)
        {
            using var serviceScope = _serviceScopeFactory.CreateScope();
            var guildConfigRepository = serviceScope.ServiceProvider.GetRequiredService<IGuildConfigRepository>();

            if (oldMessage.Content.IsEmotikunEmote() && newMessage == null)
            {
                return;
            }

            var guild = channel.Guild;
            var config = await guildConfigRepository.GetCachedGuildFullConfigAsync(guild.Id);

            if (config == null)
            {
                return;
            }

            var textChannel = (await guild.GetChannelAsync(config.LogChannelId)) as IMessageChannel;

            if (textChannel == null)
            {
                return;
            }

            try
            {
                var jumpUrl = (newMessage == null) ? "" : $"{newMessage.GetJumpUrl()}";
                await textChannel.SendMessageAsync(jumpUrl, embed: BuildMessage(oldMessage, newMessage));
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private Embed BuildMessage(IMessage oldMessage, IMessage newMessage)
        {
            string content = (newMessage == null) ? oldMessage.Content
                : $"**Stara:**\n{oldMessage.Content}\n\n**Nowa:**\n{newMessage.Content}";

            return new EmbedBuilder
            {
                Color = (newMessage == null) ? EMType.Warning.Color() : EMType.Info.Color(),
                Author = new EmbedAuthorBuilder().WithUser(oldMessage.Author, true),
                Fields = GetFields(oldMessage, newMessage == null),
                Description = content.ElipseTrimToLength(1800),
            }.Build();
        }

        private List<EmbedFieldBuilder> GetFields(IMessage message, bool deleted)
        {
            var fields = new List<EmbedFieldBuilder>
            {
                new EmbedFieldBuilder
                {
                    IsInline = true,
                    Name = deleted ? "Napisano:" : "Edytowano:",
                    Value = message.GetLocalCreatedAtShortDateTime()
                },
                new EmbedFieldBuilder
                {
                    IsInline = true,
                    Name = "Kanał:",
                    Value = message.Channel.Name
                }
            };

            if (deleted)
            {
                fields.Add(new EmbedFieldBuilder
                {
                    IsInline = true,
                    Name = "Załączniki:",
                    Value = message.Attachments?.Count
                });
            }

            return fields;
        }

        private string ReplaceTags(IGuildUser user, string message)
            => message.Replace("^nick", user.Nickname ?? user.Username).Replace("^mention", user.Mention);

        private Task onLog(LogMessage log)
        {
            switch (log.Severity)
            {
                case LogSeverity.Debug:
                    _logger.LogDebug(log.Message);
                    break;
                case LogSeverity.Info:
                    _logger.LogInformation(log.Message);
                    break;
                case LogSeverity.Verbose:
                    _logger.LogDebug(log.Message);
                    break;
                case LogSeverity.Error:
                    _logger.LogError(log.Message);
                    break;
                default:
                    _logger.LogInformation(log.Message);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
