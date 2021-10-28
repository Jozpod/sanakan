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
using Sanakan.Common.Configuration;
using Sanakan.DAL.Models.Management;
using System.Linq;
using Sanakan.DiscordBot;
using Discord.WebSocket;
using System.Collections.Generic;

namespace Sanakan.Web.HostedService
{
    public class ModeratorHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly ISystemClock _systemClock;
        private readonly IDiscordSocketClientAccessor _discordSocketClientAccessor;
        private readonly IOptionsMonitor<DaemonsConfiguration> _options;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly Process _process;
        private readonly IOperatingSystem _operatingSystem;
        private readonly ITimer _timer;
        private readonly ICacheManager _cacheManager;
        private const int MB = 1048576;

        public ModeratorHostedService(
            ILogger<MemoryUsageHostedService> logger,
            IOptionsMonitor<DaemonsConfiguration> options,
            ISystemClock systemClock,
            IServiceScopeFactory serviceScopeFactory,
            IOperatingSystem operatingSystem,
            ITimer timer)
        {
            _logger = logger;
            _systemClock = systemClock;
            _serviceScopeFactory = serviceScopeFactory;
            _options = options;
            _operatingSystem = operatingSystem;
            _timer = timer;
            _process = _operatingSystem.GetCurrentProcess();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                stoppingToken.ThrowIfCancellationRequested();
                _timer.Tick += OnTick;
                _timer.Start(
                    _options.CurrentValue.CaptureMemoryUsageDueTime,
                    _options.CurrentValue.CaptureMemoryUsagePeriod);

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _timer.Stop();
            }
        }

        private async void OnTick(object sender, TimerEventArgs e)
        {
            using var serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var penaltyInfoRepository = serviceProvider.GetRequiredService<IPenaltyInfoRepository>();
            var guildConfigRepository = serviceProvider.GetRequiredService<IGuildConfigRepository>();

            foreach (var penalty in await penaltyInfoRepository.GetCachedFullPenalties())
            {
                var guild = _discordSocketClientAccessor.Client.GetGuild(penalty.GuildId);

                if (guild == null)
                {
                    continue;
                }

                var user = guild.GetUser(penalty.UserId);

                if (user == null)
                {
                    if ((_systemClock.UtcNow - penalty.StartDate) > penalty.Duration)
                    {
                        if (penalty.Type == PenaltyType.Ban)
                        {
                            var ban = await guild.GetBanAsync(penalty.UserId);
                            if (ban != null)
                            {
                                await guild.RemoveBanAsync(penalty.UserId);
                            }
                        }
                        await RemovePenaltyFromDb(penalty);
                    }
                    continue;
                }

                var gconfig = await guildConfigRepository.GetCachedGuildFullConfigAsync(guild.Id);
                var muteModRole = guild.GetRole(gconfig.ModMuteRoleId);
                var muteRole = guild.GetRole(gconfig.MuteRoleId);

                if ((_systemClock.UtcNow - penalty.StartDate) < penalty.Duration)
                {
                    var muteMod = penalty
                        .Roles
                        .Any(x => gconfig.ModeratorRoles.Any(z =>
                            z.Role == x.Role)) ? muteModRole : null;

                    _ = Task.Run(async () => {
                        await MuteUserGuildAsync(user, muteRole, penalty.Roles, muteMod);
                    });
                    continue;
                }

                if (penalty.Type == PenaltyType.Mute)
                {
                    await UnmuteUserGuildAsync(user, muteRole, muteModRole, penalty.Roles);
                    await RemovePenaltyFromDb(penalty);
                }
            }
        }

        private async Task UnmuteUserGuildAsync(SocketGuildUser user, SocketRole muteRole, SocketRole muteModRole, IEnumerable<OwnedRole> roles)
        {
            if (muteRole != null)
            {
                if (user.Roles.Contains(muteRole))
                    await user.RemoveRoleAsync(muteRole);
            }

            if (muteModRole != null)
            {
                if (user.Roles.Contains(muteModRole))
                    await user.RemoveRoleAsync(muteModRole);
            }

            if (roles != null)
            {
                foreach (var role in roles)
                {
                    var r = user.Guild.GetRole(role.Role);

                    if (r != null)
                        if (!user.Roles.Contains(r))
                            await user.AddRoleAsync(r);
                }
            }
        }

        private async Task MuteUserGuildAsync(
         SocketGuildUser user,
         SocketRole muteRole,
         IEnumerable<OwnedRole> roles, SocketRole modMuteRole = null)
        {
            if (muteRole != null)
            {
                if (!user.Roles.Contains(muteRole))
                    await user.AddRoleAsync(muteRole);
            }

            if (modMuteRole != null)
            {
                if (!user.Roles.Contains(modMuteRole))
                    await user.AddRoleAsync(modMuteRole);
            }

            if (roles != null)
            {
                foreach (var role in roles)
                {
                    var r = user.Guild.GetRole(role.Role);

                    if (r != null)
                        if (user.Roles.Contains(r))
                            await user.RemoveRoleAsync(r);
                }
            }
        }

        private async Task RemovePenaltyFromDb(PenaltyInfo penalty)
        {
            if (penalty == null)
            {
                return;
            }

            using var serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var penaltyInfoRepository = serviceProvider.GetRequiredService<IPenaltyInfoRepository>();

            penaltyInfoRepository.Remove(penalty);
            await penaltyInfoRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"mute" });
        }
    }
}
