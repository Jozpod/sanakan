using Discord;
using DiscordBot.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Abstractions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Daemon.HostedService
{
    internal class ProfileHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly ISystemClock _systemClock;
        private readonly IDiscordClientAccessor _discordClientAccessor;
        private readonly IOptionsMonitor<DaemonsConfiguration> _options;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ITimer _timer;
        private readonly ITaskManager _taskManager;
        private bool _isRunning;

        public ProfileHostedService(
            ILogger<ProfileHostedService> logger,
            ISystemClock systemClock,
            IDiscordClientAccessor discordClientAccessor,
            IOptionsMonitor<DaemonsConfiguration> options,
            IServiceScopeFactory serviceScopeFactory,
            ITimer timer,
            ITaskManager taskManager)
        {
            _logger = logger;
            _systemClock = systemClock;
            _discordClientAccessor = discordClientAccessor;
            _options = options;
            _serviceScopeFactory = serviceScopeFactory;
            _timer = timer;
            _taskManager = taskManager;
            _discordClientAccessor.Ready += ReadyAsync;
            _discordClientAccessor.LoggedOut += LoggedOut;
        }

        public async Task RemoveUserColorAsync(IGuildUser user, IGuild guild)
        {
            if (user == null)
            {
                return;
            }

            var colors = FColorExtensions.FColors;

            foreach (uint color in colors)
            {
                var role = guild.Roles
                   .Join(user.RoleIds, pr => pr.Id, pr => pr, (src, dst) => src)
                   .FirstOrDefault(x => x.Name == color.ToString());

                if (role == null)
                {
                    continue;
                }

                var users = await guild.GetUsersAsync();
                var members = users
                    .Where(x => x.RoleIds.Any(id => id == role.Id));

                if (members.Count() == 1)
                {
                    await role.DeleteAsync();
                    return;
                }

                await user.RemoveRoleAsync(role);
            }
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
                _logger.LogInformation("profile background service stopped");
            }
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
                var client = _discordClientAccessor.Client;

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

                    var guildId = timeStatus.GuildId!.Value;

                    var guild = await client.GetGuildAsync(guildId);

                    switch (timeStatus.Type)
                    {
                        case StatusType.Globals:
                            var guildConfig = await guildConfigRepository.GetCachedById(guildId);
                            var roleId = guildConfig?.GlobalEmotesRoleId;

                            if (roleId.HasValue)
                            {
                                await RemoveRoleAsync(guild, roleId.Value, timeStatus.UserId);
                            }

                            break;

                        case StatusType.Color:
                            var user = await guild.GetUserAsync(timeStatus.UserId);
                            await RemoveUserColorAsync(user, guild);
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("profile background service canceled");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred in profile background service", ex);
            }
            finally
            {
                _isRunning = false;
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

        private Task LoggedOut()
        {
            _timer.Stop();
            return Task.CompletedTask;
        }

        private Task ReadyAsync()
        {
            _timer.Start(
                _options.CurrentValue.ProfileDueTime,
                _options.CurrentValue.ProfilePeriod);
            return Task.CompletedTask;
        }
    }
}
