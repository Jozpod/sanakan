using Discord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.Common.Cache;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Models.Management;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Daemon.HostedService
{
    internal class ModeratorHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly ISystemClock _systemClock;
        private readonly IDiscordClientAccessor _discordClientAccessor;
        private readonly IOptionsMonitor<DaemonsConfiguration> _options;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ITimer _timer;
        private readonly ITaskManager _taskManager;
        private readonly ICacheManager _cacheManager;
        private bool _isRunning;

        public ModeratorHostedService(
            ILogger<ModeratorHostedService> logger,
            ISystemClock systemClock,
            IDiscordClientAccessor discordSocketClientAccessor,
            IOptionsMonitor<DaemonsConfiguration> options,
            IServiceScopeFactory serviceScopeFactory,
            ITimer timer,
            ITaskManager taskManager,
            ICacheManager cacheManager)
        {
            _logger = logger;
            _systemClock = systemClock;
            _discordClientAccessor = discordSocketClientAccessor;
            _serviceScopeFactory = serviceScopeFactory;
            _options = options;
            _timer = timer;
            _taskManager = taskManager;
            _cacheManager = cacheManager;
            _discordClientAccessor.Ready += ReadyAsync;
            _discordClientAccessor.LoggedOut += LoggedOut;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                stoppingToken.ThrowIfCancellationRequested();
                _timer.Tick += OnTick;

                await _taskManager.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _timer.Stop();
            }
        }

        private Task LoggedOut()
        {
            _timer.Stop();
            return Task.CompletedTask;
        }

        private Task ReadyAsync()
        {
            _timer.Start(
                   _options.CurrentValue.ModeratorDueTime,
                   _options.CurrentValue.ModeratorPeriod);
            return Task.CompletedTask;
        }

        private async void OnTick(object sender, TimerEventArgs e)
        {
            if (_isRunning)
            {
                return;
            }

            _isRunning = true;

            try
            {
                using var serviceScope = _serviceScopeFactory.CreateScope();
                var serviceProvider = serviceScope.ServiceProvider;
                var penaltyInfoRepository = serviceProvider.GetRequiredService<IPenaltyInfoRepository>();
                var guildConfigRepository = serviceProvider.GetRequiredService<IGuildConfigRepository>();
                var utcNow = _systemClock.UtcNow;
                var client = _discordClientAccessor.Client;
                var penalties = await penaltyInfoRepository.GetCachedFullPenalties();

                foreach (var penalty in penalties)
                {
                    var guild = await client.GetGuildAsync(penalty.GuildId);

                    if (guild == null)
                    {
                        continue;
                    }

                    var user = await guild.GetUserAsync(penalty.UserId);
                    var isPenaltyActive = (utcNow - penalty.StartedOn) < penalty.Duration;

                    if (user == null)
                    {
                        if (isPenaltyActive)
                        {
                            continue;
                        }

                        if (penalty.Type == PenaltyType.Ban)
                        {
                            var ban = await guild.GetBanAsync(penalty.UserId);

                            if (ban != null)
                            {
                                await guild.RemoveBanAsync(penalty.UserId);
                            }
                        }

                        await RemovePenalty(penaltyInfoRepository, penalty);
                    }

                    var gconfig = await guildConfigRepository.GetCachedById(guild.Id);
                    var muteModRole = guild.GetRole(gconfig.ModMuteRoleId);
                    var muteRole = guild.GetRole(gconfig.MuteRoleId);

                    if (isPenaltyActive)
                    {
                        var muteMod = penalty
                            .Roles
                            .Any(x => gconfig.ModeratorRoles.Any(z =>
                                z.RoleId == x.RoleId)) ? muteModRole : null;

                        await MuteUserGuildAsync(user!, muteRole, penalty.Roles, muteMod);
                        continue;
                    }

                    if (penalty.Type == PenaltyType.Mute)
                    {
                        await UnmuteUserGuildAsync(user!, muteRole, muteModRole, penalty.Roles);
                        await RemovePenalty(penaltyInfoRepository, penalty);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in moderator background service");
            }
            finally
            {
                _isRunning = false;
            }
        }

        private async Task UnmuteUserGuildAsync(
            IGuildUser user,
            IRole? muteRole,
            IRole? muteModRole,
            IEnumerable<OwnedRole>? ownerRoles)
        {
            ownerRoles ??= Enumerable.Empty<OwnedRole>();
            var roleIds = user.RoleIds;

            if (muteRole != null)
            {
                if (roleIds.Contains(muteRole.Id))
                {
                    await user.RemoveRoleAsync(muteRole);
                }
            }

            if (muteModRole != null)
            {
                if (roleIds.Contains(muteModRole.Id))
                {
                    await user.RemoveRoleAsync(muteModRole);
                }
            }

            var guild = user.Guild;

            foreach (var ownerRole in ownerRoles)
            {
                var role = guild.GetRole(ownerRole.RoleId);

                if (role == null)
                {
                    continue;
                }

                if (!roleIds.Contains(role.Id))
                {
                    await user.AddRoleAsync(role.Id);
                }
            }
        }

        private async Task MuteUserGuildAsync(
            IGuildUser user,
            IRole? muteRole,
            IEnumerable<OwnedRole>? ownerRoles,
            IRole? modMuteRole = null)
        {
            ownerRoles ??= Enumerable.Empty<OwnedRole>();
            var roleIds = user.RoleIds;

            if (muteRole != null)
            {
                if (!roleIds.Contains(muteRole.Id))
                {
                    await user.AddRoleAsync(muteRole);
                }
            }

            if (modMuteRole != null)
            {
                if (!roleIds.Contains(modMuteRole.Id))
                {
                    await user.AddRoleAsync(modMuteRole);
                }
            }

            var guild = user.Guild;

            foreach (var ownerRole in ownerRoles)
            {
                var role = guild.GetRole(ownerRole.RoleId);

                if (role == null)
                {
                    continue;
                }

                if (!roleIds.Contains(role.Id))
                {
                    await user.RemoveRoleAsync(role.Id);
                }
            }
        }

        private async Task RemovePenalty(IPenaltyInfoRepository penaltyInfoRepository, PenaltyInfo? penalty)
        {
            if (penalty == null)
            {
                return;
            }

            penaltyInfoRepository.Remove(penalty);
            await penaltyInfoRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.Muted);
        }
    }
}
