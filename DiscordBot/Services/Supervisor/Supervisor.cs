﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.Extensions;

namespace Sanakan.Services.Supervisor
{
    public class Supervisor
    {
        private enum Action
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

        private SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private Dictionary<ulong, Dictionary<ulong, SupervisorEntity>> _guilds;

        private readonly DiscordSocketClient _client;
        private readonly ModeratorService _moderatorService;
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ISystemClock _systemClock;
        private readonly IOptionsMonitor<SanakanConfiguration> _config;

        private Timer _timer;

        public Supervisor(
            DiscordSocketClient client,
            IOptionsMonitor<SanakanConfiguration> config,
            ILogger<Supervisor> logger,
            ModeratorService moderatorService,
            IServiceScopeFactory serviceScopeFactory,
            ISystemClock systemClock)
        {
            _moderatorService = moderatorService;
            _client = client;
            _config = config;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _systemClock = systemClock;

            _guilds = new Dictionary<ulong, Dictionary<ulong, SupervisorEntity>>();

            _timer = new Timer(async _ =>
            {
                await _semaphore.WaitAsync();

                try
                {
                    AutoValidate();
                }
                finally
                {
                    _semaphore.Release();
                }
            },
            null,
            TimeSpan.FromMinutes(5),
            TimeSpan.FromMinutes(5));
#if !DEBUG
            _client.MessageReceived += HandleMessageAsync;
#endif
        }

        private async Task HandleMessageAsync(SocketMessage message)
        {
            if (!_config.CurrentValue.Supervision)
            {
                return;
            }

            var msg = message as SocketUserMessage;
            
            if (msg == null)
            {
                return;
            }

            if (msg.Author.IsBot || msg.Author.IsWebhook)
            {
                return;
            }

            var user = msg.Author as SocketGuildUser;
            
            if (user == null)
            {
                return;
            }

            if (_config.CurrentValue.BlacklistedGuilds.Any(x => x == user.Guild.Id))
            {
                return;
            }

            _ = Task.Run(async () =>
            {
                await _semaphore.WaitAsync();

                try
                {
                    await Analize(user, msg);
                }
                finally
                {
                    _semaphore.Release();
                }
            });

            await Task.CompletedTask;
        }

        private async Task Analize(SocketGuildUser user, SocketUserMessage message)
        {
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
            var messageContent = GetMessageContent(message);
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

            var thisMessage = susspect.Get(messageContent);
            if (!thisMessage.IsValid())
            {
                thisMessage = new SupervisorMessage(messageContent);
            }

            if (gConfig.AdminRole != 0)
                if (user.Roles.Any(x => x.Id == gConfig.AdminRole))
                    return;

            if (gConfig.ChannelsWithoutSupervision.Any(x => x.Channel == message.Channel.Id))
                return;

            var muteRole = user.Guild.GetRole(gConfig.MuteRole);
            var userRole = user.Guild.GetRole(gConfig.UserRole);
            var notifChannel = user.Guild.GetTextChannel(gConfig.NotificationChannel);

            bool hasRole = user.Roles.Any(x => x.Id == gConfig.UserRole || x.Id == gConfig.MuteRole) || gConfig.UserRole == 0;
            var action = MakeDecision(messageContent, susspect.Inc(_systemClock.UtcNow), thisMessage.Inc(), hasRole);
            await MakeActionAsync(action, user, message, userRole, muteRole, notifChannel);
        }

        private async Task MakeActionAsync(Action action, SocketGuildUser user, SocketUserMessage message, SocketRole userRole, SocketRole muteRole, ITextChannel notifChannel)
        {
            switch (action)
            {
                case Action.Warn:
                    await message.Channel.SendMessageAsync("",
                        embed: $"{user.Mention} zaraz przekroczysz granicę!".ToEmbedMessage(EMType.Bot).Build());
                    break;

                case Action.Mute:
                    if (muteRole != null)
                    {
                        if (user.Roles.Contains(muteRole))
                            return;

                        var info = await _moderatorService.MuteUserAysnc(
                            user,
                            muteRole,
                            null,
                            userRole,
                            24,
                            "spam/flood");
                        await _moderatorService.NotifyAboutPenaltyAsync(user, notifChannel, info);
                    }
                    break;

                case Action.Ban:
                    await user.Guild.AddBanAsync(user, 1, "Supervisor(ban) spam/flood");
                    break;

                default:
                case Action.None:
                    break;
            }
        }

        private Action MakeDecision(string content, int total, int specified, bool hasRole)
        {
            int mSpecified = MAX_SPECIFIED;
            int mTotal = MAX_TOTAL;

            if (content.IsCommand(_config.CurrentValue.Prefix))
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
                return Action.Warn;

            if (total == mTotal || specified == mSpecified)
            {
                if (!hasRole) return Action.Ban;
                return Action.Mute;
            }

            return Action.None;
        }

        private string GetMessageContent(SocketUserMessage message)
        {
            string content = message.Content;
            if (string.IsNullOrEmpty(message.Content))
                content = message?.Attachments?.FirstOrDefault()?.Filename ?? "embed";

            return content;
        }

        private void AutoValidate()
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
                        _guilds[guild.Key][uId] = new SupervisorEntity(null, _systemClock.UtcNow);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Supervisor: autovalidate error {ex}");
            }
        }
    }
}
