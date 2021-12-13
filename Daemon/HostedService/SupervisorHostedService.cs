using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using System;
using System.Threading;
using System.Threading.Tasks;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot;
using Sanakan.Common.Configuration;
using System.Linq;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Abstractions.Models;
using Discord;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.DiscordBot.Supervisor;

namespace Sanakan.Daemon.HostedService
{
    internal class SupervisorHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IDiscordClientAccessor _discordSocketClientAccessor;
        private readonly IOptionsMonitor<DaemonsConfiguration> _daemonsConfiguration;
        private readonly IOptionsMonitor<DiscordConfiguration> _discordConfiguration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ITaskManager _taskManager;
        private readonly ITimer _timer;
        private readonly IUserMessageSupervisor _userMessageSupervisor;
        private readonly IUserJoinedGuildSupervisor _userJoinedGuildSupervisor;
        private readonly object _syncRoot = new object();
        private bool _isRunning;

        public SupervisorHostedService(
            ILogger<SupervisorHostedService> logger,
            IDiscordClientAccessor discordSocketClientAccessor,
            IOptionsMonitor<DaemonsConfiguration> daemonsConfiguration,
            IOptionsMonitor<DiscordConfiguration> discordConfiguration,
            IServiceScopeFactory serviceScopeFactory,
            ITaskManager taskManager,
            ITimer timer,
            IUserMessageSupervisor userMessageSupervisor,
            IUserJoinedGuildSupervisor userJoinedGuildSupervisor)
        {
            _logger = logger;
            _discordSocketClientAccessor = discordSocketClientAccessor;
            _serviceScopeFactory = serviceScopeFactory;
            _daemonsConfiguration = daemonsConfiguration;
            _discordConfiguration = discordConfiguration;
            _taskManager = taskManager;
            _timer = timer;

            _discordSocketClientAccessor.LoggedIn += LoggedIn;
            _userMessageSupervisor = userMessageSupervisor;
            _userJoinedGuildSupervisor = userJoinedGuildSupervisor;
        }

        private Task LoggedIn()
        {
            _discordSocketClientAccessor.MessageReceived += HandleMessageAsync;
            _discordSocketClientAccessor.UserJoined += UserJoinedAsync;
            return Task.CompletedTask;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                stoppingToken.ThrowIfCancellationRequested();
                _timer.Tick += OnTick;
                _timer.Start(
                    _daemonsConfiguration.CurrentValue.SupervisorDueTime,
                    _daemonsConfiguration.CurrentValue.SupervisorPeriod);

                await _taskManager.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _timer.Stop();
            }
        }

        internal void OnTick(object sender, TimerEventArgs e)
        {
            if (_isRunning)
            {
                return;
            }

            _isRunning = true;

            try
            {
                lock (_syncRoot)
                {
                    _userJoinedGuildSupervisor.Refresh();
                    _userMessageSupervisor.Refresh();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred during refreshing supervisor subjects.", ex);
            }
            finally
            {
                _isRunning = false;
            }
        }

        private string GetMessageContent(IUserMessage message)
        {
            var content = message.Content;
            if (string.IsNullOrEmpty(content))
            {
                content = message?.Attachments?.FirstOrDefault()?.Filename ?? "embed";
            }

            return content;
        }

        internal async Task UserJoinedAsync(IGuildUser user)
        {
            if (!_discordConfiguration.CurrentValue.FloodSpamSupervisionEnabled)
            {
                return;
            }

            if (user.IsBotOrWebhook())
            {
                return;
            }

            var guild = user.Guild;

            if (_discordConfiguration.CurrentValue.BlacklistedGuilds.Any(x => x == guild.Id))
            {
                return;
            }

            using var serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var guildConfigRepository = serviceProvider.GetRequiredService<IGuildConfigRepository>();

            var guildConfig = await guildConfigRepository.GetCachedGuildFullConfigAsync(guild.Id);

            if (guildConfig == null)
            {
                return;
            }

            if (!guildConfig.SupervisionEnabled)
            {
                return;
            }

            var userIds = Enumerable.Empty<ulong>();

            lock (_syncRoot)
            {
                userIds = _userJoinedGuildSupervisor.GetUsersToBanCauseRaid(guild.Id, user.Username, user.Id);
            }

            var usersToBan = await Task.WhenAll(userIds.Select(pr => guild.GetUserAsync(pr)));
            foreach (var userToBan in usersToBan)
            {
                await guild.AddBanAsync(user, 1, $"Supervisor(ban) raid/scam [{user.Nickname}]");
            }

        }

        internal async Task HandleMessageAsync(IMessage message)
        {
            if (!_discordConfiguration.CurrentValue.FloodSpamSupervisionEnabled)
            {
                return;
            }

            var userMessage = message as IUserMessage;

            if (userMessage == null)
            {
                return;
            }

            var messageUser = userMessage.Author;

            if (messageUser.IsBotOrWebhook())
            {
                return;
            }

            var user = messageUser as IGuildUser;

            if (user == null)
            {
                return;
            }

            var guild = user.Guild;

            if (_discordConfiguration.CurrentValue.BlacklistedGuilds.Any(x => x == guild.Id))
            {
                return;
            }

            using var serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var guildConfigRepository = serviceProvider.GetRequiredService<IGuildConfigRepository>();

            var guildConfig = await guildConfigRepository.GetCachedGuildFullConfigAsync(guild.Id);

            if (guildConfig == null)
            {
                return;
            }

            if (!guildConfig.SupervisionEnabled)
            {
                return;
            }

            var messageContent = GetMessageContent(userMessage);
            var adminRoleId = guildConfig.AdminRoleId;
            var userRoleId = guildConfig.UserRoleId;
            var rolesId = user.RoleIds;

            if (adminRoleId.HasValue
                && rolesId.Contains(adminRoleId.Value))
            {
                return;
            }

            var channel = message.Channel;

            if (guildConfig.ChannelsWithoutSupervision.Any(x => x.ChannelId == channel.Id))
            {
                return;
            }

            var lessSeverePunishment = true;

            var hasRole = rolesId.Any(x => x == userRoleId
                || x == guildConfig.MuteRoleId)
                || !userRoleId.HasValue;

            lessSeverePunishment &= hasRole;

            var muteRole = guild.GetRole(guildConfig.MuteRoleId);
            var userRole = userRoleId.HasValue ? guild.GetRole(userRoleId.Value) : null;
            var notifyChannel = (ITextChannel)await guild.GetChannelAsync(guildConfig.NotificationChannelId);
            var decision = SupervisorAction.None;

            // TO-DO Lock
            decision = await _userMessageSupervisor.MakeDecisionAsync(guild.Id, user.Id, messageContent, lessSeverePunishment);

            var moderatorService = serviceProvider.GetRequiredService<IModeratorService>();

            switch (decision)
            {
                case SupervisorAction.Warn:
                    await channel.SendMessageAsync("",
                        embed: $"{user.Mention} zaraz przekroczysz granicę!".ToEmbedMessage(EMType.Bot).Build());
                    break;

                case SupervisorAction.Mute:
                    if (muteRole == null)
                    {
                        return;
                    }

                    if (user.RoleIds.Contains(muteRole.Id))
                    {
                        return;
                    }

                    var info = await moderatorService.MuteUserAsync(
                        user,
                        muteRole,
                        null,
                        userRole,
                        TimeSpan.FromDays(1),
                        "spam/flood");
                    await moderatorService.NotifyAboutPenaltyAsync(user, notifyChannel, info);
                    break;

                case SupervisorAction.Ban:
                    await guild.AddBanAsync(user, 1, "Supervisor(ban) spam/flood");
                    break;

                default:
                case SupervisorAction.None:
                    break;
            }
        }
    }
}
