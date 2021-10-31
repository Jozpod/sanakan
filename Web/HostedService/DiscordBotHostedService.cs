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

namespace Sanakan.Web.HostedService
{
    public class DiscordBotHostedService : BackgroundService
    {
        private DiscordSocketClient _client;
        private readonly IProducerConsumerCollection<BaseMessage> _taskQueue;
        private readonly IDiscordSocketClientAccessor _discordSocketClientAccessor;
        private readonly CommandHandler _commandHandler;
        private readonly ILogger _logger;
        private readonly IFileSystem _fileSystem;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IOptionsMonitor<DiscordConfiguration> _configuration;
        private Dictionary<ulong, ulong> _messages;
        private Dictionary<ulong, ulong> _commands;

        public DiscordBotHostedService(
            IFileSystem fileSystem,
            ILogger<DiscordBotHostedService> logger,
            IServiceScopeFactory serviceScopeFactory,
            IDiscordSocketClientAccessor discordSocketClientAccessor,
            IOptionsMonitor<DiscordConfiguration> configuration,
            CommandHandler commandHandler)
        {
            _logger = logger;
            _fileSystem = fileSystem;
            _discordSocketClientAccessor = discordSocketClientAccessor;
            _serviceScopeFactory = serviceScopeFactory;
            _configuration = configuration;
            _commandHandler = commandHandler;
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
                stoppingToken.ThrowIfCancellationRequested();

                var configuration = _configuration.CurrentValue;
                await _client.LoginAsync(TokenType.Bot, configuration.BotToken);
                await _client.SetGameAsync($"{configuration.Prefix}pomoc");
                await _client.StartAsync();
                stoppingToken.ThrowIfCancellationRequested();

                //_executor.Initialize(services);
                //_sessions.Initialize(services);
                
                await _commandHandler.InitializeAsync();
                
                stoppingToken.ThrowIfCancellationRequested();
                _discordSocketClientAccessor.Client = _client;

                await Task.Delay(Timeout.Infinite, stoppingToken);
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

            if (_configuration.CurrentValue
                .BlacklistedGuilds.Any(x => x == user.Guild.Id))
            {
                return;
            }

            var countMsg = true;
            var calculateExp = true;

            var config = await _guildConfigRepository.GetCachedGuildFullConfigAsync(user.Guild.Id);
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
                if (!await _userRepository.ExistsByDiscordIdAsync(user.Id))
                {
                    var databseUser = new User(user.Id, _systemClock.StartOfMonth);
                    _userRepository.Add(databseUser);
                    await _userRepository.SaveChangesAsync();
                }
            }

            if (countMsg)
            {
                CountMessage(user.Id, _config.CurrentValue.IsCommand(message.Content));
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
            var config = _config.CurrentValue;
            var cpp = config.Exp.CharPerPoint;
            var min = config.Exp.MinPerMessage;
            var max = config.Exp.MaxPerMessage;

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

            _blockingPriorityQueue.TryAdd(new AddExperienceMessage
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

        private async Task DisconnectedAsync(Exception ex)
        {
            _logger.LogError("Discord client disconnected.", ex);

            var configuration = _configuration.CurrentValue;

            if (!configuration.RestartWhenDisconnected)
            {
                return;
            }

            await Task.Delay(TimeSpan.FromSeconds(40));

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
            var gConfig = await _guildConfigRepository.GetGuildConfigOrCreateAsync(guild.Id);
            _guildConfigRepository.Remove(gConfig);

            var stats = await _timeStatusRepository.GetByGuildIdAsync(guild.Id);
            _timeStatusRepository.RemoveRange(stats);

            await _timeStatusRepository.SaveChangesAsync();

            var mutes = await _penaltyInfoRepository.GetByGuildIdAsync(guild.Id);
            _penaltyInfoRepository.RemoveRange(mutes);

            await _penaltyInfoRepository.SaveChangesAsync();
        }

        private async Task UserJoinedAsync(SocketGuildUser user)
        {
            if (user.IsBot || user.IsWebhook)
            {
                return;
            }

            var guildId = user.Guild.Id;

            if (_config.CurrentValue
                .BlacklistedGuilds.Any(x => x == guildId))
            {
                return;
            }

            var guildConfig = await _guildConfigRepository.GetCachedGuildFullConfigAsync(guildId);

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

            var config = _configuration.CurrentValue;
            var guildId = user.Guild.Id;

            if (!config.BlacklistedGuilds.Any(x => x == guildId))
            {
                var guildConfig = await _guildConfigRepository.GetCachedGuildFullConfigAsync(guildId);
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

            _taskQueue.TryAdd(new DeleteUserMessage
            {
            });

            //var moveTask = new Task<Task>(async () =>
            //{
            //    var duser = await _userRepository.GetUserOrCreateAsync(user.Id);
            //    var fakeu = await _userRepository.GetUserOrCreateAsync(1);

            //    foreach (var card in duser.GameDeck.Cards)
            //    {
            //        card.InCage = false;
            //        card.TagList.Clear();
            //        card.LastIdOwner = user.Id;
            //        card.GameDeckId = fakeu.GameDeck.Id;
            //    }

            //    _userRepository.Remove(duser);

            //    await _userRepository.SaveChangesAsync();

            //    _cacheManager.ExpireTag(new string[] { "users" });
            //});

            //await _executor.TryAdd(new Executable("delete user", moveTask, Priority.High), TimeSpan.FromSeconds(1));
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
