using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.Services.Commands;
using Sanakan.Web.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Sanakan.DiscordBot;
using Sanakan.Configuration;
using Sanakan.Common.Configuration;
using Sanakan.TaskQueue.Messages;
using System.Collections.Concurrent;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.Extensions;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.TaskQueue;

namespace Sanakan.Web.HostedService
{
    public class DiscordBotHostedService : BackgroundService
    {
        private DiscordSocketClient _client;
        private readonly IBlockingPriorityQueue _blockingPriorityQueue;
        private readonly IDiscordSocketClientAccessor _discordSocketClientAccessor;
        private readonly CommandHandler _commandHandler;
        private readonly ILogger _logger;
        private readonly IFileSystem _fileSystem;
        private readonly IOptionsMonitor<DiscordConfiguration> _discordConfiguration;
        private readonly IOptionsMonitor<ExperienceConfiguration> _experienceConfiguration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ISystemClock _systemClock;
        private readonly ITaskManager _taskManager;

        private Dictionary<ulong, ulong> _messages;
        private Dictionary<ulong, ulong> _commands;
        private const double SAVE_AT = 5;

        private Dictionary<ulong, double> _userExperienceMap;

        private Dictionary<ulong, DateTime> _saved;
        private Dictionary<ulong, ulong> _characters;

        private Dictionary<ulong, UserExperienceStat> _userExperienceStatsMap;

        private class UserExperienceStat
        {
            public DateTime SavedOn { get; set; }
            public ulong CharacterCount { get; set; }
            public ulong CommandsCount { get; set; }
            public ulong MessagesCount { get; set; }
        }

        public DiscordBotHostedService(
            IFileSystem fileSystem,
            IBlockingPriorityQueue blockingPriorityQueue,
            ILogger<DiscordBotHostedService> logger,
            IServiceScopeFactory serviceScopeFactory,
            IDiscordSocketClientAccessor discordSocketClientAccessor,
            IOptionsMonitor<DiscordConfiguration> discordConfiguration,
            IOptionsMonitor<ExperienceConfiguration> experienceConfiguration,
            ISystemClock systemClock,
            CommandHandler commandHandler,
            ITaskManager taskManager)
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
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                stoppingToken.ThrowIfCancellationRequested();

                using var serviceScope = _serviceScopeFactory.CreateScope();
                var serviceProvider = serviceScope.ServiceProvider;
                var database = serviceProvider.GetRequiredService<DatabaseFacade>();
                await database.EnsureCreatedAsync();

                stoppingToken.ThrowIfCancellationRequested();

                _fileSystem.CreateDirectory(Paths.CardsMiniatures);
                _fileSystem.CreateDirectory(Paths.CardsInProfiles);
                _fileSystem.CreateDirectory(Paths.SavedData);
                _fileSystem.CreateDirectory(Paths.Profiles);
                stoppingToken.ThrowIfCancellationRequested();

                _client = serviceProvider.GetRequiredService<DiscordSocketClient>();
                _client.Log += onLog;
                _client.Disconnected += DisconnectedAsync;
                _client.LeftGuild += BotLeftGuildAsync;
                _client.UserJoined += UserJoinedAsync;
                _client.UserLeft += UserLeftAsync;
                _client.MessageReceived += HandleMessageAsync;
                _client.MessageDeleted += HandleDeletedMsgAsync;
                _client.MessageUpdated += HandleUpdatedMsgAsync;
                stoppingToken.ThrowIfCancellationRequested();

                var configuration = _discordConfiguration.CurrentValue;
                await _client.LoginAsync(TokenType.Bot, configuration.BotToken);
                await _client.SetGameAsync($"{configuration.Prefix}pomoc");
                await _client.StartAsync();
                stoppingToken.ThrowIfCancellationRequested();
                
                await _commandHandler.InitializeAsync();
                
                stoppingToken.ThrowIfCancellationRequested();
                _discordSocketClientAccessor.Client = _client;

                await _taskManager.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
            }
            catch (InvalidOperationException)
            {
                _discordSocketClientAccessor.Client?.Dispose();
                _discordSocketClientAccessor.Client = null;
                _logger.LogInformation("The discord client stopped");
            }

           
        }

        private void CountMessage(ulong userId, bool isCommand)
        {
            if (!_messages.Any(x => x.Key == userId))
            {
                _messages.Add(userId, 1);
            }
            else
            {
                _messages[userId]++;
            }

            if (!_commands.Any(x => x.Key == userId))
            {
                _commands.Add(userId, isCommand ? 1u : 0u);
            }
            else if (isCommand)
            {
                _commands[userId]++;
            }
        }

        private async Task HandleMessageAsync(SocketMessage message)
        {
            if (message.Author.IsBot || message.Author.IsWebhook)
            {
                return;
            }

            var user = message.Author as SocketGuildUser;

            if (user == null)
            {
                return;
            }

            if (_discordConfiguration.CurrentValue
                .BlacklistedGuilds.Any(x => x == user.Guild.Id))
            {
                return;
            }

            var countMsg = true;
            var calculateExp = true;

            using var serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var guildConfigRepository = serviceProvider.GetRequiredService<IGuildConfigRepository>();
            var userRepository = serviceProvider.GetRequiredService<IUserRepository>();

            var config = await guildConfigRepository.GetCachedGuildFullConfigAsync(user.Guild.Id);
            if (config != null)
            {
                var role = user.Guild.GetRole(config.UserRoleId);
                if (role != null)
                {
                    if (!user.Roles.Contains(role))
                    {
                        return;
                    }
                }

                if (config.ChannelsWithoutExp != null)
                {
                    if (config.ChannelsWithoutExp.Any(x => x.Channel == message.Channel.Id))
                        calculateExp = false;
                }

                if (config.IgnoredChannels != null)
                {
                    if (config.IgnoredChannels.Any(x => x.Channel == message.Channel.Id))
                        countMsg = false;
                }
            }

            if (!_messages.Any(x => x.Key == user.Id))
            {
                if (!await userRepository.ExistsByDiscordIdAsync(user.Id))
                {
                    var databseUser = new User(user.Id, _systemClock.StartOfMonth);
                    userRepository.Add(databseUser);
                    await userRepository.SaveChangesAsync();
                }
            }

            if (countMsg)
            {
                CountMessage(user.Id, _discordConfiguration.CurrentValue.IsCommand(message.Content));
            }
            CalculateExpAndCreateTask(user, message, calculateExp);
        }

        private void CountCharacters(ulong userId, ulong characters)
        {
            if (!_characters.Any(x => x.Key == userId))
            {
                _characters.Add(userId, characters);
            }
            else
            {
                _characters[userId] += characters;
            }
        }

        private double GetPointsFromMessage(SocketMessage message)
        {
            int emoteChars = message.Tags.CountEmotesTextLength();
            int linkChars = message.Content.CountLinkTextLength();
            int nonWhiteSpaceChars = message.Content.Count(c => c != ' ');
            int quotedChars = message.Content.CountQuotedTextLength();
            double charsThatMatters = nonWhiteSpaceChars - linkChars - emoteChars - quotedChars;

            CountCharacters(message.Author.Id, (ulong)(charsThatMatters < 1 ? 1 : charsThatMatters));
            return GetExpPointBasedOnCharCount(charsThatMatters);
        }

        private double GetExpPointBasedOnCharCount(double charCount)
        {
            var experienceConfiguration = _experienceConfiguration.CurrentValue;
            var cpp = experienceConfiguration.CharPerPoint;
            var min = experienceConfiguration.MinPerMessage;
            var max = experienceConfiguration.MaxPerMessage;

            double experience = charCount / cpp;
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

        private void CalculateExpAndCreateTask(SocketGuildUser user, SocketMessage message, bool calculateExperience)
        {
            var experience = GetPointsFromMessage(message);

            if (!calculateExperience)
            {
                experience = 0;
            }

            if (!_userExperienceMap.Any(x => x.Key == user.Id))
            {
                _userExperienceMap.Add(message.Author.Id, experience);
                return;
            }

            _userExperienceMap[user.Id] += experience;

            var saved = _userExperienceMap[user.Id];
            if (saved < SAVE_AT && !CheckLastSave(user.Id))
            {
                return;
            }

            var effectiveExperience = (long)Math.Floor(saved);
            if (effectiveExperience < 1)
            {
                return;
            }

            _userExperienceMap[message.Author.Id] -= effectiveExperience;
            _saved[user.Id] = _systemClock.UtcNow;

            var messageCount = _messages[user.Id];

            //var task = CreateTask(user, message.Channel, effectiveExperience, , , , calculateExperience);
            _characters[user.Id] = 0;
            _messages[user.Id] = 0;
            _commands[user.Id] = 0;

            //    SocketGuildUser discordUser,
            //    ISocketMessageChannel channel,
            //    ulong experience,
            //    ulong messages,
            //    ulong commands,
            //    ulong characters,
            //    bool calculateExp)

            _blockingPriorityQueue.TryEnqueue(new AddExperienceMessage
            {
                Experience = effectiveExperience,
                DiscordUserId = user.Id,
                CommandCount = _commands[user.Id],
                CharacterCount = _characters[user.Id],
                MessageCount = messageCount,
                CalculateExperience = calculateExperience,
                GuildId = user.Guild.Id,
                User = user,
                Channel = message.Channel,
            }); ;
        }

        private bool CheckLastSave(ulong userId)
        {
            if (!_saved.Any(x => x.Key == userId))
            {
                _saved.Add(userId, _systemClock.UtcNow);
                return false;
            }

            return (_systemClock.UtcNow - _saved[userId].AddMinutes(30)).TotalSeconds > 1;
        }

        private async Task DisconnectedAsync(Exception ex)
        {
            _logger.LogError("Discord client disconnected.", ex);

            var configuration = _discordConfiguration.CurrentValue;

            if (!configuration.RestartWhenDisconnected)
            {
                return;
            }

            await _taskManager.Delay(TimeSpan.FromSeconds(40));

            if (_client.ConnectionState == ConnectionState.Connected)
            {
                return;
            }

            await _client.StartAsync();
            _logger.LogInformation("Reconnected!");

            return;
        }

        private async Task BotLeftGuildAsync(SocketGuild guild)
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

        private async Task UserJoinedAsync(SocketGuildUser user)
        {
            if (user.IsBot || user.IsWebhook)
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
            var textChannel = user.Guild.GetTextChannel(guildConfig.GreetingChannelId);
            await SendMessageAsync(content, textChannel);

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

        private async Task UserLeftAsync(SocketGuildUser user)
        {
            if (user.IsBot || user.IsWebhook)
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
                var textChannel = user.Guild.GetTextChannel(guildConfig.GreetingChannelId);
                await SendMessageAsync(content, textChannel);
            }

            var thisUser = _client.Guilds.FirstOrDefault(x => x.Id == user.Id);
            if (thisUser != null)
            {
                return;
            }

            _blockingPriorityQueue.TryEnqueue(new DeleteUserMessage
            {
                DiscordUserId = user.Id,
            });

            //var moveTask = new Task<Task>(async () =>
            //{
         
            //});

            //await _executor.TryAdd(new Executable("delete user", moveTask, Priority.High), TimeSpan.FromSeconds(1));
        }

        private async Task HandleUpdatedMsgAsync(
    Cacheable<IMessage, ulong> oldMessage,
    SocketMessage newMessage,
    ISocketMessageChannel channel)
        {
            if (!oldMessage.HasValue)
            {
                return;
            }


            if (newMessage.Author.IsBot || newMessage.Author.IsWebhook)
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

        private async Task HandleDeletedMsgAsync(Cacheable<IMessage, ulong> message, ISocketMessageChannel channel)
        {
            if (!message.HasValue)
            {
                return;
            }

            if (message.Value.Author.IsBot || message.Value.Author.IsWebhook)
            {
                return;
            }

            if (message.Value.Content.Length < 4 && message.Value.Attachments.Count < 1)
            {
                return;
            }

            if (message.Value.Channel is SocketGuildChannel gChannel)
            {
                if (_discordConfiguration.CurrentValue.BlacklistedGuilds.Any(x => x == gChannel.Guild.Id))
                {
                    return;
                }

                _ = Task.Run(async () =>
                {
                    await LogMessageAsync(gChannel, message.Value);
                });
            }

            await Task.CompletedTask;
        }

        private async Task LogMessageAsync(SocketGuildChannel channel, IMessage oldMessage, IMessage? newMessage = null)
        {
            using var serviceScope = _serviceScopeFactory.CreateScope();
            var guildConfigRepository = serviceScope.ServiceProvider.GetRequiredService<IGuildConfigRepository>();

            if (oldMessage.Content.IsEmotikunEmote() && newMessage == null)
            {
                return;
            }

            var config = await guildConfigRepository.GetCachedGuildFullConfigAsync(channel.Guild.Id);

            if (config == null)
            {
                return;
            }

            var textChannel = channel.Guild.GetTextChannel(config.LogChannelId);

            if (textChannel == null)
            {
                return;
            }

            var jump = (newMessage == null) ? "" : $"{newMessage.GetJumpUrl()}";
            await textChannel.SendMessageAsync(jump, embed: BuildMessage(oldMessage, newMessage));
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

        private async Task SendMessageAsync(string message, ITextChannel channel)
        {
            if (channel != null) await channel.SendMessageAsync(message);
        }

        private string ReplaceTags(SocketGuildUser user, string message)
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
