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
using System.Collections.Generic;
using Sanakan.DiscordBot;
using Discord.WebSocket;
using Sanakan.Common.Configuration;
using System.Linq;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Abstractions.Models;
using Discord;
using Sanakan.Extensions;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.DiscordBot.Supervisor;

namespace Sanakan.Web.HostedService
{
    public class SupervisorHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly ISystemClock _systemClock;
        private readonly IDiscordClientAccessor _discordSocketClientAccessor;
        private readonly IOptionsMonitor<DaemonsConfiguration> _daemonsConfiguration;
        private readonly IOptionsMonitor<DiscordConfiguration> _discordConfiguration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ITaskManager _taskManager;
        private readonly ITimer _timer;
        private readonly IUserMessageSupervisor _userMessageSupervisor;
        private readonly IUserJoinedGuildSupervisor _userJoinedGuildSupervisor;
        private bool _isRunning;

        public SupervisorHostedService(
            ILogger<SupervisorHostedService> logger,
            IDiscordClientAccessor discordSocketClientAccessor,
            IOptionsMonitor<DaemonsConfiguration> daemonsConfiguration,
            IOptionsMonitor<DiscordConfiguration> discordConfiguration,
            ISystemClock systemClock,
            IServiceScopeFactory serviceScopeFactory,
            ITaskManager taskManager,
            ITimer timer,
            IUserMessageSupervisor userMessageSupervisor,
            IUserJoinedGuildSupervisor userJoinedGuildSupervisor)
        {
            _logger = logger;
            _discordSocketClientAccessor = discordSocketClientAccessor;
            _systemClock = systemClock;
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

        private string GetMessageContent(IUserMessage message)
        {
            string content = message.Content;
            if (string.IsNullOrEmpty(message.Content))
            {
                content = message?.Attachments?.FirstOrDefault()?.Filename ?? "embed";
            }

            return content;
        }

        private async Task UserJoinedAsync(IGuildUser user)
        {
            if (!_discordConfiguration.CurrentValue.FloodSpamSupervisionEnabled)
            {
                return;
            }

            if (user.IsBot || user.IsWebhook)
            {
                return;
            }

            if (_discordConfiguration.CurrentValue.BlacklistedGuilds.Any(x => x == user.Guild.Id))
            {
                return;
            }

            using var serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var guildConfigRepository = serviceProvider.GetRequiredService<IGuildConfigRepository>();

            var guildConfig = await guildConfigRepository.GetCachedGuildFullConfigAsync(user.Guild.Id);

            if (guildConfig == null)
            {
                return;
            }

            if (!guildConfig.SupervisionEnabled)
            {
                return;
            }

            var guild = user.Guild;
            var userIds = _userJoinedGuildSupervisor.GetUsersToBanCauseRaid(user.Guild.Id, user.Username, user.Id);
            var usersToBan = await Task.WhenAll(userIds.Select(pr => guild.GetUserAsync(pr)));
            foreach (var userToBan in usersToBan)
            {
                await guild.AddBanAsync(user, 1, $"Supervisor(ban) raid/scam [{user.Nickname}]");
            }

        }

        private async Task HandleMessageAsync(IMessage message)
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

            if (userMessage.Author.IsBot || userMessage.Author.IsWebhook)
            {
                return;
            }

            var user = userMessage.Author as IGuildUser;

            if (user == null)
            {
                return;
            }

            if (_discordConfiguration.CurrentValue.BlacklistedGuilds.Any(x => x == user.Guild.Id))
            {
                return;
            }

            using var serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var guildConfigRepository = serviceProvider.GetRequiredService<IGuildConfigRepository>();

            var gConfig = await guildConfigRepository.GetCachedGuildFullConfigAsync(user.Guild.Id);

            if (gConfig == null)
            {
                return;
            }

            if (!gConfig.SupervisionEnabled)
            {
                return;
            }

            var messageContent = GetMessageContent(userMessage);
            var adminRoleId = gConfig.AdminRoleId;
            var userRoleId = gConfig.UserRoleId;

            if (adminRoleId.HasValue
                && user.RoleIds.Contains(adminRoleId.Value))
            {
                return;
            }

            if (gConfig.ChannelsWithoutSupervision.Any(x => x.Channel == message.Channel.Id))
            {
                return;
            }

            var lessSeverePunishment = true;

            var hasRole = user.RoleIds.Any(x => x == userRoleId
                || x == gConfig.MuteRoleId)
                || !userRoleId.HasValue;

            lessSeverePunishment &= hasRole;

            var muteRole = user.Guild.GetRole(gConfig.MuteRoleId);
            var userRole = user.Guild.GetRole(userRoleId.Value);
            var notifChannel = (ITextChannel)await user.Guild.GetChannelAsync(gConfig.NotificationChannelId);
            var decision = _userMessageSupervisor.MakeDecision(user.Guild.Id, user.Id, messageContent, lessSeverePunishment);

            var moderatorService = serviceProvider.GetRequiredService<IModeratorService>();

            switch (decision)
            {
                case SupervisorAction.Warn:
                    await message.Channel.SendMessageAsync("",
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
                    await moderatorService.NotifyAboutPenaltyAsync(user, notifChannel, info);
                    break;

                case SupervisorAction.Ban:
                    await user.Guild.AddBanAsync(user, 1, "Supervisor(ban) spam/flood");
                    break;

                default:
                case SupervisorAction.None:
                    break;
            }
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

        private async void OnTick(object sender, TimerEventArgs e)
        {
            if(_isRunning)
            {
                return;
            }

            _isRunning = true;

            try
            {
                _userJoinedGuildSupervisor.Refresh();
                _userMessageSupervisor.Refresh();
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
    }
}
