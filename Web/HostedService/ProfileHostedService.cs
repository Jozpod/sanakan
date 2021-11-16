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
using System.Collections.Concurrent;
using Sanakan.DiscordBot;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Models;
using Discord.WebSocket;
using Discord;
using System.Linq;
using DiscordBot.Services;

namespace Sanakan.Web.HostedService
{
    public class ProfileHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly ISystemClock _systemClock;
        private readonly IDiscordClientAccessor _discordSocketClientAccessor;
        private readonly IOptionsMonitor<DaemonsConfiguration> _options;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ITimer _timer;
        private readonly ITaskManager _taskManager;

        public ProfileHostedService(
            ILogger<ProfileHostedService> logger,
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
            _options = options;
            _serviceScopeFactory = serviceScopeFactory;
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
                    _options.CurrentValue.ProfileDueTime,
                    _options.CurrentValue.ProfilePeriod);

                await _taskManager.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                
            }
        }

        private async void OnTick(object sender, TimerEventArgs e)
        {
            var client = _discordSocketClientAccessor.Client;

            if(client == null)
            {
                return;
            }

            using var serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var guildConfigRepository = serviceProvider.GetRequiredService<IGuildConfigRepository>();
            var timeStatusRepository = serviceProvider.GetRequiredService<ITimeStatusRepository>();

            var timeStatuses = await timeStatusRepository.GetBySubTypeAsync();

            foreach (var timeStatus in timeStatuses)
            {
                if (timeStatus.IsActive(_systemClock.UtcNow))
                {
                    continue;
                }

                var guild = await client.GetGuildAsync(timeStatus.GuildId.Value);

                switch (timeStatus.Type)
                {
                    case StatusType.Globals:
                        var guildConfig = await guildConfigRepository.GetCachedGuildFullConfigAsync(timeStatus.GuildId.Value);
                        await RemoveRoleAsync(guild, guildConfig?.GlobalEmotesRoleId ?? 0, timeStatus.UserId);
                        break;

                    case StatusType.Color:
                        var user = await guild.GetUserAsync(timeStatus.UserId);
                        await RomoveUserColorAsync(user);
                        break;

                    default:
                        break;
                }
            }
        }

        public async Task RomoveUserColorAsync(IGuildUser user)
        {
            if (user == null)
            {
                return;
            }

            var colors = FColorExtensions.FColors;
            var guild = user.Guild;

            foreach (uint color in colors)
            {
                var role = guild.Roles
                   .Join(user.RoleIds, pr => pr.Id, pr => pr, (src, dst) => src)
                   .FirstOrDefault(x => x.Name == color.ToString());

                if (role == null)
                {
                    continue;
                }

                var members = (await guild.GetUsersAsync()).Where(x => x.RoleIds.Any(id => id == role.Id));

                if (members.Count() == 1)
                {
                    await role.DeleteAsync();
                    return;
                }

                await user.RemoveRoleAsync(role);
            }
        }

        private async Task RemoveRoleAsync(IGuild guild, ulong roleId, ulong userId)
        {
            var role = guild.GetRole(roleId);

            if (role == null)
            {
                return;
            }

            var user = await guild.GetUserAsync(userId);

            if (user == null)
            {
                return;
            }

            await user.RemoveRoleAsync(role);
        }
    }
}
