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
using Sanakan.Common.Configuration;
using Sanakan.DAL.Models.Management;
using System.Linq;
using Sanakan.DiscordBot;
using Discord.WebSocket;
using System.Collections.Generic;
using Sanakan.Common.Cache;
using Discord;

namespace Sanakan.Web.HostedService
{
    public class ModeratorHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly ISystemClock _systemClock;
        private readonly IDiscordClientAccessor _discordSocketClientAccessor;
        private readonly IOptionsMonitor<DaemonsConfiguration> _options;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ITimer _timer;
        private readonly ICacheManager _cacheManager;
        private readonly ITaskManager _taskManager;

        public ModeratorHostedService(
            ILogger<ModeratorHostedService> logger,
            ISystemClock systemClock,
            IDiscordClientAccessor discordSocketClientAccessor,
            IOptionsMonitor<DaemonsConfiguration> options,
            IServiceScopeFactory serviceScopeFactory,
            ITimer timer,
            ITaskManager taskManager)
        {
            _logger = logger;
            _systemClock = systemClock;
            _discordSocketClientAccessor = discordSocketClientAccessor;
            _serviceScopeFactory = serviceScopeFactory;
            _options = options;
            _timer = timer;
            _taskManager = taskManager;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                stoppingToken.ThrowIfCancellationRequested();
                _timer.Tick += OnTick;
                _timer.Start(
                    _options.CurrentValue.ModeratorDueTime,
                    _options.CurrentValue.ModeratorPeriod);

                await _taskManager.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
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
            var utcNow = _systemClock.UtcNow;

            foreach (var penalty in await penaltyInfoRepository.GetCachedFullPenalties())
            {
                var guild = await _discordSocketClientAccessor.Client.GetGuildAsync(penalty.GuildId);

                if (guild == null)
                {
                    continue;
                }

                var user = await guild.GetUserAsync(penalty.UserId);

                if (user == null)
                {
                    if ((utcNow - penalty.StartDate) > penalty.Duration)
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

                if ((utcNow - penalty.StartDate) < penalty.Duration)
                {
                    var muteMod = penalty
                        .Roles
                        .Any(x => gconfig.ModeratorRoles.Any(z =>
                            z.RoleId == x.RoleId)) ? muteModRole : null;

                    await MuteUserGuildAsync(user, muteRole, penalty.Roles, muteMod);
                    continue;
                }

                if (penalty.Type == PenaltyType.Mute)
                {
                    await UnmuteUserGuildAsync(user, muteRole, muteModRole, penalty.Roles);
                    await RemovePenaltyFromDb(penalty);
                }
            }
        }

        private async Task UnmuteUserGuildAsync(
            IGuildUser user,
            IRole muteRole,
            IRole muteModRole,
            IEnumerable<OwnedRole> ownerRoles)
        {
            if (muteRole != null)
            {
                if (user.RoleIds.Contains(muteRole.Id))
                {
                    await user.RemoveRoleAsync(muteRole);
                }
            }

            if (muteModRole != null)
            {
                if (user.RoleIds.Contains(muteModRole.Id))
                {
                    await user.RemoveRoleAsync(muteModRole);
                }
            }

            if (ownerRoles != null)
            {
                foreach (var ownerRole in ownerRoles)
                {
                    var role = user.Guild.GetRole(ownerRole.RoleId);

                    if (role == null)
                    {
                        continue;
                    }

                    if (!user.RoleIds.Contains(role.Id))
                    {
                        await user.AddRoleAsync(role.Id);
                    }
                }
            }
        }

        private async Task MuteUserGuildAsync(
            IGuildUser user,
            IRole muteRole,
            IEnumerable<OwnedRole> ownerRoles,
            IRole? modMuteRole = null)
        {
            if (muteRole != null)
            {
                if (!user.RoleIds.Contains(muteRole.Id))
                {
                    await user.AddRoleAsync(muteRole);
                }
            }

            if (modMuteRole != null)
            {
                if (!user.RoleIds.Contains(modMuteRole.Id))
                {
                    await user.AddRoleAsync(modMuteRole);
                }
            }

            if (ownerRoles != null)
            {
                foreach (var ownerRole in ownerRoles)
                {
                    var role = user.Guild.GetRole(ownerRole.RoleId);

                    if (role == null)
                    {
                        continue;
                    }

                    if (!user.RoleIds.Contains(role.Id))
                    {
                        await user.AddRoleAsync(role.Id);
                    }
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

            _cacheManager.ExpireTag(CacheKeys.Muted);
        }
    }
}
