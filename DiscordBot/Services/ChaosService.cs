using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Configuration;
using Sanakan.Extensions;

namespace Sanakan.Services
{
    public class ChaosService
    {
        private readonly DiscordSocketClient _client;
        private List<ulong> _changed;
        private readonly IOptionsMonitor<BotConfiguration> _config;
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly IGuildConfigRepository _guildConfigRepository;
        private readonly ILogger _logger;
        private readonly Timer _timer;

        public ChaosService(
            DiscordSocketClient client,
            IRandomNumberGenerator randomNumberGenerator,
            IGuildConfigRepository guildConfigRepository,
            IOptionsMonitor<BotConfiguration> config,
            ILogger<ChaosService> logger)
        {
            _client = client;
            _randomNumberGenerator = randomNumberGenerator;
            _guildConfigRepository = guildConfigRepository;
            _config = config;
            _logger = logger;
            _changed = new List<ulong>();
            _timer = new Timer(_ =>
            {
                try
                {
                    _changed = new List<ulong>();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"in chaos: {ex}", ex);
                    _changed.Clear();
                }
            },
            null,
            TimeSpan.FromHours(1),
            TimeSpan.FromHours(1));

#if !DEBUG
            _client.MessageReceived += HandleMessageAsync;
#endif
        }

        private async Task HandleMessageAsync(SocketMessage message)
        {
            var userMessage = message as SocketUserMessage;
            
            if (userMessage == null)
            {
                return;
            }

            if (userMessage.Author.IsBot || userMessage.Author.IsWebhook)
            {
                return;
            }

            var sourceUser = userMessage.Author as SocketGuildUser;
            
            if (sourceUser == null)
            {
                return;
            }

            if (_config.CurrentValue.BlacklistedGuilds.Any(x => x == sourceUser.Guild.Id))
            {
                return;
            }

            var gConfig = await _guildConfigRepository.GetCachedGuildFullConfigAsync(sourceUser.Guild.Id);

            if (gConfig == null)
            {
                return;
            }

            if (!gConfig.ChaosMode)
            {
                return;
            }

            if (!_randomNumberGenerator.TakeATry(3))
            {
                return;
            }

            var notChangedUsers = sourceUser.Guild
                .Users
                .Where(x => !x.IsBot
                    && x.Id != sourceUser.Id
                    && !_changed.Any(c => c == x.Id))
                .ToList();

            if (notChangedUsers.Count < 2)
            {
                return;
            }

            if (_changed.Any(x => x == sourceUser.Id))
            {
                sourceUser = _randomNumberGenerator.GetOneRandomFrom(notChangedUsers);
                notChangedUsers.Remove(sourceUser);
            }

            var targetUser = _randomNumberGenerator.GetOneRandomFrom(notChangedUsers);

            var sourceNickname = sourceUser.Nickname ?? sourceUser.Username;
            var targetNickname = targetUser.Nickname ?? targetUser.Username;

            await sourceUser.ModifyAsync(x => x.Nickname = targetNickname);
            await targetUser.ModifyAsync(x => x.Nickname = sourceNickname);
            _changed.AddRange(new[] { sourceUser.Id, targetUser.Id });
        }
    }
}
