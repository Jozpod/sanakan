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
using Sanakan.Services.Supervisor;
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

namespace Sanakan.Web.HostedService
{
    public class SupervisorHostedService : BackgroundService
    {
        private enum SupervisorAction
        {
            None,
            Ban,
            Mute,
            Warn
        }

        private const int MAX_TOTAL = 13;
        private const int MAX_SPECIFIED = 8;

        private const int COMMAND_MOD = 2;
        private const int UNCONNECTED_MOD = -2;

        private readonly ILogger _logger;
        private readonly ISystemClock _systemClock;
        private readonly IDiscordSocketClientAccessor _discordSocketClientAccessor;
        private readonly IOptionsMonitor<DaemonsConfiguration> _daemonsConfiguration;
        private readonly IOptionsMonitor<DiscordConfiguration> _discordConfiguration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        private readonly ITimer _timer;
        private readonly Dictionary<ulong, Dictionary<ulong, SupervisorEntity>> _guilds;

        public SupervisorHostedService(
            ILogger<MemoryUsageHostedService> logger,
            IOptionsMonitor<DaemonsConfiguration> daemonsConfiguration,
            IOptionsMonitor<DiscordConfiguration> discordConfiguration,
            ISystemClock systemClock,
            IServiceScopeFactory serviceScopeFactory,
            ITimer timer)
        {
            _guilds = new Dictionary<ulong, Dictionary<ulong, SupervisorEntity>>();
            _logger = logger;
            _systemClock = systemClock;
            _serviceScopeFactory = serviceScopeFactory;
            _daemonsConfiguration = daemonsConfiguration;
            _discordConfiguration = discordConfiguration;
            _timer = timer;

            _discordSocketClientAccessor.Initialized += Initialized;
        }

        private Task Initialized()
        {
            _discordSocketClientAccessor.Client.MessageReceived += HandleMessageAsync;
            return Task.CompletedTask;
        }

        private string GetMessageContent(SocketUserMessage message)
        {
            string content = message.Content;
            if (string.IsNullOrEmpty(message.Content))
                content = message?.Attachments?.FirstOrDefault()?.Filename ?? "embed";

            return content;
        }

        private async Task HandleMessageAsync(SocketMessage message)
        {
            if (!_discordConfiguration.CurrentValue.FloodSpamSupervisionEnabled)
            {
                return;
            }

            var userMessage = message as SocketUserMessage;

            if (userMessage == null)
            {
                return;
            }

            if (userMessage.Author.IsBot || userMessage.Author.IsWebhook)
            {
                return;
            }

            var user = userMessage.Author as SocketGuildUser;

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

            if (!gConfig.Supervision)
            {
                return;
            }

            if (!_guilds.Any(x => x.Key == user.Guild.Id))
            {
                _guilds.Add(user.Guild.Id, new Dictionary<ulong, SupervisorEntity>());
                return;
            }

            var guild = _guilds[user.Guild.Id];
            var messageContent = GetMessageContent(userMessage);
            if (!guild.Any(x => x.Key == user.Id))
            {
                guild.Add(user.Id, new SupervisorEntity(messageContent, _systemClock.UtcNow));
                return;
            }

            var susspect = guild[user.Id];
            if (!susspect.IsValid(_systemClock.UtcNow))
            {
                susspect = new SupervisorEntity(messageContent, _systemClock.UtcNow);
                return;
            }

            var utcNow = _systemClock.UtcNow;
            var thisMessage = susspect.Get(utcNow, messageContent);

            if (!thisMessage.IsValid(utcNow))
            {
                thisMessage = new SupervisorMessage(utcNow, messageContent);
            }

            if (gConfig.AdminRoleId != 0)
                if (user.Roles.Any(x => x.Id == gConfig.AdminRoleId))
                {
                    return;
                }

            if (gConfig.ChannelsWithoutSupervision.Any(x => x.Channel == message.Channel.Id))
            {
                return;
            }

            var muteRole = user.Guild.GetRole(gConfig.MuteRoleId);
            var userRole = user.Guild.GetRole(gConfig.UserRoleId);
            var notifChannel = user.Guild.GetTextChannel(gConfig.NotificationChannelId);

            bool hasRole = user.Roles.Any(x => x.Id == gConfig.UserRoleId || x.Id == gConfig.MuteRoleId) || gConfig.UserRoleId == 0;
            var action = MakeDecision(messageContent, susspect.Inc(_systemClock.UtcNow), thisMessage.Inc(), hasRole);
            await MakeActionAsync(action, user, userMessage, userRole, muteRole, notifChannel);

            await Task.CompletedTask;
        }

        private async Task MakeActionAsync(
           SupervisorAction action,
           SocketGuildUser user,
           SocketUserMessage message,
           SocketRole userRole,
           SocketRole muteRole,
           ITextChannel notifChannel)
        {
            using var serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var moderatorService = serviceProvider.GetRequiredService<IModeratorService>();

            switch (action)
            {
                case SupervisorAction.Warn:
                    await message.Channel.SendMessageAsync("",
                        embed: $"{user.Mention} zaraz przekroczysz granicę!".ToEmbedMessage(EMType.Bot).Build());
                    break;

                case SupervisorAction.Mute:
                    if (muteRole != null)
                    {
                        if (user.Roles.Contains(muteRole))
                            return;

                        var info = await moderatorService.MuteUserAysnc(
                            user,
                            muteRole,
                            null,
                            userRole,
                            TimeSpan.FromDays(1),
                            "spam/flood");
                        await moderatorService.NotifyAboutPenaltyAsync(user, notifChannel, info);
                    }
                    break;

                case SupervisorAction.Ban:
                    await user.Guild.AddBanAsync(user, 1, "Supervisor(ban) spam/flood");
                    break;

                default:
                case SupervisorAction.None:
                    break;
            }
        }

        private SupervisorAction MakeDecision(string content, int total, int specified, bool hasRole)
        {
            int mSpecified = MAX_SPECIFIED;
            int mTotal = MAX_TOTAL;

            if (_discordConfiguration.CurrentValue.IsCommand(content))
            {
                mTotal += COMMAND_MOD;
                mSpecified += COMMAND_MOD;
            }

            if (!hasRole)
            {
                mTotal += UNCONNECTED_MOD;
                mSpecified += UNCONNECTED_MOD;
            }

            int mWSpec = mSpecified - 1;
            int mWTot = mTotal - 1;

            if ((total == mWTot || specified == mWSpec) && hasRole)
                return SupervisorAction.Warn;

            if (total == mTotal || specified == mSpecified)
            {
                if (!hasRole) return SupervisorAction.Ban;
                return SupervisorAction.Mute;
            }

            return SupervisorAction.None;
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

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _timer.Stop();
            }
        }

        private async void OnTick(object sender, TimerEventArgs e)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                try
                {
                    var toClean = new Dictionary<ulong, List<ulong>>();
                    foreach (var guild in _guilds)
                    {
                        var users = new List<ulong>();
                        foreach (var susspect in guild.Value)
                        {
                            if (!susspect.Value.IsValid(_systemClock.UtcNow))
                            {
                                users.Add(susspect.Key);
                            }
                        }
                        toClean.Add(guild.Key, users);
                    }

                    foreach (var guild in toClean)
                    {
                        foreach (var uId in guild.Value)
                        {
                            _guilds[guild.Key][uId] = new SupervisorEntity(null, _systemClock.UtcNow);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"Supervisor: autovalidate error {ex}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not get ", ex);

            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}
